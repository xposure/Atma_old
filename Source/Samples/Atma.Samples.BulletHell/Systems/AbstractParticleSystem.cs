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

namespace Atma.Samples.BulletHell.Systems
{
    public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

    public struct ParticleState
    {
        public Vector2 Velocity;
        public ParticleType Type;
        public float LengthMultiplier;
    }

    public class TestParticleSystem : AbstractParticleSystem<ParticleState>
    {

        protected override void updateParticle(ref Particle particle)
        {
            var vel = particle.State.Velocity;

            particle.Position += vel;
            particle.Orientation = vel.ToAngle();

            float speed = vel.Length();
            float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
            alpha *= alpha;
            if (particle.PercentLife > 0.985f)
                alpha = 0;

            particle.Color.A = (byte)(255 * alpha);
            //particle.Color.A = 0;

            particle.Scale.Y = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

            var pos = particle.Position + new Vector2(1024, 768) / 2f;
            int width = 1024;
            int height = 768;

            // collide with the edges of the screen
            if (pos.X < 0) vel.X = Math.Abs(vel.X);
            else if (pos.X > width)
                vel.X = -Math.Abs(vel.X);
            if (pos.Y < 0) vel.Y = Math.Abs(vel.Y);
            else if (pos.Y > height)
                vel.Y = -Math.Abs(vel.Y);

            // denormalized floats cause significant performance issues
            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00001f)
                vel = Vector2.Zero;

            vel *= 0.965f;       // particles gradually slow down
            particle.State.Velocity = vel;
        }
    }

    public abstract class AbstractParticleSystem<T> : IComponentSystem, IUpdateSubscriber, IRenderSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:particle";

        protected struct Particle
        {
            public Material Material;
            public Vector2 Position;
            public float Orientation;

            public Vector2 Scale;

            public Color Color;
            public float Duration;
            public float PercentLife;
            public T State;
        }

        //private Queue<int> _particles = new Queue<Particle>(1024);
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        private ObjectPool<Particle> _particles = new ObjectPool<Particle>(4096);

        public void update(float delta)
        {
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;
                var removal = new List<int>();
                //int removalCount = 0;
                foreach (var id in _particles.activeItems)
                //for (int i = 0; i < particleList.Count; i++)
                {
                    var particle = _particles[id];
                    updateParticle(ref particle);

                    particle.PercentLife -= 1f / particle.Duration;
                    _particles[id] = particle;

                    // sift deleted particles to the end of the list
                    //Swap(particleList, i - removalCount, i);

                    // if the particle has expired, delete this particle
                    if (particle.PercentLife < 0)
                        removal.Add(id);
                    //    removalCount++;
                }

                foreach (var id in removal)
                    _particles.FreeItem(id);
                //particleList.Count -= removalCount;
            }
        }

        protected abstract void updateParticle(ref Particle particle);

        public void render()
        {

        }

        public void CreateParticle(Material material, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
        {
            if (_particles.Count == 4096)
                _particles.FreeItem( _particles.indexOf(0));

            var id = _particles.GetFreeItem();

            var particle = _particles[id];
            // Create the particle
            particle.Material = material;
            particle.Position = position;
            particle.Color = tint;

            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.Scale = scale;
            particle.Orientation = theta;
            particle.State = state;

            _particles[id] = particle;
        }

        public void init()
        {
            CoreRegistry.require<SpriteRenderer>(SpriteRenderer.Uri).onBeforeRender += ParticleSystem_onBeforeRender;
        }

        void ParticleSystem_onBeforeRender(GraphicSubsystem graphics)
        {
            graphics.GL.flush(SortMode.None);

            foreach (var id in _particles.activeItems)
            {
                var p = _particles[id];

                graphics.Draw(0,
                              p.Material,
                              p.Position,
                              AxisAlignedBox.Null,
                              p.Color,
                              p.Orientation + MathHelper.PiOver2,
                              new Vector2(0.5f, 0.5f),
                              new Vector2(p.Scale.X * p.Material.texture.width , p.Scale.Y * p.Material.texture.height),
                              SpriteEffects.None,
                              0f);
            }
        }

        public void shutdown()
        {
        }
    }
}
