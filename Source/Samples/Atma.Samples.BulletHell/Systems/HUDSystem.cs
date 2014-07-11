using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Managers;
using Atma.Systems;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems
{
    public class HUDSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:hud";

        private int _lives = 3;
        private int _score = 0;
        private int _multiplier = 1;

        public void update(float delta)
        {
           
        }

        public void init()
        {
            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);
            gui.onRender += gui_onRender;

            _lives = 3;
            _score = 0;
            _multiplier = 1;
        }

        void gui_onRender(GUIManager gui)
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            var screen = CoreRegistry.require<Atma.Graphics.DisplayDevice>(Atma.Graphics.DisplayDevice.Uri);
            gui.label(new Vector2(300, 0), string.Format("lives: {0}", _lives));
            gui.label(new Vector2(500, 0), string.Format("score: {0}", _score));
            gui.label(new Vector2(700, 0), string.Format("multi: {0}", _multiplier));

            //if (waitingForRespawn)
            //{
            //    if (_lives < 0)
            //        gui.label(new Vector2(400, 400), 2, string.Format("game over! right click to start over!"));
            //    else
            //        gui.label(new Vector2(400, 400), 2, string.Format("right click to respawn!"));
            //}
        }

        public void shutdown()
        {
        }
    }
}
