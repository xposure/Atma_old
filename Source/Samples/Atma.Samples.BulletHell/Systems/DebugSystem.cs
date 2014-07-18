using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Managers;
using Microsoft.Xna.Framework;
using Atma.Graphics;
using Atma.Rendering;

namespace Atma.Samples.BulletHell.Systems
{
    public class DebugSystem :GameSystem, IComponentSystem, IUpdateSubscriber, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:debug";

        private Texture2D _fpsLine;
        private Texture2D _basewhite;

        public void init()
        {
            //CoreRegistry.require<SpriteRenderer>(SpriteRenderer.Uri).onAfterRender += DebugSystem_onAfterRender;
            CoreRegistry.require<GUIManager>(GUIManager.Uri).onRender += DebugSystem_onRender;
            _fpsLine = new Texture2D("TEXTURE:bullethell:fpsline", 1, 2);
            _fpsLine.setData(new Color[] { Color.Green, Color.Red });
            _basewhite = new Texture2D("TEXTURE:bullethell:basewhite", 1, 1);
            _basewhite.setData(new Color[] { Color.White });
        }

        void DebugSystem_onRender(GUIManager obj)
        {
            var index = 0;
            foreach (var kvp in PerformanceMonitor.getRunningMean().OrderByDescending(x => x.Value))
            {
                if (kvp.Value > 2f)
                    renderFPSLine(kvp.Key, index++);
            }

            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);
            gui.label(new Vector2(0, 20), "sprites rendered: " + Graphics.GraphicSubsystem.spritesRendered.ToString());
            gui.label(new Vector2(0, 40), "texture swaps: " + Graphics.GraphicSubsystem.textureChanges.ToString());
        }

        private void renderFPSLine(string activity, int graphIndex)
        {
            const int linewidth = 100;
            const int lineheight = 15;
            const int linethickness = 4;
            const float maxms = 16.6f;

            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);

            var metricData = PerformanceMonitor.getMetricData();

            var means = PerformanceMonitor.getRunningMean();
            var mean = means.get(activity);

            var spikes = PerformanceMonitor.getDecayingSpikes();
            var spike = spikes.get(activity);

            var totals = PerformanceMonitor.getRunningTotals();
            var total = totals.get(activity);

            var lastframe = PerformanceMonitor.getLastFrame();
            var frame = lastframe.get(activity);

            var p = new Vector2(20, display.height - graphIndex * lineheight - lineheight);
            var t = new Vector2(linewidth + p.X, p.Y);

            graphics.batch.drawLine(_fpsLine, p, t, width: linethickness, depth: 0.2f);

            var mx = (float)(mean / maxms);
            var sx = (float)(spike / maxms);
            var fx = frame / maxms;

            var warn = false;
            if (mx > 1f || sx > 1f || fx > 1f)
            {
                warn = true;
                mx = MathHelper.Min(mx, 1f);
                sx = MathHelper.Min(sx, 1f);
                fx = MathHelper.Min(fx, 1f);
            }


            graphics.batch.drawLine(_basewhite, new Vector2(mx * linewidth + p.X, p.Y - linethickness), new Vector2(mx * linewidth + p.X, p.Y + linethickness), width: 6, color: Color.Yellow);
            graphics.batch.drawLine(_basewhite, new Vector2(sx * linewidth + p.X, p.Y - linethickness), new Vector2(sx * linewidth + p.X, p.Y + linethickness), width: 4, color: Color.Red);
            graphics.batch.drawLine(_basewhite, new Vector2(fx * linewidth + p.X, p.Y - linethickness), new Vector2(fx * linewidth + p.X, p.Y + linethickness), width: 2);

            gui.label(p + new Vector2(-20, -10), frame.ToString(), warn ? Color.Yellow : Color.White);
            gui.label(t + new Vector2(10, -10), activity, warn ? Color.Yellow : Color.White);
            //gui.label(t + new Vector2(10, -10), string.Format("{0} ({1}f/{2}s/{3}m)", activity, (int)frame, (int)spike, (int)mean));
        }


        public void shutdown()
        {
        }

        public void update(float delta)
        {

        }

        public void renderOpaque()
        {
        }
        public void renderShadows()
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
