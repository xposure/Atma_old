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
using Atma.Assets;
using Atma.Core;

namespace Atma.Samples.BulletHell.Systems.Controllers
{
    public class PlayerController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:player";
        private Random random = new Random();

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
                var wp = CameraComponent.mainCamera.screenToWorld(input.MousePosition);

                var state = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);

                if (state.IsConnected)
                {
                    controller.fireDirection = state.ThumbSticks.Right;
                    controller.fireDirection.Y = -controller.fireDirection.Y;
                    controller.fireWeapon = controller.fireDirection != Vector2.Zero;
                    controller.thrust = state.ThumbSticks.Left;
                    controller.thrust.Y = -controller.thrust.Y;
                    steering = controller.thrust;
                }
                else
                {

                    if (input.IsKeyDown(Keys.A)) steering += new Vector2(-1, 0);
                    if (input.IsKeyDown(Keys.D)) steering += new Vector2(1, 0);
                    if (input.IsKeyDown(Keys.W)) steering += new Vector2(0, -1);
                    if (input.IsKeyDown(Keys.S)) steering += new Vector2(0, 1);

                    controller.fireWeapon = input.IsLeftMouseDown;
                    controller.fireDirection = wp - transform.DerivedPosition;
                    controller.thrust = steering;
                }

                if (steering != Vector2.Zero)
                {
                    var time = CoreRegistry.require<TimeBase>(TimeBase.Uri);

                    float hue1 = (time.gameTime * 6) % 6f;
                    //float hue2 = (hue1 + time.gameTime * 2) % 6f;
                    Color color1 = Utility.HSVToColor(hue1, 0.5f, 1);
                    //Color color2 = Utility.HSVToColor(hue2, 0.5f, 1);

                    var pm = CoreRegistry.require<TestParticleSystem>(TestParticleSystem.Uri);
                    var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
                    var particleMat = assets.getMaterial("bullethell:particle");

                    for (int k = 0; k < 30; k++)
                    {
                        float speed = 6f * (1f - 1 / (random.NextFloat() * 2f + 1));

                        var v = transform.DerivedBackward;
                        v = v.Rotate(Vector2.Zero, (random.NextFloat() * 0.5f - 0.25f) );
                        v *= speed;

                        pm.CreateParticle(particleMat, transform.DerivedPosition, color1, 40, new Vector2(0.25f, 0.55f),
                            new ParticleState() { Velocity = v, Type = ParticleType.Bullet, LengthMultiplier = 1f });
                    }

                }
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
