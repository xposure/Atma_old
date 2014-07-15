using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Managers;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Atma.Graphics;

namespace Atma.Samples.BulletHell.Systems
{
    public class DebugSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:debug";

        private int samples = 400;
        private int updateCount = 0;
        private Queue<float> _fps = new Queue<float>();

        public void init()
        {
            //CoreRegistry.require<SpriteRenderer>(SpriteRenderer.Uri).onAfterRender += DebugSystem_onAfterRender;
            CoreRegistry.require<GUIManager>(GUIManager.Uri).onRender += DebugSystem_onRender;
        }

        void DebugSystem_onRender(GUIManager obj)
        {
            
            //var fps = _fps.ToArray();
            var activity = "test render";

            var metricData = PerformanceMonitor.getMetricData();

            var index = 0;
            var fps = new float[metricData.Count];
            foreach (var sample in metricData)
            {
                fps[index++] = sample.get(activity);
            }

            if (fps.Length == 0)
                return;

            //fps = _fps.ToArray();

            var lineheight = 100f;
            var halfheight = lineheight / 2f;
            var max = fps[0];
            var min = fps[0];
            var avg = fps[0];
            for (var i = 1; i < fps.Length; i++)
            {
                max = Math.Max(fps[i], max);
                min = Math.Min(fps[i], min);
                avg = avg * 0.95f + fps[i] * 0.05f;
            }

            //avg /= max;
            //avg *= 0.8f;


            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);
            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            var screen = CoreRegistry.require<Atma.Graphics.DisplayDevice>(Atma.Graphics.DisplayDevice.Uri);
            //gui.label(new Vector2(400,400), ((int)(avg * 1000)).ToString() + "ms " + string.Format("{0}", updateCount));

            //gui.label(new Vector2(0, 20), Atma.MonoGame.Graphics.MonoGL.instance.drawCallsLastFrame.ToString());
            //gui.label(new Vector2(0, 40), Atma.MonoGame.Graphics.MonoGL.instance.spritesSubmittedLastFrame.ToString());

            //var materials = Disseminate.MonoGame.Graphics.MonoGL.instance.materialsRenderedLastFrame;
            //for(var i = 0; i < materials.Count; i++)
            //{
            //    gui.label(new Vector2(0, 60 + i * 20), materials[i] + "   " + materials[i].GetHashCode().ToString());
            //}

            var y = avg * 0.8f / max;
            graphics.DrawLine(new Vector2(0, screen.height - (y * lineheight)), new Vector2(samples, screen.height - (y * lineheight)), Color.Yellow);
            gui.label(new Vector2(samples + 5, screen.height - (y * lineheight) - 4), ((int)(avg)).ToString() + "ms " + string.Format("{0}", (int)PerformanceMonitor.getRunningMean().get(activity)));

            max *= 1.2f;
            min *= 0.8f;

            if (fps.Length > 0)
            {
                var y0 = (fps[0] / max);
                for (var i = 0; i < fps.Length; i++)
                {
                    var y1 = (fps[i] / max);
                    graphics.DrawLine(new Vector2(i - 1, screen.height - (y0 * lineheight)), new Vector2(i, screen.height - (y1 * lineheight)), Color.Red);
                    y0 = y1;
                    //graphics.DrawLine(new Vector2(i, screen.height), new Vector2(i, screen.height - (fps[i] / max) * lineheight), Color.Red);
                }
            }
            graphics.DrawRect(AxisAlignedBox.FromRect(new Vector2(0, screen.height - lineheight), new Vector2(samples, screen.height)), Color.Green);
        }

        public void shutdown()
        {
        }

        public void update(float delta)
        {
            _fps.Enqueue(delta);
            if (_fps.Count > samples)
                _fps.Dequeue();

            updateCount++;
        }
    }
}
