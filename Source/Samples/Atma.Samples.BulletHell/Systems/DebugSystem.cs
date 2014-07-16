using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Managers;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Atma.Graphics;
using Atma.Rendering;

namespace Atma.Samples.BulletHell.Systems
{
    public class DebugSystem : IComponentSystem, IUpdateSubscriber, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:debug";

        private Color[] graphcolors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan, Color.Orange, Color.White };
        private int lineheight = 100;
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
            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);
            var graphics = this.graphics();
            var screen = this.display();

            gui.label(new Vector2(0, 300), CoreRegistry.require<TestParticleSystem>(TestParticleSystem.Uri).totalParticles.ToString());
            //var fps = _fps.ToArray();
            var index = 0;
            //var spikes = .ToArray();
            //foreach (var kvp in PerformanceMonitor.getRunningMean().OrderByDescending(x => x.Value))
            //{
            //    renderGraph(kvp.Key, index++);
            //}

            renderGraph("render alpha", 0);
            renderGraph("update particles", 1);

            graphics.DrawRect(AxisAlignedBox.FromRect(new Vector2(0, screen.height - lineheight), new Vector2(samples, lineheight)), Color.Green);

        }

        private void renderGraph(string activity, int graphIndex)
        {
            if (graphIndex < 0 || graphIndex > 1)
                return;

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
            var screen = CoreRegistry.require<Atma.Graphics.DisplayDevice>(Atma.Graphics.DisplayDevice.Uri);
            var graphics = this.graphics();

            var y = avg * 0.8f / max;
            //graphics.DrawLine(new Vector2(0, screen.height - (y * lineheight)), new Vector2(samples, screen.height - (y * lineheight)), Color.Yellow);

            var text = string.Format("{0} - {1}/{2}ms", activity, (int)PerformanceMonitor.getRunningMean().get(activity), (int)PerformanceMonitor.getDecayingSpikes().get(activity));
            var color = graphcolors[graphIndex];
            gui.label(new Vector2(samples + 5, screen.height + ((graphIndex) * 20) - lineheight), text, color);

            max *= 1.2f;
            min *= 0.8f;

            if (fps.Length > 0)
            {
                var y0 = (fps[0] / max);
                for (var i = 0; i < fps.Length; i++)
                {
                    var y1 = (fps[i] / max);
                    graphics.DrawLine(new Vector2(i - 1, screen.height - (y0 * lineheight)), new Vector2(i, screen.height - (y1 * lineheight)), color);
                    y0 = y1;
                    //graphics.DrawLine(new Vector2(i, screen.height), new Vector2(i, screen.height - (fps[i] / max) * lineheight), Color.Red);
                }
            }
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

        public void renderOpaque()
        {
        }

        public void renderAlphaBlend()
        {
        }

        public void renderOverlay()
        {

        }
    }
}
