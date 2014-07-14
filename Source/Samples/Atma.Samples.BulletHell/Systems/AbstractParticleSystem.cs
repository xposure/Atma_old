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
        private CircularParticleArray _particles2 = new CircularParticleArray(4096);

        public void update(float delta)
        {
            accumulator -= tick;
            var removalCount = 0;
            //var removal = new List<int>();
            //int removalCount = 0;
            for(var id = 0; id < _particles2.Count;id++)
            //foreach (var id in _particles.activeItems)
            //for (int i = 0; i < particleList.Count; i++)
            {
                var particle = _particles2[id];
                updateParticle(ref particle);

                particle.PercentLife -= 1f / particle.Duration;
                _particles2[id] = particle;

                // sift deleted particles to the end of the list
                Swap(_particles2, id - removalCount, id);

                if (particle.PercentLife < 0)
                    removalCount++;
            }
            _particles2.Count -= removalCount;
            //foreach (var id in removal)
            //    _particles.FreeItem(id);
            //particleList.Count -= removalCount;
        }

        protected abstract void updateParticle(ref Particle particle);

        public void render()
        {

        }

        public void CreateParticle(Material material, Vector2 position, Color tint, float duration, Vector2 scale, T state, float theta = 0)
        {
            var  particle = new Particle();
            particle.Material = material;
            particle.Position = position;
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
            CoreRegistry.require<SpriteRenderer>(SpriteRenderer.Uri).onBeforeRender += ParticleSystem_onBeforeRender;
            CoreRegistry.require<CameraSystem>(CameraSystem.Uri).renderAdditive += AbstractParticleSystem_renderAdditive;
        }

        void AbstractParticleSystem_renderAdditive(CameraComponent arg1, RenderQueue arg2)
        {

            arg2.depth(0f);
            arg2.source(AxisAlignedBox.Null);
            for(var id = 0; id < _particles2.Count;id++)
            //foreach (var id in _particles.activeItems)
            {
                var p = _particles2[id];

                var size = new Vector2(p.Scale.X * p.Material.texture.width, p.Scale.Y * p.Material.texture.height);
                //var len = p.Length();
                //size *= (1f - len / 2048f);
                //p *= (1f - len / 2048f);
                arg2.texture(p.Material.texture);
                arg2.color(p.Color);
                arg2.quad(p.Position, p.Position + size, new Vector2(0.5f, 0.5f), p.Orientation + MathHelper.PiOver2);

                //graphics.Draw(0,
                //              p.Material,
                //              p.Position,
                //              AxisAlignedBox.Null,
                //              p.Color,
                //              p.Orientation + MathHelper.PiOver2,
                //              new Vector2(0.5f, 0.5f),
                //              new Vector2(p.Scale.X * p.Material.texture.width, p.Scale.Y * p.Material.texture.height),
                //              SpriteEffects.None,
                //              0f);
            }
        }

        void ParticleSystem_onBeforeRender(GraphicSubsystem graphics)
        {
            //return;
            //graphics.GL.flush(SortMode.None);

        }

        public void shutdown()
        {
        }
    }
}
