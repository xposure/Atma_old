using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entity;
using Atma.Managers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using GameName1.BulletHell;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Atma.Samples.BulletHell.Systems.Controllers
{
    public class PlayerController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:player";

        public void update(float delta)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            var input = CoreRegistry.require<InputManager>(InputManager.Uri);
            var player = em.createRef(em.getEntityByTag("player"));

            if (player.exists && player.hasComponent("input", "transform"))
            {
                var controller = player.getComponent<InputComponent>("input");
                var transform = player.getComponent<Transform>("transform");

                var steering = Vector2.Zero;
                if (input.IsKeyDown(Keys.A)) steering += new Vector2(-1, 0);
                if (input.IsKeyDown(Keys.D)) steering += new Vector2(1, 0);
                if (input.IsKeyDown(Keys.W)) steering += new Vector2(0, -1);
                if (input.IsKeyDown(Keys.S)) steering += new Vector2(0, 1);

                controller.thrust = steering;

                var wp = Camera.mainCamera.screenToWorld(input.MousePosition);


                controller.fireWeapon = input.IsLeftMouseDown;
                controller.fireDirection = wp - transform.DerivedPosition;
                //var transform = player.getComponent<Transform>("transform");
                //var physics = player.getComponent<PhysicsComponent>("physics");
                //var hasInput = input.IsAnyKeyDown(Keys.A, Keys.S, Keys.D, Keys.W);
                //if (hasInput)
                //{
                //    var p = transform.DerivedPosition;
                //    //var steering = Steering.seek(speed, p, p + velocity);
                //    var steering = Steering.zero;


                //    if (input.IsKeyDown(Keys.A)) steering += Steering.seek(physics.speed, p, p + new Vector2(-1, 0));
                //    if (input.IsKeyDown(Keys.D)) steering += Steering.seek(physics.speed, p, p + new Vector2(1, 0));
                //    if (input.IsKeyDown(Keys.W)) steering += Steering.seek(physics.speed, p, p + new Vector2(0, -1));
                //    if (input.IsKeyDown(Keys.S)) steering += Steering.seek(physics.speed, p, p + new Vector2(0, 1));

                //    physics.velocity = steering.integrate(p, physics.velocity, physics.maxForce, physics.speed, physics.mass);
                //}
                //else
                //{
                //    physics.velocity *= physics.drag;
                //    if (physics.velocity.X >= -0.01 && physics.velocity.X <= 0.01)
                //        physics.velocity.X = 0f;

                //    if (physics.velocity.Y >= -0.01 && physics.velocity.Y <= 0.01)
                //        physics.velocity.Y = 0f;
                //}
            }
        }

        public void init()
        {

        }

        public void shutdown()
        {
        }
    }
}
