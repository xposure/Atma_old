//using Atma.Assets;
//using Atma.Core;
//using Atma.Engine;
//using Atma.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Atma.Systems
//{
//    public class AbstractSystem
//    {
//        private GraphicSubsystem _graphics;
//        public GraphicSubsystem graphics
//        {
//            get
//            {
//                if (_graphics == null)
//                    _graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
                
//                return _graphics;
//            }
//        }

//        private DisplayDevice _display;
//        public DisplayDevice display
//        {
//            get
//            {
//                if (_display == null)
//                    _display = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri);

//                return _display;
//            }
//        }

//        private TimeBase _time;
//        public TimeBase time
//        {
//            get
//            {
//                if (_time == null)
//                    _time = CoreRegistry.require<TimeBase>(TimeBase.Uri);

//                return _time;
//            }
//        }

//        private ComponentSystemManager _components;
//        public ComponentSystemManager components
//        {
//            get
//            {
//                if (_components == null)
//                    _components = CoreRegistry.require<ComponentSystemManager>(ComponentSystemManager.Uri);

//                return _components;
//            }
//        }
        
//        private AssetManager _assets;
//        public AssetManager assets
//        {
//            get
//            {
//                if (_assets == null)
//                    _assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

//                return _assets;
//            }
//        }



//    }
//}
