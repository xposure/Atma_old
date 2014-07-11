using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entity;
using Atma.Graphics;
using Atma.Samples.BulletHell.Systems.Controllers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems
{
    public class WeaponComponent : Component
    {
        public float fireRate = 0.01f;
        public float fireTimer = 0f;
        public float damage = 1f;
        public float velocity = 10f;
        public float spread = 0.4f;
        public Material material;
        //public float energy = 5f;
    }

    public class WeaponSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:weapon";

        private Random random = new Random();

        public void update(float delta)
        {
            doBullets(delta);
            doDamage(delta);
        }

        private void doBullets(float delta)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
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

                    var randomSpread = (random.Next(-weapon.spread, weapon.spread) + random.Next(-weapon.spread, weapon.spread)) / 2f;
                    createBullet(em, weapon, transform.DerivedPosition - transform.DerivedRight * 12 + transform.DerivedForward * 24, dir.GetRotation() + randomSpread);
                    createBullet(em, weapon, transform.DerivedPosition + transform.DerivedRight * 12 + transform.DerivedForward * 24, dir.GetRotation() + randomSpread);
                }
            }
        }

        private struct DamageContainer : IRadixKey
        {
            public PhysicsComponent physics;
            public Transform transform;
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

            var hud = CoreRegistry.require<HUDSystem>(HUDSystem.Uri);
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
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

            foreach (var id in em.getEntitiesByTag("enemy"))
            {
                var dc = new DamageContainer();
                dc.id = id;
                dc.isBullet = false;
                dc.physics = em.getComponent<PhysicsComponent>(id, "physics");
                dc.transform = em.getComponent<Transform>(id, "transform");
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
                        objArr[i].destroyed = true;
                        objArr[k].destroyed = true;
                        em.destroy(objArr[i].id);
                        em.destroy(objArr[k].id);
                        hud._score += 10;
                        break;
                    }
                }
            }
        }

        private void createBullet(EntityManager em, WeaponComponent wc, Vector2 p, float rotation)
        {
            var bullet = em.createRef(em.create());
            var physics = bullet.addComponent("physics", new PhysicsComponent());
            physics.radius = 10f;
            physics.speed = 10;
            physics.maxForce = 5.4f;
            physics.mass = 0;
            physics.velocity = new Vector2(Utility.Cos(rotation), Utility.Sin(rotation)) * wc.velocity;
            physics.drag = 1f;

            var sprite = bullet.addComponent("sprite", new SpriteComponent());
            sprite.material = wc.material;
            sprite.color = Color.Blue;

            var transform = bullet.addComponent("transform", new Transform());
            transform.Position = p;
            transform.Orientation = rotation;

            var expire = bullet.addComponent<ExpireComponent>("expire", new ExpireComponent());
            expire.timer = 5f;

            bullet.tag("bullet");
            //        graphics.Draw(_material, bullet.position, bullet.rotation + MathHelper.PiOver2, new Vector2(8, 24), color);


            //var index = _bulletPool.GetFreeItem();
            //_activeBullets.Add(index);

            //var bullet = _bulletPool[index];
            //bullet.position = p;
            //bullet.rotation = rotation;
            //bullet.direction = new Vector2(Utility.Cos(rotation), Utility.Sin(rotation));
            ////bullet.energy = 0.5f;
            ////bullet.totalEnergy = 0.5f;
            ////bullet.fadeOut = 0.1f;
            ////bullet.totalFadeOut = 0.1f;
            ////bullet.velocity = 10f;
            //bullet.energy = energy;
            //bullet.totalEnergy = energy;
            //bullet.fadeOut = 0.1f;
            //bullet.totalFadeOut = 0.1f;
            //bullet.velocity = velocity;
            //bullet.damage = damage;

            //_bulletPool[index] = bullet;
        }

        public void init()
        {
        }

        public void shutdown()
        {
        }
    }
}
