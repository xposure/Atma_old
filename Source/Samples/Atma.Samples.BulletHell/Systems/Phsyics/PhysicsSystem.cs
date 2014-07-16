using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Engine;
using Atma.Entities;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Atma.Managers;
using Atma.Graphics;
using Atma.Assets;
using Atma.Rendering.Sprites;

namespace Atma.Samples.BulletHell.Systems.Phsyics
{
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

    public class PhysicsSystem : IComponentSystem, IUpdateSubscriber, IRenderSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:physics";
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        public void update(float delta)
        {
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;

                var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
                foreach (var id in em.getWithComponents("transform", "physics"))
                {
                    var transform = em.getComponent<Transform>(id, "transform");
                    var physics = em.getComponent<PhysicsComponent>(id, "physics");


                    transform.Position += physics.velocity;
                    //transform.LookAt(wp);
                }
            }
        }

        public void init()
        {
            CoreRegistry.require<GUIManager>(GUIManager.Uri).onRender += PhysicsSystem_onRender;
            //CoreRegistry<GUIManager>.require(
        }

        void PhysicsSystem_onAfterRender(GraphicSubsystem graphics)
        {
            return;
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
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

        void PhysicsSystem_onRender(GUIManager obj)
        {

        }

        public void shutdown()
        {
        }

        public void render()
        {

        }
    }
}
