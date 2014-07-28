using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Engine;
using Atma.Entities;
using Microsoft.Xna.Framework;
using Atma.Managers;
using Atma.Graphics;
using Atma.Assets;
using Atma.Rendering.Sprites;
using Atma.Samples.BulletHell.World;
using Atma.Rendering;

namespace Atma.Samples.BulletHell.Systems.Phsyics
{
    public interface ICollisionSystem
    {
        void broadphase(CollisionQuery query, List<Collision> collisions);
    }

    public struct Collision
    {
        public int flag;
        public int gameObject;
        public AxisAlignedBox aabb;

        public Collision(int flag, int gameObject, AxisAlignedBox aabb)
        {
            this.flag = flag;
            this.gameObject = gameObject;
            this.aabb = aabb;
        }
    }

    public struct CollisionQuery
    {
        public int flag;
        public int gameObject;
        public AxisAlignedBox aabb;

        public CollisionQuery(int flag, int gameObject, AxisAlignedBox aabb)
        {
            this.flag = flag;
            this.gameObject = gameObject;
            this.aabb = aabb;
        }
    }

    public class PhysicsComponent : Component
    {
        public float speed = 2f;
        public Vector2 velocity;
        public float mass = 10f;
        public float maxForce = 1.4f;
        public float drag = 0.94f;
        public float radius = 12f;
        protected AxisAlignedBox _bounds = AxisAlignedBox.Null;
        //public AxisAlignedBox Bounds
        //{
        //    get
        //    {
        //        if (_needsUpdate)
        //            updateBounds();
        //        return _bounds;
        //    }
        //}


    }

    public class PhysicsSystem : IComponentSystem, IUpdateSubscriber, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:physics";
        private int steps = 0;
        private int checks = 0;
        private int hits = 0;
        private float tick = 1f / 60f;
        private float accumulator = 0f;
        private List<Collision> _broadphase = new List<Collision>();

        private bool hasCollision(AxisAlignedBox check)
        {
            foreach (var c in _broadphase)
            {
                checks++;
                if (c.aabb.Intersects(check))
                    return true;
            }

            return false;
        }

        public void update(float delta)
        {

            checks = 0;
            steps = 0;
            hits = 0;

            PerformanceMonitor.start("physics loop");
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;

                var map = CoreRegistry.require<Map>();
                var em = CoreRegistry.require<EntityManager>();
                foreach (var id in em.getWithComponents("transform", "physics"))
                {
                    _broadphase.Clear();

                    var transform = em.getComponent<Transform>(id, "transform");
                    var physics = em.getComponent<PhysicsComponent>(id, "physics");

                    var p = transform.DerivedPosition;
                    var m = physics.velocity;

                    var shape = AxisAlignedBox.FromDimensions(p, new Vector2(16, 16));
                    var sweep = AxisAlignedBox.FromDimensions(shape.Center, shape.Size);
                    sweep.Center = p + m;
                    sweep.Merge(shape);

                    var query = new CollisionQuery(0, id, sweep);
                    foreach (var c in CoreRegistry.getBy<ICollisionSystem>())
                        c.broadphase(query, _broadphase);

                    hits += _broadphase.Count;

                    var dx = Math.Abs(m.X);
                    var dy = Math.Abs(m.Y);
                    var sx = Math.Sign(m.X);
                    var sy = Math.Sign(m.Y);
                    var px = new Vector2(sx, 0);
                    var py = new Vector2(0, sy);

                    while (dx > 0 || dy > 0)
                    {
                        steps++;
                        if (dx > dy)
                        {
                            shape.Center = p + px;
                            var collided = hasCollision(shape);
                            if (collided && dy <= 0)
                                dx = 0;
                            if (!collided)
                                p.X += sx;
                            else
                                physics.velocity.X = 0;

                            dx--;
                        }
                        else
                        {
                            shape.Center = p + py;
                            var collided = hasCollision(shape);
                            if (collided && dx <= 0)
                                dy = 0;
                            if (!collided)
                                p.Y += sy;
                            else
                                physics.velocity.Y = 0;

                            dy--;
                        }
                    }

                    transform.Position = p;


                    //physics.velocity *= physics.drag;
                    //transform.LookAt(wp);
                }
            }
            PerformanceMonitor.end("physics loop");
        }

        public void init()
        {
            CoreRegistry.require<GUIManager>().onRender += PhysicsSystem_onRender;
            //CoreRegistry<GUIManager>.require(
        }

        void PhysicsSystem_onAfterRender(GraphicSubsystem graphics)
        {
            return;
            var assets = CoreRegistry.require<AssetManager>();
            var em = CoreRegistry.require<EntityManager>();
            var material = assets.getMaterial("engine:default");

            //graphics.GL.begin(target, SortMode.None, Camera.mainCamera.ViewMatrix, Camera.mainCamera.viewport);
            foreach (var id in em.getWithComponents("transform", "physics"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var physics = em.getComponent<PhysicsComponent>(id, "physics");

                graphics.DrawCircle(material, transform.DerivedPosition, physics.radius, 12, Color.Gray);
            }
            //graphics.GL.end();
        }

        void PhysicsSystem_onRender(GUIManager gui)
        {
            //var gui = CoreRegistry.require<GUIManager>();
            gui.label(new Vector2(0, 400), string.Format("steps: {0}", steps));
            gui.label(new Vector2(0, 420), string.Format("checks: {0}", checks));
            gui.label(new Vector2(0, 440), string.Format("hits: {0}", hits));
        }

        public void shutdown()
        {
        }

        public void render()
        {

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

        public void renderShadows()
        {
        }
    }
}
