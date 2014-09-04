using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNA = Microsoft.Xna.Framework;
using XFG = Microsoft.Xna.Framework.Graphics;

namespace Atma.MonoGame
{
    public class Driver
    {
        #region Constructors

        /// <summary>
        ///		Default constructor.
        /// </summary>
        public Driver(XFG.GraphicsAdapter adapterDetails)
        {
            this._desktopMode = adapterDetails.CurrentDisplayMode;
            //this._name = adapterDetails.DeviceName;
            //this._description = adapterDetails.Description;
            //this._adapterNum = adapterDetails.DeviceId;
            //this._adapterIdentifier = adapterDetails.DeviceIdentifier;
            this._adapter = adapterDetails;
            
            _videoModeList = new VideoModeCollection();
        }

        #endregion Constructors

        #region Properties

        #region Name Property

        private string _name;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return _name; } }

        #endregion Name Property

        #region Description Property

        private string _description;

        /// <summary>
        /// 
        /// </summary>
        public string Description { get { return _description; } }

        #endregion Description Property

        #region Adapter Property

        private XFG.GraphicsAdapter _adapter;

        /// <summary>
        /// 
        /// </summary>
        public XFG.GraphicsAdapter Adapter { get { return _adapter; } }

        #endregion AdapterNumber Property

        #region AdapterNumber Property

        private int _adapterNum;

        /// <summary>
        /// 
        /// </summary>
        public int AdapterNumber { get { return _adapterNum; } }

        #endregion AdapterNumber Property

        #region AdapterIdentifier Property

        private Guid _adapterIdentifier;

        /// <summary>
        /// 
        /// </summary>
        public Guid AdapterIdentifier { get { return _adapterIdentifier; } }

        #endregion AdapterIdentifier Property

        #region DesktopMode Property

        private XFG.DisplayMode _desktopMode;

        /// <summary>
        ///		
        /// </summary>
        public XFG.DisplayMode DesktopMode { get { return _desktopMode; } }

        #endregion DesktopMode Property

        #region VideoModes Property

        private VideoModeCollection _videoModeList;

        /// <summary>
        ///		
        /// </summary>
        public VideoModeCollection VideoModes { get { return _videoModeList; } }

        #endregion VideoModes Property

        #region XnaDevice Property

        private XFG.GraphicsDevice _device;

        /// <summary>
        ///		
        /// </summary>
        public XFG.GraphicsDevice XnaDevice { get { return _device; } set { _device = value; } }

        #endregion XnaDevice Property

        #endregion Properties
    }
}
