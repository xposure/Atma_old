using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA = Microsoft.Xna.Framework;
using XFG = Microsoft.Xna.Framework.Graphics;

namespace Atma.MonoGame
{
    /// <summary>
    /// Summary description for VideoMode.
    /// </summary>
    public class VideoMode
    {
        #region Member variables

        private XFG.DisplayMode displayMode;
        private int modeNum;
        private static int modeCount = 0;

        #endregion

        #region Constructors

        /// <summary>
        ///		Default constructor.
        /// </summary>
        public VideoMode()
        {
            modeNum = ++modeCount;
            displayMode = new XFG.DisplayMode();
        }

        /// <summary>
        ///		Accepts a existing XNAVideoMode object.
        /// </summary>
        public VideoMode(VideoMode videoMode)
        {
            modeNum = ++modeCount;
            displayMode = videoMode.displayMode;
        }

        /// <summary>
        ///		Accepts a existing Direct3D.DisplayMode object.
        /// </summary>
        public VideoMode(XFG.DisplayMode videoMode)
        {
            modeNum = ++modeCount;
            displayMode = videoMode;
        }

        /// <summary>
        ///		Destructor.
        /// </summary>
        ~VideoMode()
        {
            modeCount--;
        }

        #endregion

        #region Properties

        /// <summary>
        ///		Width of this video mode.
        /// </summary>
        public int Width { get { return displayMode.Width; } }

        /// <summary>
        ///		Height of this video mode.
        /// </summary>
        public int Height { get { return displayMode.Height; } }

        /// <summary>
        ///		Format of this video mode.
        /// </summary>
        public XFG.SurfaceFormat Format { get { return displayMode.Format; } }

        /// <summary>
        ///		Refresh rate of this video mode.
        /// </summary>
        public int RefreshRate { get { return displayMode.RefreshRate; } }

        /// <summary>
        ///		Color depth of this video mode.
        /// </summary>
        public int ColorDepth
        {
            get
            {
                if (displayMode.Format == XFG.SurfaceFormat.Bgr32 ||
                    displayMode.Format == XFG.SurfaceFormat.Color
                {
                    return 32;
                }
                else
                {
                    return 16;
                }
            }
        }

        /// <summary>
        ///		Gets the XNA.DisplayMode object associated with this video mode.
        /// </summary>
        public XFG.DisplayMode DisplayMode { get { return displayMode; } }

        /// <summary>
        ///		Returns a string representation of this video mode.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} x {1} @ {2}-bit color", displayMode.Width, displayMode.Height, this.ColorDepth);
        }

        #endregion
    }

    /// <summary>
    /// Summary description for VideoModeCollection.
    /// </summary>
    public class VideoModeCollection : List<VideoMode>
    {
        public VideoMode this[string description]
        {
            get
            {
                foreach (VideoMode mode in this)
                {
                    if (mode.ToString() == description)
                    {
                        return mode;
                    }
                }
                return null;
            }
        }
    }
}
