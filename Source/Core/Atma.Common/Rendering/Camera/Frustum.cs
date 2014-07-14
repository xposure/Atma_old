using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Atma.Rendering.Camera
{
    /// <summary>
    /// Defines the 6 planes the make up a frustum.
    /// </summary>
    public enum FrustumPlane
    {
        Near = 0,
        Far,
        Left,
        Right,
        Top,
        Bottom,

        /// <summary>Used for methods that require returning a value of this type but cannot return null.</summary>
        None
    }

    /// <summary>
    /// Type of projection used by the camera.
    /// </summary>
    public enum ProjectionType
    {
        /// <summary> Things stay the same size no matter where they are in terms of the camera.  Normally only used in 3D HUD elements. </summary>
        Orthographic,

        /// <summary> Things get smaller when they are furthur away from the camera. </summary>
        Perspective
    }

    /// <summary>
    ///     A frustum represents a pyramid, capped at the near and far end which is
    ///     used to represent either a visible area or a projection area. Can be used
    ///     for a number of applications.
    /// </summary>
    public class Frustum 
    {
        #region Constants

        /// <summary>
        ///		Small constant used to reduce far plane projection to avoid inaccuracies.
        /// </summary>
        public const float InfiniteFarPlaneAdjust = 0.00001f;

        /// <summary>
        ///		Arbitrary large distance to use for the far plane when set to 0 (infinite far plane).
        /// </summary>
        public const float InfiniteFarPlaneDistance = 100000.0f;

        #endregion Constants

        #region Fields and Properties

        #region ProjectionType Property

        /// <summary>
        ///		Perspective or Orthographic?
        /// </summary>
        private ProjectionType _projectionType;

        /// <summary>
        ///    Gets/Sets the type of projection to use (orthographic or perspective). Default is perspective.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return _projectionType; }
            set
            {
                _projectionType = value;
                InvalidateFrustum();
            }
        }

        #endregion ProjectionType Property

        #region FieldOfView Property

        /// <summary>
        ///     y-direction field-of-view (default 45).
        /// </summary>
        private float _fieldOfView;

        /// <summary>
        ///		Sets the Y-dimension Field Of View (FOV) of the camera.
        /// </summary>
        /// <remarks>
        ///		Field Of View (FOV) is the angle made between the camera's position, and the left &amp; right edges
        ///		of the 'screen' onto which the scene is projected. High values (90+ Degrees) result in a wide-angle,
        ///		fish-eye kind of view, low values (30- Degrees) in a stretched, telescopic kind of view. Typical values
        ///		are between 45 and 60 Degrees.
        ///		<p/>
        ///		This value represents the HORIZONTAL field-of-view. The vertical field of view is calculated from
        ///		this depending on the dimensions of the viewport (they will only be the same if the viewport is square).
        /// </remarks>
        virtual public float FieldOfView
        {
            get { return _fieldOfView; }
            set
            {
                _fieldOfView = value;
                InvalidateFrustum();
                InvalidateView();
            }
        }

        #endregion FieldOfView Property

        #region Far Property

        /// <summary>
        ///     Far clip distance - default 10000.
        /// </summary>
        protected float _farDistance;

        /// <summary>
        ///		Gets/Sets the distance to the far clipping plane.
        ///	 </summary>
        ///	 <remarks>
        ///		The view frustrum is a pyramid created from the camera position and the edges of the viewport.
        ///		This frustrum does not extend to infinity - it is cropped near to the camera and there is a far
        ///		plane beyond which nothing is displayed. This method sets the distance for the far plane. Different
        ///		applications need different values: e.g. a flight sim needs a much further far clipping plane than
        ///		a first-person shooter. An important point here is that the larger the gap between near and far
        ///		clipping planes, the lower the accuracy of the Z-buffer used to depth-cue pixels. This is because the
        ///		Z-range is limited to the size of the Z buffer (16 or 32-bit) and the max values must be spread over
        ///		the gap between near and far clip planes. The bigger the range, the more the Z values will
        ///		be approximated which can cause artifacts when lots of objects are close together in the Z-plane. So
        ///		make sure you clip as close to the camera as you can - don't set a huge value for the sake of
        ///		it.
        /// </remarks>
        /// <value>
        ///		The distance to the far clipping plane from the frustum in
        ///		world coordinates.  If you specify 0, this means an infinite view
        ///		distance which is useful especially when projecting shadows; but
        ///		be careful not to use a near distance too close.
        /// </value>
        virtual public float Far
        {
            get { return _farDistance; }
            set
            {
                _farDistance = value;
                InvalidateFrustum();
                InvalidateView(); //XEONX FIX: Now the IsObjectVisible() will work properly
            }
        }

        #endregion Far Property

        #region Near Property

        /// <summary>
        ///     Near clip distance - default 100.
        /// </summary>
        private float _nearDistance;

        /// <summary>
        ///		Gets/Sets the position of the near clipping plane.
        ///	</summary>
        ///	<remarks>
        ///		The position of the near clipping plane is the distance from the cameras position to the screen
        ///		on which the world is projected. The near plane distance, combined with the field-of-view and the
        ///		aspect ratio, determines the size of the viewport through which the world is viewed (in world
        ///		co-ordinates). Note that this world viewport is different to a screen viewport, which has it's
        ///		dimensions expressed in pixels. The cameras viewport should have the same aspect ratio as the
        ///		screen viewport it renders into to avoid distortion.
        /// </remarks>
        virtual public float Near
        {
            get { return _nearDistance; }
            set
            {
                Debug.Assert(value > 0, "Near clip distance must be greater than zero.");

                _nearDistance = value;
                InvalidateFrustum();
                InvalidateView(); //XEONX FIX: Now the IsObjectVisible() will work properly
            }
        }

        #endregion Near Property

        #region AspectRatio Property

        /// <summary>
        ///     x/y viewport ratio - default 1.3333
        /// </summary>
        private float _aspectRatio;

        /// <summary>
        ///		Gets/Sets the aspect ratio to use for the camera viewport.
        /// </summary>
        /// <remarks>
        ///		The ratio between the x and y dimensions of the rectangular area visible through the camera
        ///		is known as aspect ratio: aspect = width / height .
        ///		<p/>
        ///		The default for most fullscreen windows is 1.3333f - this is also assumed unless you
        ///		use this property to state otherwise.
        /// </remarks>
        virtual public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                _aspectRatio = value;
                InvalidateFrustum();
            }
        }

        #endregion AspectRatio Property

        #region FrustumOffset Property

        ///<summary>
        /// Off-axis frustum center offset - default (0.0, 0.0)
        ///</summary>
        protected Vector2 _frustumOffset;

        ///<summary>
        /// Off-axis frustum center offset - default (0.0, 0.0)
        ///</summary>
        public Vector2 FrustumOffset
        {
            get { return _frustumOffset; }
            set
            {
                _frustumOffset = value;
                this.UpdateFrustum();
            }
        }

        #endregion FrustumOffset Property

        ///<summary>
        /// Focal length of frustum (for stereo rendering, defaults to 1.0)
        ///</summary>
        protected float _focalLength;

        /// <summary>
        ///     The 6 main clipping planes.
        /// </summary>
        protected Plane[] _planes = new Plane[6];

        /// <summary>
        ///     Stored versions of parent orientation.
        /// </summary>
        protected Quaternion _lastParentOrientation;

        /// <summary>
        ///     Stored versions of parent position.
        /// </summary>
        protected Vector3 _lastParentPosition;

        #region ProjectionMatrixRS Property

        /// <summary>
        /// Gets the projection matrix for this frustum adjusted for the current
        /// rendersystem specifics (may be right or left-handed, depth range
        /// may vary).
        /// </summary>
        /// <remarks>
        /// This method retrieves the rendering-API dependent version of the projection
        /// matrix. If you want a 'typical' projection matrix then use _projectionMatrix.
        /// </remarks>
        protected Matrix4 _projectionMatrixRS;

        /// <summary>
        /// Gets the projection matrix for this frustum adjusted for the current
        /// rendersystem specifics (may be right or left-handed, depth range
        /// may vary).
        /// </summary>
        /// <remarks>
        /// This method retrieves the rendering-API dependent version of the projection
        /// matrix. If you want a 'typical' projection matrix then use ProjectionMatrix.
        /// </remarks>
        virtual public Matrix4 ProjectionMatrixRS
        {
            get
            {
                UpdateFrustum();

                return _projectionMatrixRS;
            }
        }

        #endregion ProjectionMatrixRS Property

        #region ProjectionMatrixRSDepth Property

        /// <summary>
        ///  The depth-adjusted projection matrix for the current rendersystem,
        ///  but one which still conforms to right-hand rules.
        /// </summary>
        /// <remarks>
        ///     This differs from the rendering-API dependent getProjectionMatrix
        ///     in that it always returns a right-handed projection matrix result
        ///     no matter what rendering API is being used - this is required for
        ///     vertex and fragment programs for example. However, the resulting depth
        ///     range may still vary between render systems since D3D uses [0,1] and
        ///     GL uses [-1,1], and the range must be kept the same between programmable
        ///     and fixed-function pipelines.
        /// </remarks>
        protected Matrix4 _projectionMatrixRSDepth;

        /// <summary>
        ///    Gets the 'standard' projection matrix for this camera, ie the
        ///    projection matrix which conforms to standard right-handed rules.
        /// </summary>
        /// <remarks>
        ///    This differs from the rendering-API dependent ProjectionMatrix
        ///    in that it always returns a right-handed projection matrix result
        ///    no matter what rendering API is being used - this is required for
        ///    vertex and fragment programs for example. However, the resulting depth
        ///    range may still vary between render systems since D3D uses [0,1] and
        ///    GL uses [-1,1], and the range must be kept the same between programmable
        ///    and fixed-function pipelines.
        ///    <para/>
        ///    This corresponds to the Ogre mProjMatrixRSDepth and
        ///    getProjectionMatrixWithRSDepth
        /// </remarks>
        virtual public Matrix4 ProjectionMatrixRSDepth
        {
            get
            {
                UpdateFrustum();

                return _projectionMatrixRSDepth;
            }
        }

        #endregion ProjectionMatrixRSDepth Property

        #region ProjectionMatrix Property

        /// <summary>
        /// The normal projection matrix for this frustum, ie the
        /// projection matrix which conforms to standard right-handed rules and
        /// uses depth range [-1,+1].
        /// </summary>
        ///<remarks>
        ///    This differs from the rendering-API dependent getProjectionMatrixRS
        ///    in that it always returns a right-handed projection matrix with depth
        ///    range [-1,+1], result no matter what rendering API is being used - this
        ///    is required for some uniform algebra for example.
        ///</remarks>
        protected Matrix4 _projectionMatrix;

        /// <summary>
        /// Gets the projection matrix for this frustum.
        /// </summary>
        virtual public Matrix4 ProjectionMatrix
        {
            get
            {
                UpdateFrustum();

                return _projectionMatrix;
            }
            protected set
            {
                {
                    _projectionMatrix = value;
                }
            }
        }

        #endregion ProjectionMatrix Property

        #region ViewMatrix Property

        /// <summary>
        ///     Pre-calced view matrix.
        /// </summary>
        protected Matrix4 _viewMatrix;

        /// <summary>
        ///     Gets the view matrix for this frustum.
        /// </summary>
        virtual public Matrix4 ViewMatrix
        {
            get
            {
                UpdateView();

                return _viewMatrix;
            }
            set { _viewMatrix = value; }
        }

        #endregion ViewMatrix Property

        protected bool _recalculateFrustumPlanes;

        public Plane[] FrustumPlanes
        {
            get
            {
                UpdateFrustumPlanes();
                return _planes;
            }
        }

        /// <summary>
        ///     Something's changed in the frustum shape?
        /// </summary>
        protected bool _recalculateFrustum;

        /// <summary>
        ///		Evaluates whether or not the view frustum is out of date.
        /// </summary>
        virtual protected bool IsFrustumOutOfDate
        {
            get
            {
                if (useObliqueDepthProjection)
                {
                    // always out of date since plane needs to be in view space
                    if (IsViewOutOfDate)
                    {
                        _recalculateFrustum = true;
                    }

                }

                return _recalculateFrustum;
            }
        }

        /// <summary>
        ///     Something in the view pos has changed?
        /// </summary>
        protected bool _recalculateView;

        /// <summary>
        ///		Gets whether or not the view matrix is out of date.
        /// </summary>
        virtual protected bool IsViewOutOfDate
        {
            get
            {
                return _recalculateView;
            }
        }

        ///<summary>
        /// Are we using a custom view matrix?
        /// </summary>
        private bool _customViewMatrix;

        public bool IsCustomViewMatrixEnabled { get { return _customViewMatrix; } }

        /// <summary>
        /// Are we using a custom projection matrix?
        /// </summary>
        private bool _customProjectionMatrix;

        public bool IsCustomProjectionMatrixEnabled { get { return _customProjectionMatrix; } }

        /// <summary>
        ///     Bounding box of this frustum.
        /// </summary>
        protected AxisAlignedBox3 _boundingBox = AxisAlignedBox3.Null;

        /// <summary>
        ///
        /// </summary>
        protected bool _recalculateVertexData = true;

        /// <summary>
        /// Signal to recalculate World Space Corners
        /// </summary>
        protected bool _recalculateWorldSpaceCorners;

        /// <summary>
        ///		Frustum corners in world space.
        /// </summary>
        protected Vector3[] _worldSpaceCorners = new Vector3[8];

        /** Temp coefficient values calculated from a frustum change,
            used when establishing the frustum planes when the view changes. */
        protected float[] _coeffL = new float[2];
        protected float[] _coeffR = new float[2];
        protected float[] _coeffB = new float[2];
        protected float[] _coeffT = new float[2];

        #region IsRefelcted Property

        /// <summary>
        ///		Is this frustum to act as a reflection of itself?
        /// </summary>
        private bool _isReflected;

        /// <summary>
        ///     Gets a flag that specifies whether this camera is being reflected or not.
        /// </summary>
        virtual public bool IsReflected { get { return _isReflected; } }

        #endregion IsRefelcted Property

        #region ReflectionMatrix Property

        /// <summary>
        ///		Derive reflection matrix.
        /// </summary>
        private Matrix4 _reflectionMatrix;

        /// <summary>
        ///     Returns the reflection matrix of the camera if appropriate.
        /// </summary>
        virtual public Matrix4 ReflectionMatrix { get { return _reflectionMatrix; } protected set { _reflectionMatrix = value; } }

        #endregion ReflectionMatrix Property

        #region ReflectionPlane Property

        /// <summary>
        ///		Fixed reflection.
        /// </summary>
        private Plane _reflectionPlane;

        /// <summary>
        ///     Returns the reflection plane of the camera if appropriate.
        /// </summary>
        virtual public Plane ReflectionPlane { get { return _reflectionPlane; } protected set { _reflectionPlane = value; } }

        #endregion ReflectionPlane Property

        /// <summary>
        ///		Is this frustum using an oblique depth projection?
        /// </summary>
        protected bool useObliqueDepthProjection;

        /// <summary>
        ///		Fixed oblique projection plane.
        /// </summary>
        protected Plane obliqueProjPlane;

        protected List<Vector4> customParams = new List<Vector4>();

        /* Frustum extents */
        protected float _left, _right, _top, _bottom;

        protected bool _frustumExtentsManuallySet;

        protected float _orthoHeight;

        /// <summary>
        ///
        /// </summary>
        public float OrthoWindowHeight
        {
            get { return _orthoHeight; }
            set
            {
                _orthoHeight = value;
                InvalidateFrustum();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public float OrthoWindowWidth
        {
            get { return _orthoHeight * _aspectRatio; }
            set
            {
                _orthoHeight = value / _aspectRatio;
                InvalidateFrustum();
            }
        }

        #endregion Fields and Properties

        #region Constructors

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public Frustum()
            : base()
        {
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < 6; i++)
            {
                _planes[i] = new Plane();
            }

            _fieldOfView = Utility.PI / 4.0f;
            _nearDistance = 100.0f;
            _farDistance = 100000.0f;
            _aspectRatio = 1.33333333333333f;
            _orthoHeight = 1000.0f;
            _frustumExtentsManuallySet = false;
            _recalculateFrustum = true;
            _recalculateView = true;

            // Init matrices
            _viewMatrix = Matrix4.Zero;
            _projectionMatrix = Matrix4.Zero;
            _projectionMatrixRS = Matrix4.Zero;

            _projectionType = ProjectionType.Perspective;

            _lastParentPosition = Vector3.Zero;
            _lastParentOrientation = Quaternion.Identity;


            _customProjectionMatrix = false;
            _customViewMatrix = false;

            _frustumOffset = new Vector2(0.0f, 0.0f);
            _focalLength = 1.0f;


            UpdateView();
        }

        #endregion Constructors

        #region Methods

        public void SetCustomViewMatrix(bool enable)
        {
            SetCustomViewMatrix(enable, Matrix4.Identity);
        }

        public void SetCustomViewMatrix(bool enable, Matrix4 viewMatrix)
        {
            _customViewMatrix = enable;
            if (enable)
            {
                Debug.Assert(viewMatrix.IsAffine);
                _viewMatrix = viewMatrix;
            }
            InvalidateView();
        }

        public void SetCustomProjectionMatrix(bool enable)
        {
            SetCustomProjectionMatrix(enable, Matrix4.Identity);
        }

        public void SetCustomProjectionMatrix(bool enable, Matrix4 projMatrix)
        {
            _customProjectionMatrix = enable;
            if (enable)
            {
                _projectionMatrix = projMatrix;
            }

            InvalidateFrustum();
        }

        /// <summary>
        ///     Disables reflection modification previously turned on with <see cref="EnableReflection"/>.
        /// </summary>
        virtual public void DisableReflection()
        {
            _isReflected = false;
            InvalidateView();
        }


        /// <summary>
        ///     Modifies this camera so it always renders from the reflection of itself through the
        ///     plane specified.
        /// </summary>
        /// <remarks>
        ///     This is obviously useful for rendering planar reflections.
        /// </remarks>
        /// <param name="plane"></param>
        virtual public void EnableReflection(Plane plane)
        {
            _isReflected = true;
            _reflectionPlane = plane;
            _reflectionMatrix = Utility.BuildReflectionMatrix(plane);
            InvalidateView();
        }

        /// <summary>
        ///		Get the derived position of this frustum.
        /// </summary>
        /// <returns></returns>
        virtual protected Vector3 GetPositionForViewUpdate()
        {
            return _lastParentPosition;
        }

        /// <summary>
        ///		Get the derived orientation of this frustum.
        /// </summary>
        /// <returns></returns>
        virtual protected Quaternion GetOrientationForViewUpdate()
        {
            return _lastParentOrientation;
        }

        /// <summary>
        ///		Overloaded method.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool IsObjectVisible(AxisAlignedBox3 box)
        {
            // this overload doesnt care about the clipping plane, but we gotta
            // pass in something to the out param anyway
            FrustumPlane dummy;
            return IsObjectVisible(box, out dummy);
        }

        /// <summary>
        ///		Tests whether the given box is visible in the Frustum.
        ///	 </summary>
        /// <param name="box"> Bounding box to be checked.</param>
        /// <param name="culledBy">
        ///		Optional FrustrumPlane params which will be filled by the plane which culled
        ///		the box if the result was false.
        ///	</param>
        /// <returns>True if the box is visible, otherwise false.</returns>
        public bool IsObjectVisible(AxisAlignedBox3 box, out FrustumPlane culledBy)
        {
            culledBy = FrustumPlane.None;

            // Null boxes are always invisible
            if (box.IsNull)
            {
                return false;
            }

            // Infinite Boxes are always visible
            if (box.IsInfinite)
            {
                return true;
            }

            // Make any pending updates to the calculated frustum
            UpdateFrustumPlanes();

            // Get corners of the box
            Vector3[] corners = box.Corners;

            // For each plane, see if all points are on the negative side
            // If so, object is not visible
            for (int plane = 0; plane < 6; plane++)
            {
                // skip far plane if infinite view frustum
                if (_farDistance == 0 && plane == (int)FrustumPlane.Far)
                {
                    continue;
                }

                if (_planes[plane].GetSide(corners[0]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[1]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[2]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[3]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[4]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[5]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[6]) == PlaneSide.Negative &&
                    _planes[plane].GetSide(corners[7]) == PlaneSide.Negative)
                {
                    // ALL corners on negative side therefore out of view
                    culledBy = (FrustumPlane)plane;
                    return false;
                }
            }

            // box is not culled
            return true;
        }

        /// <summary>
        ///		Overloaded method.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool IsObjectVisible(Sphere sphere)
        {
            // this overload doesnt care about the clipping plane, but we gotta
            // pass in something to the out param anyway
            FrustumPlane dummy;
            return IsObjectVisible(sphere, out dummy);
        }

        /// <summary>
        ///		Tests whether the given sphere is in the viewing frustum.
        /// </summary>
        /// <param name="sphere">Bounding sphere to be checked.</param>
        /// <param name="culledBy">
        ///		Optional FrustrumPlane params which will be filled by the plane which culled
        ///		the box if the result was false.
        ///	</param>
        /// <returns>True if the box is visible, otherwise false.</returns>
        public bool IsObjectVisible(Sphere sphere, out FrustumPlane culledBy)
        {
            // Make any pending updates to the calculated frustum
            UpdateFrustumPlanes();

            // For each plane, see if sphere is on negative side
            // If so, object is not visible
            for (int plane = 0; plane < 6; plane++)
            {
                if (_farDistance == 0 && plane == (int)FrustumPlane.Far)
                {
                    continue;
                }

                // If the distance from sphere center to plane is negative, and 'more negative'
                // than the radius of the sphere, sphere is outside frustum
                if (_planes[plane].GetDistance(sphere.Center) < -sphere.Radius)
                {
                    // ALL corners on negative side therefore out of view
                    culledBy = (FrustumPlane)plane;
                    return false;
                }
            }

            // sphere is not culled
            culledBy = FrustumPlane.None;
            return true;
        }

        /// <summary>
        ///		Overloaded method.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool IsObjectVisible(Vector3 vertex)
        {
            // this overload doesnt care about the clipping plane, but we gotta
            // pass in something to the out param anyway
            FrustumPlane dummy;
            return IsObjectVisible(vertex, out dummy);
        }

        /// <summary>
        ///		Tests whether the given 3D point is in the viewing frustum.
        /// </summary>
        /// <param name="vector">3D point to check for frustum visibility.</param>
        /// <param name="culledBy">
        ///		Optional FrustrumPlane params which will be filled by the plane which culled
        ///		the box if the result was false.
        ///	</param>
        /// <returns>True if the box is visible, otherwise false.</returns>
        public bool IsObjectVisible(Vector3 vertex, out FrustumPlane culledBy)
        {
            // Make any pending updates to the calculated frustum
            UpdateFrustumPlanes();

            // For each plane, see if all points are on the negative side
            // If so, object is not visible
            for (int plane = 0; plane < 6; plane++)
            {
                if (_farDistance == 0 && plane == (int)FrustumPlane.Far)
                {
                    continue;
                }

                if (_planes[plane].GetSide(vertex) == PlaneSide.Negative)
                {
                    // ALL corners on negative side therefore out of view
                    culledBy = (FrustumPlane)plane;
                    return false;
                }
            }

            // vertex is not culled
            culledBy = FrustumPlane.None;
            return true;
        }

        virtual public bool ProjectSphere(Sphere sphere, out float left, out float top, out float right, out float bottom)
        {
            // initialise
            left = bottom = -1.0f;
            right = top = 1.0f;

            // Transform light position into camera space
            Vector3 eyeSpacePos = this.ViewMatrix.TransformAffine(sphere.Center);

            if (eyeSpacePos.Z < 0)
            {
                float r = sphere.Radius;
                // early-exit
                if (eyeSpacePos.LengthSquared() <= r * r)
                {
                    return false;
                }

                Vector3 screenSpacePos = this.ProjectionMatrix * eyeSpacePos;

                // perspective attenuate
                Vector3 spheresize = new Vector3(r, r, eyeSpacePos.Z);
                spheresize = this.ProjectionMatrixRSDepth * spheresize;

                float possLeft = screenSpacePos.X - spheresize.X;
                float possRight = screenSpacePos.X + spheresize.X;
                float possTop = screenSpacePos.Y + spheresize.Y;
                float possBottom = screenSpacePos.Y - spheresize.Y;

                left = Utility.Max(-1.0f, possLeft);
                right = Utility.Min(1.0f, possRight);
                top = Utility.Min(1.0f, possTop);
                bottom = Utility.Max(-1.0f, possBottom);
            }

            return (left != -1.0f) || (top != 1.0f) || (right != 1.0f) || (bottom != -1.0f);
        }

        /// <summary>
        ///     Signal to update frustum information.
        /// </summary>
        virtual protected void InvalidateFrustum()
        {
            _recalculateFrustum = true;
            _recalculateFrustumPlanes = true;
            _recalculateWorldSpaceCorners = true;
            _recalculateVertexData = true;
        }

        /// <summary>
        ///     Signal to update view information.
        /// </summary>
        virtual protected void InvalidateView()
        {
            _recalculateView = true;
            _recalculateFrustumPlanes = true;
            _recalculateWorldSpaceCorners = true;
        }

        protected void CalculateProjectionParameters(out float vpLeft, out float vpRight, out float vpBottom, out float vpTop)
        {
            if (_customProjectionMatrix)
            {
                // Convert clipspace corners to camera space
                Matrix4 invProj = _projectionMatrix.Inverse();
                Vector3 topLeft = new Vector3(-0.5f, 0.5f, 0.0f);
                Vector3 bottomRight = new Vector3(0.5f, -0.5f, 0.0f);

                topLeft = invProj * topLeft;
                bottomRight = invProj * bottomRight;

                vpLeft = topLeft.X;
                vpTop = topLeft.Y;
                vpRight = bottomRight.X;
                vpBottom = bottomRight.Y;
            }
            else if (_frustumExtentsManuallySet)
            {
                vpLeft = _left;
                vpRight = _right;
                vpTop = _top;
                vpBottom = _bottom;
            }
            // Calculate general projection parameters
            else if (ProjectionType == ProjectionType.Perspective)
            {
                // Calculate general projection parameters

                float thetaY = _fieldOfView * 0.5f;
                float tanThetaY = Utility.Tan(thetaY);
                float tanThetaX = tanThetaY * _aspectRatio;

                // Unknown how to apply frustum offset to orthographic camera, just ignore here
                float nearFocal = _nearDistance / _focalLength;
                float nearOffsetX = _frustumOffset.X * nearFocal;
                float nearOffsetY = _frustumOffset.Y * nearFocal;
                float half_w = tanThetaX * _nearDistance;
                float half_h = tanThetaY * _nearDistance;

                vpLeft = -half_w + nearOffsetX;
                vpRight = +half_w + nearOffsetX;
                vpBottom = -half_h + nearOffsetY;
                vpTop = +half_h + nearOffsetY;

                _left = vpLeft;
                _right = vpRight;
                _top = vpTop;
                _bottom = vpBottom;
            }
            else //if (ProjectionType == Projection.Orthographic)
            {
                // Unknown how to apply frustum offset to orthographic camera, just ignore here
                float half_w = OrthoWindowWidth * 0.5f;
                float half_h = OrthoWindowHeight * 0.5f;

                vpLeft = -half_w;
                vpRight = +half_w;
                vpBottom = -half_h;
                vpTop = +half_h;

                _left = vpLeft;
                _right = vpRight;
                _top = vpTop;
                _bottom = vpBottom;
            }
        }

        /// <summary>
        ///		Updates the frustum data.
        /// </summary>
        protected void UpdateFrustum()
        {
            if (IsFrustumOutOfDate)
            {
                _updateFrustum();
            }
        }

        virtual protected void _updateFrustum()
        {
            float vpTop, vpRight, vpBottom, vpLeft;

            CalculateProjectionParameters(out vpLeft, out vpRight, out vpBottom, out vpTop);

            if (!_customProjectionMatrix)
            {
                // The code below will dealing with general projection
                // parameters, similar glFrustum and glOrtho.
                // Doesn't optimise manually except division operator, so the
                // code more self-explaining.

                float inv_w = 1.0f / (vpRight - vpLeft);
                float inv_h = 1.0f / (vpTop - vpBottom);
                float inv_d = 1.0f / (_farDistance - _nearDistance);

                // Recalc if frustum params changed
                if (_projectionType == ProjectionType.Perspective)
                {
                    // Calc matrix elements
                    float A = 2.0f * _nearDistance * inv_w;
                    float B = 2.0f * _nearDistance * inv_h;
                    float C = (vpRight + vpLeft) * inv_w;
                    float D = (vpTop + vpBottom) * inv_h;
                    float q, qn;
                    if (_farDistance == 0.0f)
                    {
                        // Infinite far plane
                        q = Frustum.InfiniteFarPlaneAdjust - 1.0f;
                        qn = _nearDistance * (Frustum.InfiniteFarPlaneAdjust - 2.0f);
                    }
                    else
                    {
                        q = -(_farDistance + _nearDistance) * inv_d;
                        qn = -2.0f * (_farDistance * _nearDistance) * inv_d;
                    }

                    // NB: This creates 'uniform' perspective projection matrix,
                    // which depth range [-1,1], right-handed rules
                    //
                    // [ A   0   C   0  ]
                    // [ 0   B   D   0  ]
                    // [ 0   0   q   qn ]
                    // [ 0   0   -1  0  ]
                    //
                    // A = 2 * near / (right - left)
                    // B = 2 * near / (top - bottom)
                    // C = (right + left) / (right - left)
                    // D = (top + bottom) / (top - bottom)
                    // q = - (far + near) / (far - near)
                    // qn = - 2 * (far * near) / (far - near)

                    _projectionMatrix = Matrix4.Zero;
                    _projectionMatrix.m00 = A;
                    _projectionMatrix.m02 = C;
                    _projectionMatrix.m11 = B;
                    _projectionMatrix.m12 = D;
                    _projectionMatrix.m22 = q;
                    _projectionMatrix.m23 = qn;
                    _projectionMatrix.m32 = -1.0f;

                    if (useObliqueDepthProjection)
                    {
                        // Translate the plane into view space

                        // Don't use getViewMatrix here, incase overrided by
                        // camera and return a cull frustum view matrix
                        UpdateView();

                        Plane plane = _viewMatrix * obliqueProjPlane;

                        // Thanks to Eric Lenyel for posting this calculation
                        // at www.terathon.com

                        // Calculate the clip-space corner point opposite the
                        // clipping plane
                        // as (sgn(clipPlane.x), sgn(clipPlane.y), 1, 1) and
                        // transform it into camera space by multiplying it
                        // by the inverse of the projection matrix

                        /* generalised version
                        Vector4 q = matrix.inverse() *
                        Vector4(Math::Sign(plane.normal.x),
                        Math::Sign(plane.normal.y), 1.0f, 1.0f);
                         */
                        Vector4 q1 = new Vector4();
                        q1.X = (System.Math.Sign(plane.Normal.X) + _projectionMatrix.m02) / _projectionMatrix.m00;
                        q1.Y = (System.Math.Sign(plane.Normal.Y) + _projectionMatrix.m12) / _projectionMatrix.m11;
                        q1.Z = -1.0f;
                        q1.W = (1.0f + _projectionMatrix.m22) / _projectionMatrix.m23;

                        // Calculate the scaled plane vector
                        Vector4 clipPlane4d = new Vector4(plane.Normal.X, plane.Normal.Y, plane.Normal.Z, plane.D);
                        Vector4 c = clipPlane4d * (2.0f / (clipPlane4d.Dot(q1)));

                        // Replace the third row of the projection matrix
                        _projectionMatrix.m20 = c.X;
                        _projectionMatrix.m21 = c.Y;
                        _projectionMatrix.m22 = c.Z + 1.0f;
                        _projectionMatrix.m23 = c.W;
                    }
                } // perspective
                else if (_projectionType == ProjectionType.Orthographic)
                {
                    float A = 2.0f * inv_w;
                    float B = 2.0f * inv_h;
                    float C = -(vpRight + vpLeft) * inv_w;
                    float D = -(vpTop + vpBottom) * inv_h;
                    float q, qn;
                    if (_farDistance == 0.0f)
                    {
                        // Can not do infinite far plane here, avoid divided zero only
                        q = -Frustum.InfiniteFarPlaneAdjust / _nearDistance;
                        qn = -Frustum.InfiniteFarPlaneAdjust - 1.0f;
                    }
                    else
                    {
                        q = -2.0f * inv_d;
                        qn = -(_farDistance + _nearDistance) * inv_d;
                    }

                    // NB: This creates 'uniform' orthographic projection matrix,
                    // which depth range [-1,1], right-handed rules
                    //
                    // [ A   0   0   C  ]
                    // [ 0   B   0   D  ]
                    // [ 0   0   q   qn ]
                    // [ 0   0   0   1  ]
                    //
                    // A = 2 * / (right - left)
                    // B = 2 * / (top - bottom)
                    // C = - (right + left) / (right - left)
                    // D = - (top + bottom) / (top - bottom)
                    // q = - 2 / (far - near)
                    // qn = - (far + near) / (far - near)

                    _projectionMatrix = Matrix4.Zero;
                    _projectionMatrix.m00 = A;
                    _projectionMatrix.m03 = C;
                    _projectionMatrix.m11 = B;
                    _projectionMatrix.m13 = D;
                    _projectionMatrix.m22 = q;
                    _projectionMatrix.m23 = qn;
                    _projectionMatrix.m33 = 1.0f;
                } // ortho
            } // if !_customProjectionMatrix

            // grab a reference to the current render system
            //FIX
            //RenderSystem renderSystem = Root.Instance.RenderSystem;
            // API specific
            //_projectionMatrixRS = renderSystem.ConvertProjectionMatrix(_projectionMatrix);
            // API specific for Gpu Programs
            //_projectionMatrixRSDepth = renderSystem.ConvertProjectionMatrix(_projectionMatrix, true);
            _projectionMatrixRS = _projectionMatrix;
            _projectionMatrixRSDepth = _projectionMatrix;
            // Calculate bounding box (local)
            // Box is from 0, down -Z, max dimensions as determined from far plane
            // If infinite view frustum just pick a far value
            float farDist = (_farDistance == 0.0f) ? 100000.0f : _farDistance;

            // Near plane bounds
            Vector3 min = new Vector3(vpLeft, vpBottom, -farDist);
            Vector3 max = new Vector3(vpRight, vpTop, 0);

            if (_customProjectionMatrix)
            {
                // Some custom projection matrices can have unusual inverted settings
                // So make sure the AABB is the right way around to start with
                Vector3 tmp = min;
                min = min.Floor2(max);
                max = max.Ceil2(tmp);
            }

            float radio = 1.0f;
            if (_projectionType == ProjectionType.Perspective)
            {
                // Merge with far plane bounds
                radio = _farDistance / _nearDistance;
                min = min.Floor2(new Vector3(vpLeft * radio, vpBottom * radio, -_farDistance));
                max = max.Ceil2(new Vector3(vpRight * radio, vpTop * radio, 0));
            }

            _boundingBox.SetExtents(min, max);

            _recalculateFrustum = false;

            // Signal to update frustum clipping planes
            _recalculateFrustumPlanes = true;
        }

        protected void UpdateFrustumPlanes()
        {
            UpdateView();
            UpdateFrustum();

            if (_recalculateFrustumPlanes)
            {
                _updateFrustumPlanes();
            }
        }

        virtual protected void _updateFrustumPlanes()
        {
            // -------------------------
            // Update the frustum planes
            // -------------------------
            Matrix4 combo = _projectionMatrix * _viewMatrix;

            _planes[(int)FrustumPlane.Left].Normal.X = combo[3, 0] + combo[0, 0];
            _planes[(int)FrustumPlane.Left].Normal.Y = combo[3, 1] + combo[0, 1];
            _planes[(int)FrustumPlane.Left].Normal.Z = combo[3, 2] + combo[0, 2];
            _planes[(int)FrustumPlane.Left].D = combo[3, 3] + combo[0, 3];

            _planes[(int)FrustumPlane.Right].Normal.X = combo[3, 0] - combo[0, 0];
            _planes[(int)FrustumPlane.Right].Normal.Y = combo[3, 1] - combo[0, 1];
            _planes[(int)FrustumPlane.Right].Normal.Z = combo[3, 2] - combo[0, 2];
            _planes[(int)FrustumPlane.Right].D = combo[3, 3] - combo[0, 3];

            _planes[(int)FrustumPlane.Top].Normal.X = combo[3, 0] - combo[1, 0];
            _planes[(int)FrustumPlane.Top].Normal.Y = combo[3, 1] - combo[1, 1];
            _planes[(int)FrustumPlane.Top].Normal.Z = combo[3, 2] - combo[1, 2];
            _planes[(int)FrustumPlane.Top].D = combo[3, 3] - combo[1, 3];

            _planes[(int)FrustumPlane.Bottom].Normal.X = combo[3, 0] + combo[1, 0];
            _planes[(int)FrustumPlane.Bottom].Normal.Y = combo[3, 1] + combo[1, 1];
            _planes[(int)FrustumPlane.Bottom].Normal.Z = combo[3, 2] + combo[1, 2];
            _planes[(int)FrustumPlane.Bottom].D = combo[3, 3] + combo[1, 3];

            _planes[(int)FrustumPlane.Near].Normal.X = combo[3, 0] + combo[2, 0];
            _planes[(int)FrustumPlane.Near].Normal.Y = combo[3, 1] + combo[2, 1];
            _planes[(int)FrustumPlane.Near].Normal.Z = combo[3, 2] + combo[2, 2];
            _planes[(int)FrustumPlane.Near].D = combo[3, 3] + combo[2, 3];

            _planes[(int)FrustumPlane.Far].Normal.X = combo[3, 0] - combo[2, 0];
            _planes[(int)FrustumPlane.Far].Normal.Y = combo[3, 1] - combo[2, 1];
            _planes[(int)FrustumPlane.Far].Normal.Z = combo[3, 2] - combo[2, 2];
            _planes[(int)FrustumPlane.Far].D = combo[3, 3] - combo[2, 3];

            // Renormalise any normals which were not unit length
            for (int i = 0; i < 6; i++)
            {
                //float length = _planes[i].Normal.Normalize();
                float length = _planes[i].Normal.Length();
                _planes[i].Normal.Normalize();
                _planes[i].D /= length;
            }

            _recalculateFrustumPlanes = false;
        }

        /// <summary>
        ///		Updates the view matrix.
        /// </summary>
        protected void UpdateView()
        {
            // check if the view is out of date
            if (IsViewOutOfDate)
            {
                _updateView();
            }
        }

        virtual protected void _updateView()
        {
            // ----------------------
            // Update the view matrix
            // ----------------------

            // View matrix is:
            //
            //  [ Lx  Uy  Dz  Tx  ]
            //  [ Lx  Uy  Dz  Ty  ]
            //  [ Lx  Uy  Dz  Tz  ]
            //  [ 0   0   0   1   ]
            //
            // Where T = -(Transposed(Rot) * Pos)

            // This is most efficiently done using 3x3 Matrices

            // Get orientation from quaternion

            if (!_customViewMatrix)
            {
                Quaternion orientation = GetOrientationForViewUpdate();
                Vector3 position = GetPositionForViewUpdate();
                Matrix3 rotation = orientation.ToRotationMatrix();

                // Make the translation relative to new axes
                Matrix3 rotT = rotation.Transpose();
                Vector3 trans = -rotT * position;

                // Make final matrix
                _viewMatrix = Matrix4.Identity;
                _viewMatrix = rotT; // fills upper 3x3
                _viewMatrix[0, 3] = trans.X;
                _viewMatrix[1, 3] = trans.Y;
                _viewMatrix[2, 3] = trans.Z;

                // Deal with reflections
                if (_isReflected)
                {
                    _viewMatrix = _viewMatrix * _reflectionMatrix;
                }
            }

            _recalculateView = false;

            // Signal to update frustum clipping planes
            _recalculateFrustumPlanes = true;
            // Signal to update world space corners
            _recalculateWorldSpaceCorners = true;
            // Signal to update frustum if oblique plane enabled,
            // since plane needs to be in view space
            if (useObliqueDepthProjection)
            {
                _recalculateFrustum = true;
            }
        }

        protected void UpdateWorldSpaceCorners()
        {
            UpdateView();

            if (_recalculateWorldSpaceCorners)
            {
                _updateWorldSpaceCorners();
            }
        }

        virtual protected void _updateWorldSpaceCorners()
        {
            Matrix4 eyeToWorld = _viewMatrix.InverseAffine();

            // Note: Even though we could be dealing with general a projection matrix here,
            //       it is incompatible with the infinite far plane, thus, we need to work
            //		 with projection parameters.

            // Calc near plane corners
            float nearLeft, nearRight, nearBottom, nearTop;
            CalculateProjectionParameters(out nearLeft, out nearRight, out nearBottom, out nearTop);

            // Treat infinite fardist as some arbitrary far value
            float farDist = (_farDistance == 0) ? 100000 : _farDistance;

            // Calc far palne corners
            float ratio = _projectionType == ProjectionType.Perspective ? _farDistance / _nearDistance : 1;
            float farLeft = nearLeft * ratio;
            float farRight = nearRight * ratio;
            float farBottom = nearBottom * ratio;
            float farTop = nearTop * ratio;

            // near
            _worldSpaceCorners[0] = eyeToWorld.TransformAffine(new Vector3(nearRight, nearTop, -_nearDistance));
            _worldSpaceCorners[1] = eyeToWorld.TransformAffine(new Vector3(nearLeft, nearTop, -_nearDistance));
            _worldSpaceCorners[2] = eyeToWorld.TransformAffine(new Vector3(nearLeft, nearBottom, -_nearDistance));
            _worldSpaceCorners[3] = eyeToWorld.TransformAffine(new Vector3(nearRight, nearBottom, -_nearDistance));
            // far
            _worldSpaceCorners[4] = eyeToWorld.TransformAffine(new Vector3(farRight, farTop, -_farDistance));
            _worldSpaceCorners[5] = eyeToWorld.TransformAffine(new Vector3(farLeft, farTop, -_farDistance));
            _worldSpaceCorners[6] = eyeToWorld.TransformAffine(new Vector3(farLeft, farBottom, -_farDistance));
            _worldSpaceCorners[7] = eyeToWorld.TransformAffine(new Vector3(farRight, farBottom, -_farDistance));

            _recalculateWorldSpaceCorners = false;
        }

        /// <summary>
        /// Set view area size for orthographic mode.
        /// </summary>
        /// <remarks>
        /// Note that this method adjusts the frustum's aspect ratio.
        /// </remarks>
        /// <param name="w">Width of the area to be visible</param>
        /// <param name="h">Height of the area to be visible</param>
        public void SetOrthoWindow(float w, float h)
        {
            _orthoHeight = h;
            _aspectRatio = w / h;
            InvalidateFrustum();
        }

        public void SetFrustumExtents(float left, float right, float top, float bottom)
        {
            _frustumExtentsManuallySet = true;
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;

            InvalidateFrustum();
        }

        public void ResetFrustumExtents()
        {
            _frustumExtentsManuallySet = false;
            InvalidateFrustum();
        }

        public void GetFrustumExtents(out float left, out float right, out float top, out float bottom)
        {
            UpdateFrustum();
            left = _left;
            right = _right;
            top = _top;
            bottom = _bottom;
        }

        #endregion Methods

        #region Overloaded operators

        /// <summary>
        ///		An indexer that accepts a FrustumPlane enum value and return the appropriate plane side of the Frustum.
        /// </summary>
        public Plane this[FrustumPlane plane]
        {
            get
            {
                // make any pending updates to the calculated frustum
                UpdateFrustumPlanes();

                // convert the incoming plan enum type to a int
                int index = (int)plane;

                // access the planes array by index
                return _planes[index];
            }
        }

        #endregion Overloaded operators

    }
}
