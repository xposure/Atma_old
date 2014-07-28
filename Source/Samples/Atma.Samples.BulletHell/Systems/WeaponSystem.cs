using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Atma.Samples.BulletHell.Systems.Controllers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Microsoft.Xna.Framework;
using Atma.Assets;
using Atma.Rendering.Sprites;
using Atma.Samples.BulletHell.World;

namespace Atma.Samples.BulletHell.Systems
{
    public class WeaponComponent : Component
    {
        public float fireRate = 0.15f;
        public float fireTimer = 0f;
        public float damage = 1f;
        public float velocity = 15f;
        public float spread = 0.04f;
        public Texture2D material;
        //public float energy = 5f;
    }

    public class WeaponSystem : GameSystem, IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:weapon";

        private Map _map;
        private Random random = new Random();

        public void update(float delta)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F))
            {
                var hud = CoreRegistry.require<HUDSystem>();
                var em = CoreRegistry.require<EntityManager>();
                var pm = CoreRegistry.require<TestParticleSystem>();
                var assets = CoreRegistry.require<AssetManager>();
                var particleMat = assets.getMaterial("bullethell:particle");

                foreach (var id in em.getEntitiesByTag("enemy").ToArray())
                {
                    var transform = em.getComponent<Transform>(id, "transform");
                    destroy(em, id, transform.DerivedPosition, particleMat, pm);
                }
            }

            doBullets(delta);
            doDamage(delta);
            //doTrack(delta);

        }

        //private void doTrack(float delta)
        //{
        //    var em = CoreRegistry.require<EntityManager>();
        //    var ids = em.getWithComponents("transform", "gun").ToArray();
        //    foreach (var id in ids)
        //    {
        //        var transform = em.getComponent<Transform>(id, "transform");
        //        var wp = GameWorld.instance.currentCamera.screenToWorld(input.MousePosition);
        //        //var input = em.getComponent<InputComponent>(id, "input");
        //        //var weapon = em.getComponent<WeaponComponent>(id, "weapon");

        //        //if (weapon.fireTimer > 0f)
        //        //    weapon.fireTimer -= delta;

        //        //if (input.fireWeapon && weapon.fireTimer <= 0f)
        //        //{

        //        //    weapon.fireTimer = weapon.fireRate;
        //        //    var dir = input.fireDirection;
        //        //    dir.Normalize();

        //        //    var left = new Vector2(-12, 24);
        //        //    var right = new Vector2(12, 24);
        //        //    left = left.Rotate(Vector2.Zero, dir.GetRotation() - MathHelper.PiOver2);
        //        //    right = right.Rotate(Vector2.Zero, dir.GetRotation() - MathHelper.PiOver2);

        //        //    var randomSpread = (random.Next(-weapon.spread, weapon.spread) + random.Next(-weapon.spread, weapon.spread)) / 2f;
        //        //    createBullet(em, weapon, transform.DerivedPosition + left, dir.GetRotation() + randomSpread);
        //        //    createBullet(em, weapon, transform.DerivedPosition + right, dir.GetRotation() + randomSpread);
        //        //}


        //    }
        //}

        private void doBullets(float delta)
        {
            var em = CoreRegistry.require<EntityManager>();
            var ids = em.getWithComponents("transform", "input", "weapon").ToArray();
            foreach (var id in ids)
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var input = em.getComponent<InputComponent>(id, "input");
                var weapon = em.getComponent<WeaponComponent>(id, "weapon");

                if (weapon.fireTimer > 0f)
                    weapon.fireTimer -= delta;

                if (input.fireWeapon && weapon.fireTimer <= 0f)
                {

                    weapon.fireTimer = weapon.fireRate;
                    var dir = input.fireDirection;
                    dir.Normalize();

                    var left = new Vector2(-12, 24);
                    var right = new Vector2(12, 24);
                    left = left.Rotate(Vector2.Zero, dir.GetRotation() - MathHelper.PiOver2);
                    right = right.Rotate(Vector2.Zero, dir.GetRotation() - MathHelper.PiOver2);

                    var randomSpread = (random.Next(-weapon.spread, weapon.spread) + random.Next(-weapon.spread, weapon.spread)) / 2f;
                    createBullet(em, weapon, transform.DerivedPosition + left, dir.GetRotation() + randomSpread);
                    createBullet(em, weapon, transform.DerivedPosition + right, dir.GetRotation() + randomSpread);
                }
            }
        }

        private struct DamageContainer : IRadixKey
        {
            public PhysicsComponent physics;
            public Transform transform;
            public HealthComponent health;
            public bool isBullet;
            public int id;
            public bool destroyed;

            public int Key { get { return MinX; } }

            public int MinX { get { return (int)(transform.DerivedPosition.X - physics.radius); } }
            public int MaxX { get { return (int)(transform.DerivedPosition.X + physics.radius); } }

            public bool isValid { get { return physics != null && transform != null; } }
        }

        private List<DamageContainer> results = new List<DamageContainer>(1000);

        private void doDamage(float delta)
        {
            results.Clear();

            var hud = CoreRegistry.require<HUDSystem>();
            var em = CoreRegistry.require<EntityManager>();
            var pm = CoreRegistry.require<TestParticleSystem>();
            var assets = CoreRegistry.require<AssetManager>();
            var particleMat = assets.getMaterial("bullethell:particle");

            foreach (var id in em.getEntitiesByTag("bullet"))
            {
                var dc = new DamageContainer();
                dc.id = id;
                dc.isBullet = true;
                dc.physics = em.getComponent<PhysicsComponent>(id, "physics");
                dc.transform = em.getComponent<Transform>(id, "transform");
                if (dc.isValid)
                    results.Add(dc);
            }

            //var aabb = AxisAlignedBox.FromDimensions(Vector2.Zero, new Vector2(1024, 768));
            for (var i = 0; i < results.Count; i++)
            {
                var bullet = results[i];


                if (bullet.physics.collided)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        float speed = 14f * (1f - 1 / (random.NextFloat() * 5f + 1));
                        //var v = new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                        var theta = random.NextFloat() * MathHelper.TwoPi;// new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                        //v = v.Rotate(Vector2.Zero, MathHelper.PiOver2);
                        //v.Normalize();

                        //v *= speed;

                        bullet.destroyed = true;
                        em.destroy(bullet.id);
                        pm.CreateParticle(particleMat, bullet.transform.DerivedPosition - bullet.transform.DerivedForward * 10, Color.Yellow, 15, new Vector2(1f, 1f),
                            new ParticleState() { speed = speed, Type = ParticleType.Bullet, LengthMultiplier = 24f }, theta);

                    }
                    results[i] = bullet;
                }
            }

            foreach (var id in em.getEntitiesByTag("enemy"))
            {
                var dc = new DamageContainer();
                dc.id = id;
                dc.isBullet = false;
                dc.physics = em.getComponent<PhysicsComponent>(id, "physics");
                dc.transform = em.getComponent<Transform>(id, "transform");
                dc.health = em.getComponent<HealthComponent>(id, "health");
                if (dc.isValid)
                    results.Add(dc);
            }


            //shitty loop
            var objArr = results.ToArray();
            Utility.RadixSort(objArr);
            for (var i = 0; i < objArr.Length; i++)
            {
                if (objArr[i].destroyed)
                    continue;

                var p = objArr[i].transform.Position;
                var circle = new Circle(p, objArr[i].physics.radius);
                for (var k = i + 1; k < objArr.Length; k++)
                {
                    if (objArr[k].MinX > objArr[i].MaxX)
                        break;

                    if (objArr[k].destroyed)
                        continue;

                    if (objArr[i].isBullet == objArr[k].isBullet)
                        continue;
                    //overlaptests++;

                    var otherp = objArr[k].transform.Position;
                    var other = new Circle(otherp, objArr[k].physics.radius);
                    if (circle.Intersects(other))
                    {
                        var b = objArr[i].isBullet ? i : k;
                        var e = objArr[i].isBullet ? k : i;

                        objArr[b].destroyed = true;
                        em.destroy(objArr[b].id);

                        objArr[e].health.health--;
                        if (!objArr[e].health.isAlive())
                        {
                            var explosionforce = em.createRef(em.create());
                            explosionforce.addComponent("expire", new ExpireComponent() { timer = 0.25f });
                            explosionforce.addComponent("flee", new FleeComponent());
                            explosionforce.addComponent("transform", new Transform() { Position = objArr[b].transform.DerivedPosition });
                            hud._score += 10;
                            objArr[e].destroyed = true;
                            destroy(em, objArr[e].id, objArr[e].transform.DerivedPosition, particleMat, pm);

                        }
                        else
                        {
                            objArr[e].physics.velocity += objArr[b].physics.velocity * 5;
                        }

                        if (objArr[i].destroyed)
                            break;
                    }
                }
            }
        }

        private void destroy(EntityManager em, int id, Vector2 p, Material particleMat, TestParticleSystem pm)
        {

            float hue1 = random.NextFloat() * 6;
            float hue2 = (hue1 + random.NextFloat() * 2) % 6f;
            Color color1 = Utility.HSVToColor(hue1, 0.5f, 1);
            Color color2 = Utility.HSVToColor(hue2, 0.5f, 1);
            Color color = Color.Lerp(color1, color2, random.NextFloat());
            //color.A = 0;
            em.destroy(id);

            for (int j = 0; j < 3; j++)
            {
                float speed = 9f * (1f - 1 / (random.NextFloat() * 10f + 1));
                var theta = random.NextFloat() * MathHelper.TwoPi;// new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                //var v = new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                //v.Normalize();
                //v *= speed;
                var state = new ParticleState()
                {
                    speed = speed,
                    Type = ParticleType.Enemy,
                    LengthMultiplier = 32f
                };


                pm.CreateParticle(particleMat, p, color, 120, new Vector2(0.25f, 1.5f), state, theta);
            }
        }

        private void createBullet(EntityManager em, WeaponComponent wc, Vector2 p, float rotation)
        {
            var bullet = em.createRef(em.create());
            var physics = bullet.addComponent("physics", new PhysicsComponent());
            physics.radius = 2f;
            physics.speed = 10;
            physics.maxForce = 5.4f;
            physics.mass = 0;
            physics.velocity = new Vector2(Utility.Cos(rotation), Utility.Sin(rotation)) * wc.velocity;
            physics.drag = 0.95f;

            var sprite = bullet.addComponent("sprite", new Sprite());
            sprite.texture = wc.material;
            sprite.color = Color.Yellow;

            var transform = bullet.addComponent("transform", new Transform());
            transform.Position = p;
            transform.Orientation = rotation + MathHelper.PiOver2;

            var expire = bullet.addComponent<ExpireComponent>("expire", new ExpireComponent());
            expire.timer = 1f; 

            bullet.tag("bullet");

        }

        public void init()
        {
            _map = CoreRegistry.require<Map>();
        }

        public void shutdown()
        {
        }
    }
}
