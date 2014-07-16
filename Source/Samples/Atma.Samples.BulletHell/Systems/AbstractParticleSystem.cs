using Atma.Assets;
using Atma.Collections;
using Atma.Engine;
using Atma.Graphics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Rendering;
using Atma.Rendering.Sprites;

namespace Atma.Samples.BulletHell.Systems
{
    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

    public struct ParticleState
    {
        public float speed;
        public ParticleType Type;
        public float LengthMultiplier;
    }

    public class TestParticleSystem : AbstractParticleSystem<ParticleState>
    {
        public override void update(float delta)
        {
            //if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F))
                randomSpawn();
                base.update(delta);
        }

        private void randomSpawn()
        {
            var display = this.display();
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            var particleMat = assets.getMaterial("bullethell:particle");

            var p = new Vector2(random.NextFloat() * display.width - (display.width / 2), random.NextFloat() * display.height - (display.height / 2));

            float hue1 = random.NextFloat() * 6;
            float hue2 = (hue1 + random.NextFloat() * 2) % 6f;
            Color color1 = Utility.HSVToColor(hue1, 0.5f, 1);
            Color color2 = Utility.HSVToColor(hue2, 0.5f, 1);
            Color color = Color.Lerp(color1, color2, random.NextFloat());

            for (int j = 0; j < 1; j++)
            {
                float speed = 18f *(1f - 1 / (random.NextFloat() * 10f + 1));
                var theta = random.NextFloat() * MathHelper.TwoPi;// new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                //v.Normalize();

                //theta *= speed;
                var state = new ParticleState()
                {
                    speed = speed,
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 0.1f
                };


                CreateParticle(particleMat, p, color, 50, new Vector2(0.5f, 1.5f), state, theta);
            }
        }

        protected override void updateParticle(ref Particle particle)
        {
            //var vel = particle.State.Velocity;

            particle.Position += particle.Direction * particle.State.speed;

            var dir = particle.Direction;
            var ax = dir.X;
            var ay = dir.Y;
            if (ax < 0) ax = -ax;
            if (ay < 0) ay = -ay;

            float speed = particle.State.speed;
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;
            //if (particle.PercentLife > 0.985f)
            //    alpha = 0;

            particle.Color.A = (byte)(255 * alpha * 0.5f);
            //particle.Color.A = 0;

            particle.Scale.Y = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

            var pos = particle.Position + new Vector2(512, 384);// new Vector2(1024, 768) / 2f;
            int width = 1024;
            int height = 768;

            // collide with the edges of the screen
            var dirty = false;

            if (pos.X < 0) { dir.X = ax; dirty = true; }
            else if (pos.X > width) { dir.X = -ax; dirty = true; }

            if (pos.Y < 0) { dir.Y = ay; dirty = true; }
            else if (pos.Y > height) { dir.Y = -ay; dirty = true; }

            if (ax + ay < 0.00001f)
            {
                dir = Vector2.Zero;
                particle.Direction = dir;
            }
                
            if (dirty)
            {
                particle.Orientation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;
                particle.Direction = dir;
            }

            // denormalized floats cause significant performance issues



            speed *= 0.97f;       // particles gradually slow down
            particle.State.speed = speed;
        }
    }

    public abstract class AbstractParticleSystem<T> : IComponentSystem, IUpdateSubscriber, IRenderSubscriber, IRenderSystem
    {
        private class CircularParticleArray
        {
            private int start;
            public int Start
            {
                get { return start; }
                set { start = value % list.Length; }
            }

            public int Count { get; set; }
            public int Capacity { get { return list.Length; } }
            private Particle[] list;

            public CircularParticleArray(int capacity)
            {
                list = new Particle[capacity];
            }

            public Particle this[int i]
            {
                get { return list[(start + i) % list.Length]; }
                set { list[(start + i) % list.Length] = value; }
            }
        }

        public static readonly GameUri Uri = "componentsystem:particle";

        protected struct Particle
        {
            public Material Material;
            public Vector2 Position;
            public Vector2 Direction;
            public float Orientation;

            public Vector2 Scale;

            public Color Color;
            public float Duration;
            public float PercentLife;
            public T State;
        }

        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        //private Queue<int> _particles = new Queue<Particle>(1024);
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        //private ObjectPool<Particle> _particles = new ObjectPool<Particle>(4096);
        private CircularParticleArray _particles2 = new CircularParticleArray(1024 * 8);
        protected Random random = new Random();

        public int totalParticles { get { return _particles2.Count; } }

        public virtual void update(float delta)
        {

            PerformanceMonitor.start("update particles");
            accumulator -= tick;
            var removalCount = 0;
            //var removal = new List<int>();
            //int removalCount = 0;
            for (var index = 0; index < _particles2.Count; index++)
            //foreach (var id in _particles.activeItems)
            //for (int i = 0; i < particleList.Count; i++)
            {
                var particle = _particles2[index];
                updateParticle(ref particle);

                particle.PercentLife -= 1f / particle.Duration;
                _particles2[index] = particle;

                // sift deleted particles to the end of the list
                Swap(_particles2, index - removalCount, index);

                if (particle.PercentLife < 0)
                    removalCount++;
            }
            _particles2.Count -= removalCount;
            //foreach (var id in removal)
            //    _particles.FreeItem(id);
            //particleList.Count -= removalCount;
            PerformanceMonitor.end("update particles");
        }

        protected abstract void updateParticle(ref Particle particle);

        public void render()
        {

        }

        public void CreateParticle(Material material, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta)
        {
            var particle = new Particle();
            particle.Material = material;
            particle.Position = position;
            particle.Direction = new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
            particle.Color = tint;

            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;

            if (_particles2.Count == _particles2.Capacity)
            {
                // if the list is full, overwrite the oldest particle, and rotate the circular list
                _particles2[0] = particle;
                _particles2.Start++;
            }
            else
            {
                _particles2[_particles2.Count] = particle;
                _particles2.Count++;
            }
        }

        public void init()
        {
            //CoreRegistry.require<SpriteRenderer>(SpriteRenderer.Uri).onBeforeRender += ParticleSystem_onBeforeRender;

        }

     

    

        public void shutdown()
        {
        }

        public void renderOpaque()
        {
        }

        public void renderAlphaBlend()
        {


            //return;
            var world = this.world();
            var graphics = this.graphics();
            var id = 0;
            while (id < _particles2.Count)
            {
                world.beginAdditive();
                for (; id < _particles2.Count; id++)
                //foreach (var id in _particles.activeItems)
                {
                    var p = _particles2[id];

                    graphics.batch.Draw(p.Material.texture,
                          position: p.Position, scale: p.Scale, rotation: p.Orientation + MathHelper.PiOver2,
                          color: p.Color, origin: p.Material.textureSize * 0.5f);

                    ////var size = new Vector2(p.Scale.X * p.Material.texture.width, p.Scale.Y * p.Material.texture.height);
                    ////var len = p.Length();
                    ////size *= (1f - len / 2048f);
                    ////p *= (1f - len / 2048f);
                    //arg2.texture(p.Material.texture);
                    //arg2.color(p.Color);
                    //arg2.quad(p.Position, p.Position + size, new Vector2(0.5f, 0.5f), p.Orientation + MathHelper.PiOver2);

                    ////graphics.Draw(0,
                    ////              p.Material,
                    ////              p.Position,
                    ////              AxisAlignedBox.Null,
                    ////              p.Color,
                    ////              p.Orientation + MathHelper.PiOver2,
                    ////              new Vector2(0.5f, 0.5f),
                    ////              new Vector2(p.Scale.X * p.Material.texture.width, p.Scale.Y * p.Material.texture.height),
                    ////              SpriteEffects.None,
                    ////              0f);
                    if (id > 0 && (id % 4096) == 0)
                        break;
                }
                world.endAdditive();
                id++;
            }
        }

        public void renderOverlay()
        {
        }
    }
}
