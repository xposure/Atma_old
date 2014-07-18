using Atma.Assets;
using Atma.Collections;
using Atma.Engine;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Rendering;
using Atma.Rendering.Sprites;
using Atma.Samples.BulletHell.World;
using Texture2D = Atma.Graphics.Texture2D;

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
        //private Map _map;

        public override void init()
        {
            base.init();
            //randomSpawn();
            //_map = this.map();
        }

        public override void update(float delta)
        {
            //if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F))
            //if (((indexr++) % 60) == 0)
            randomSpawn();
            base.update(delta);
        }

        private void randomSpawn()
        {
            var particleMat = assets.getMaterial("bullethell:particle");

            var p = new Vector2(random.NextFloat() * display.width - (display.width / 2), random.NextFloat() * display.height - (display.height / 2));

            float hue1 = random.NextFloat() * 6;
            float hue2 = (hue1 + random.NextFloat() * 2) % 6f;
            Color color1 = Utility.HSVToColor(hue1, 0.5f, 1);
            Color color2 = Utility.HSVToColor(hue2, 0.5f, 1);
            Color color = Color.Lerp(color1, color2, random.NextFloat());

            for (int j = 0; j < 1; j++)
            {
                float speed = 18f * (1f - 1 / (random.NextFloat() * 10f + 1));
                //float speed = 18f * (1f - 1 / (random.NextFloat() * 10f + 1));
                var theta = random.NextFloat() * MathHelper.TwoPi;// new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                //v.Normalize();

                //theta *= speed;
                var state = new ParticleState()
                {
                    speed = speed,
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 32f
                };


                CreateParticle(particleMat, p, color, 40, new Vector2(2f, 0f), state, theta);
            }
        }
        private int indexr = 0;

        protected override void updateParticle(ref Particle particle)
        {
            //var cell = _map.getCellFromWorld(particle.Position);
            var cell = CellType.WALL;

            //particle.Color = Color.White;
            if (cell == CellType.WALL || cell == CellType.PERIMITER)
            {
                //particle.Color = Color.Red;
                particle.Position += particle.Direction * particle.State.speed;
            }
            else
            {
                //var newp = particle.Position + particle.Direction * particle.State.speed;
                //var newcell = _map.getCellFromWorld(newp);

                //if (newcell == CellType.WALL || newcell == CellType.PERIMITER)
                //{
                //    particle.Color = Color.Red;
                //    //var ray = new Ray(particle.Position, particle.Direction);
                //    //var aabb = _map.getCellAABBFromWorld(newp);

                //    //if (ray.Intersects(aabb).Hit)
                //    {
                //        indexr++;
                //        if ((indexr % 2) == 0)
                //            particle.Direction = new Vector2(particle.Direction.Y, -particle.Direction.X);
                //        else
                //            particle.Direction = new Vector2(-particle.Direction.Y, particle.Direction.X);

                //        particle.Orientation = (float)Math.Atan2(particle.Direction.Y, particle.Direction.X);// +MathHelper.PiOver2;
                //    }
                //    //particle.Direction = dir;

                //    //var i = Utility.Intersects(ray, aabb);
                //    //i.
                //}
                //particle.Position += particle.Direction * particle.State.speed;
            }




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
            //int width = 1024;
            //int height = 768;

            // collide with the edges of the screen
            var dirty = false;

            //if (pos.X < 0) { dir.X = ax; dirty = true; }
            //else if (pos.X > width) { dir.X = -ax; dirty = true; }

            //if (pos.Y < 0) { dir.Y = ay; dirty = true; }
            //else if (pos.Y > height) { dir.Y = -ay; dirty = true; }

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

    public abstract class AbstractParticleSystem<T> : GameSystem, IComponentSystem, IUpdateSubscriber, IRenderSubscriber, IRenderSystem
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
        private Texture2D lightTexture;
        private SpriteBatch2 batch;
        private CircularParticleArray _particles2 = new CircularParticleArray(1024 * 8);
        protected Random random = new Random();

        public int totalParticles { get { return _particles2.Count; } }

        public virtual void update(float delta)
        {
            //return;

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
            //particle.Position = new Vector2(64, 64);

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

        public virtual void init()
        {
            batch = new SpriteBatch2(display.device);
            lightTexture = Texture2D.createMetaball("TEXTURE:bullethell:particlelight", 64, Texture2D.circleFalloff, Texture2D.colorWhite);
        }


        public void shutdown()
        {
        }

        public void renderOpaque()
        {
            ////return;
            //var world = this.world();
            //var graphics = this.graphics();
            //var id = 0;
            //while (id < _particles2.Count)
            //{
            //    batch.Begin(SpriteSortMode.Deferred);
            //    for (; id < _particles2.Count; id++)
            //    {
            //        var p = _particles2[id];

            //        batch.drawLine(p.Material.texture, p.Position, p.Position + p.Direction * p.Scale.Y, color: Color.Blue, width: 4f * p.Scale.X, depth: 0.1f);
            //        //batch.Draw(p.Material.texture,
            //        //      position: p.Position, scale: p.Scale, rotation: p.Orientation + MathHelper.PiOver2,
            //        //      color: p.Color, origin: new Vector2(0f, 0.5f));


            //        if (id > 0 && (id % 4096) == 0)
            //            break;
            //    }
            //    batch.End();
            //    id++;
            //}
        }

        private float ang = 0;
        public void renderShadows()
        {
            //var asset = this.assets();
            //var mat = asset.getMaterial("bullethell:particlelight");
            //var display = this.display();
            //var world = this.world();
            var id = 0;

            while (id < _particles2.Count)
            {
                batch.Begin(SpriteSortMode.Deferred);
                for (; id < _particles2.Count; id++)
                {
                    var p = _particles2[id];

                    var intensity = p.Color.A / 255f; // MathHelper.Max(0.5f, MathHelper.Min(0.5f, p.Color.A / 255f));
                    //batch.drawLine(lightTexture, p.Position - p.Direction * 128 , p.Position + p.Direction * (128 + p.Scale.Y) , color: p.Color, width: 128f );
                    batch.draw(lightTexture, p.Position, new Vector2(150, 150) * intensity, color: Color.Black, origin: new Vector2(0.5f, 0.5f));
                    batch.draw(lightTexture, p.Position, new Vector2(100, 100) * intensity, origin: new Vector2(0.5f, 0.5f),
                        color: Color.FromNonPremultiplied(p.Color.R, p.Color.G, p.Color.B, (byte)(196 * intensity)));

                    //batch.draw(mat.texture,
                    //   position: p.Position, size: new Vector2(32, 32), rotation: ang,// p.Orientation + MathHelper.PiOver2,
                    //   color: Color.Red, origin: new Vector2(0.5f, 0.5f));
                    //ang += 0.1f;
                    ////batch.Draw(mat.texture,
                    //      position: p.Position, scale: new Vector2(32,32) , rotation: 0,// p.Orientation + MathHelper.PiOver2,
                    //      color: Color.Red, origin: new Vector2(0.5f, 0.5f));


                    if (id > 0 && (id % 4096) == 0)
                        break;
                }
                batch.End();
                id++;
            }
        }

        public void renderAlphaBlend()
        {
            var currentBlend = display.device.BlendState;
            display.device.BlendState = BlendState.Additive;
            //return;
            var id = 0;
            while (id < _particles2.Count)
            {
                batch.Begin(SpriteSortMode.Deferred);
                for (; id < _particles2.Count; id++)
                {
                    var p = _particles2[id];

                    batch.drawLine(p.Material.texture, p.Position, p.Position + p.Direction * p.Scale.Y, color: p.Color, width: 4f * p.Scale.X);
                    //batch.Draw(p.Material.texture,
                    //      position: p.Position, scale: p.Scale, rotation: p.Orientation + MathHelper.PiOver2,
                    //      color: p.Color, origin: new Vector2(0f, 0.5f));


                    if (id > 0 && (id % 4096) == 0)
                        break;
                }
                batch.End();
                id++;
            }
            display.device.BlendState = currentBlend;
        }

        public void renderOverlay()
        {
        }
    }
}
