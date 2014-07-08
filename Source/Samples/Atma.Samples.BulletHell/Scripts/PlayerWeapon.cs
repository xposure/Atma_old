using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Atma;
using Atma.Collections;
using Atma.Resources;
using Atma.Managers;
using Atma.Engine;
using Atma.Assets;
using Atma.Graphics;

namespace GameName1.BulletHell.Scripts
{
    public class PlayerWeapon : Script
    {
        public struct Bullet
        {
            public float damage;
            public float energy;
            public float totalEnergy;
            public float fadeOut;
            public float totalFadeOut;
            public float velocity;
            public float rotation;
            public Vector2 position;
            public Vector2 direction;
        }

        public Color color = Color.LightBlue;
        public float fireRate = 0.1f;
        public float fireTimer = 0f;
        public float damage = 1f;
        public float velocity = 10f;
        public float energy = 5f;

        private Material _material;
        private ObjectPool<Bullet> _bulletPool = new ObjectPool<Bullet>();
        private List<int> _activeBullets = new List<int>();
        private List<Entity> collisionCache = new List<Entity>(10);

        private void init()
        {
            var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            var data = new MaterialData();
            data.SetBlendState(BlendState.Additive);
            _material = resources.createMaterialFromTexture("content/textures/bullethell/bullet.png", data);
           // _material.SetBlendState(BlendState.Additive);
        }

        private void update()
        {
            var input = CoreRegistry.require<InputManager>(InputManager.Uri);
            if (fireTimer <= 0f && input.IsLeftMouseDown)
            {
                fireTimer = fireRate;
                //createBullet(transform.DerivedPosition, transform.DerivedOrientation);
                //if (targetFilters != TargetFilters.Aliens)
                {
                    var randomSpread = (random.Next(-0.04f, 0.04f) + random.Next(-0.04f, 0.04f)) / 2f;
                    createBullet(transform.DerivedPosition - transform.DerivedRight * 12 + transform.DerivedForward  * 24, transform.DerivedOrientation + randomSpread);
                    createBullet(transform.DerivedPosition + transform.DerivedRight * 12 + transform.DerivedForward * 24, transform.DerivedOrientation + randomSpread);
                }
            }
        }

        private void fixedupdate()
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            if (fireTimer > 0f)
                fireTimer -= time.delta;
           
            for (int i = _activeBullets.Count - 1; i >= 0 && _activeBullets.Count > 0; i--)
            {
                var index = _activeBullets[i];
                var bullet = _bulletPool[index];

                if (bullet.energy > 0f)
                    bullet.energy -= time.delta;
                else if (bullet.fadeOut > 0f)
                    bullet.fadeOut -= time.delta;

                if (bullet.energy <= 0 && bullet.fadeOut <= 0)
                {
                    _bulletPool.FreeItem(index);
                    _activeBullets[i] = _activeBullets[_activeBullets.Count - 1];
                    _activeBullets.RemoveAt(_activeBullets.Count - 1);
                    //i++;
                }
                else
                {
                    bullet.position += bullet.direction * bullet.velocity;

                    collisionCache.Clear();
                    rootObject.broadcast("checkCollision", bullet.position, 10f, 1, collisionCache);

                    if (collisionCache.Count > 0)
                    {
                        foreach (var e in collisionCache)
                        {
                            e.gameObject.broadcast("hit");

                            _bulletPool.FreeItem(index);
                            _activeBullets[i] = _activeBullets[_activeBullets.Count - 1];
                            _activeBullets.RemoveAt(_activeBullets.Count - 1);
                            //i++;
                        }
                    }
                    ////check for collision
                    //var target = _targets.get(bullet.position, 40, targetFilters).FirstOrDefault();
                    //if (target != null)
                    //{
                    //    bullet.energy = 0;
                    //    bullet.fadeOut = 0;
                    //    _bulletPool.FreeItem(index);
                    //    _activeBullets[i] = _activeBullets[_activeBullets.Count - 1];
                    //    _activeBullets.RemoveAt(_activeBullets.Count - 1);
                    //    target.broadcast("takedamage", damage);
                    //    var scrolling = rootObject.getScript<ScrollingText>();
                    //    scrolling.emit(bullet.position, damage.ToString());

                    //}
                }

                _bulletPool[index] = bullet;
            }
        }

        private void render()
        {
            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                var index = _activeBullets[i];
                var bullet = _bulletPool[index];

                graphics.Draw(_material, bullet.position, bullet.rotation + MathHelper.PiOver2, new Vector2(8, 24),  color);
            }
        }


        private void createBullet(Vector2 p, float rotation)
        {

            var index = _bulletPool.GetFreeItem();
            _activeBullets.Add(index);

            var bullet = _bulletPool[index];
            bullet.position = p;
            bullet.rotation = rotation;
            bullet.direction = new Vector2(Utility.Cos(rotation), Utility.Sin(rotation));
            //bullet.energy = 0.5f;
            //bullet.totalEnergy = 0.5f;
            //bullet.fadeOut = 0.1f;
            //bullet.totalFadeOut = 0.1f;
            //bullet.velocity = 10f;
            bullet.energy = energy;
            bullet.totalEnergy = energy;
            bullet.fadeOut = 0.1f;
            bullet.totalFadeOut = 0.1f;
            bullet.velocity = velocity;
            bullet.damage = damage;

            _bulletPool[index] = bullet;
        }

    }
}
