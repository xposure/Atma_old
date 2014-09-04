using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Graphics
{
    public abstract class RenderWindow : DisposableObject, IDisposable
    {
        protected bool _isFullscreen = false;

        //public abstract bool isClose { get; }
        //public abstract bool isActive { get; }
        //public abstract bool isVisible { get; }

        public abstract void resize(int width, int height);

        public abstract void create(string name, int width, int height, bool fullScreen);

        public virtual void WindowMovedOrResized()
        {
            
        }

        public abstract void update();

        public abstract void prepareToRender();

        public virtual bool IsActive { get; set; }

        #region Height Property

        /// <summary>
        ///    Height of this render target.
        /// </summary>
        private int _height;

        /// <summary>
        /// Gets/Sets the height of this render target.
        /// </summary>
        virtual public int Height { get { return _height; } protected set { _height = value; } }

        #endregion Height Property

        #region Width Property

        /// <summary>
        ///    Width of this render target.
        /// </summary>
        private int _width;

        /// <summary>
        /// Gets/Sets the width of this render target.
        /// </summary>
        virtual public int Width { get { return _width; } protected set { _width = value; } }

        #endregion Width Property



    }    
}
