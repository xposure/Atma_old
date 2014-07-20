using Atma.Assets;
using Atma.Engine;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Controllers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Rendering.Sprites;

namespace Atma.Samples.BulletHell.Systems
{
    public class EnemySpawnerSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:enemyspawner";

        private float spawnChance = 200;
        private Random random;
        private int spawnCount = 1000;
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        public void init()
        {
            random = new Random();
        }

        public void shutdown()
        {
        }

        public void update(float delta)
        {
            //return;
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;

                var em = CoreRegistry.require<EntityManager>();

                if (em.countEntitiesByTag("enemy") >= spawnCount)
                    return;

                var player = em.createRef(em.getEntityByTag("player"));

                if (!player.exists || !player.hasComponent("transform"))
                    return;

                var playerPosition = player.getComponent<Transform>("transform").DerivedPosition;

                if (random.Next(0f, spawnChance) < 0.5f)
                {
                    var assets = CoreRegistry.require<AssetManager>();

                    var p = GetSpawnPosition();
                    var enemies = (int)(random.Next(25, 35) * 200f / spawnChance);
                    for (var i = 0; i < enemies; i++)
                    {
                        var speed = random.NextFloat() * 20;
                        var v = new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                        v.Normalize();
                        v *= speed;

                        var id = em.create();
                        var enemyGO = em.createRef(id);

                        //var enemyGO = rootObject.createChild("enemy");
                        //var enemy = enemyGO.createScript<Enemy>();
                        enemyGO.addComponent("input", new InputComponent());
                        enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                        enemyGO.addComponent("physics", new PhysicsComponent());
                        enemyGO.addComponent("seperate", new SeperationComponent());
                        enemyGO.addComponent("health", new HealthComponent());

                        enemyGO.tag("enemy");
                        var sprite = enemyGO.addComponent("sprite", new Sprite());
                        sprite.color = Color.Orange;
                        sprite.material = assets.getMaterial("bullethell:enemy1");

                        var transform = enemyGO.addComponent("transform", new Transform());
                        transform.Position = p + v;

                        ////var enemyai = enemyGO.createScript<AIChaseEntity>();
                        ////enemyai.target = player;

                        //var enemySprite = enemyGO.createScript<Sprite>();
                        //enemySprite.color = Color.Orange;
                        //enemySprite.material = assets.getMaterial("bullethell:enemy1");
                        ////enemySprite.material.SetBlendState(BlendState.Additive);
                    }
                }

                if (random.Next(0f, spawnChance) < 0.5f)
                {
                    var assets = CoreRegistry.require<AssetManager>();
                    var enemies = (int)(random.Next(25, 35) * 200f / spawnChance);
                    var p = GetSpawnPosition();
                    for (var i = 0; i < enemies; i++)
                    {
                        var speed = random.NextFloat() * 20;
                        var v = new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                        v.Normalize();
                        v *= speed;
                        var id = em.create();
                        var enemyGO = em.createRef(id);

                        //var enemyGO = rootObject.createChild("enemy");
                        //var enemy = enemyGO.createScript<Enemy>();
                        enemyGO.addComponent("input", new InputComponent());
                        enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                        enemyGO.addComponent("physics", new PhysicsComponent());
                        enemyGO.addComponent("seperate", new SeperationComponent());
                        enemyGO.addComponent("health", new HealthComponent());
                        enemyGO.tag("enemy");
                        var sprite = enemyGO.addComponent("sprite", new Sprite());
                        sprite.color = Color.Orange;
                        sprite.material = assets.getMaterial("bullethell:enemy2");

                        var transform = enemyGO.addComponent("transform", new Transform());
                        transform.Position = p + v;
                    }
                }
                if (random.Next(0f, spawnChance) < 0.5f)
                {
                    var assets = CoreRegistry.require<AssetManager>();
                    var enemies = (int)(random.Next(25, 35) * 200f / spawnChance);
                    var p = GetSpawnPosition();
                    for (var i = 0; i < enemies; i++)
                    {
                        var speed = random.NextFloat() * 20;
                        var v = new Vector2(random.NextFloat() * 2 - 1, random.NextFloat() * 2 - 1);
                        v.Normalize();
                        v *= speed;
                        var id = em.create();
                        var enemyGO = em.createRef(id);

                        //var enemyGO = rootObject.createChild("enemy");
                        //var enemy = enemyGO.createScript<Enemy>();
                        enemyGO.addComponent("input", new InputComponent());
                        enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                        enemyGO.addComponent("physics", new PhysicsComponent());
                        enemyGO.addComponent("seperate", new SeperationComponent());
                        enemyGO.addComponent("health", new HealthComponent());
                        enemyGO.tag("enemy");
                        var sprite = enemyGO.addComponent("sprite", new Sprite());
                        sprite.color = Color.Orange;
                        sprite.material = assets.getMaterial("bullethell:enemy3");

                        var transform = enemyGO.addComponent("transform", new Transform());
                        transform.Position = p + v;
                    }
                }
                spawnChance -= 0.005f;
            }
        }

        private Vector2 GetSpawnPosition()
        {
            var p = -new Vector2(1024, 768) / 2f;
            switch (random.Next(0, 5))
            {
                case 0:
                    p += new Vector2(50, 50);
                    break;
                case 1:
                    p += new Vector2(1024 - 50, 768 - 50);
                    break;
                case 2:
                    p += new Vector2(1024 - 50, 50);
                    break;
                case 3:
                    p += new Vector2(50, 768 - 50);
                    break;
            }
            return p;

            //Vector2 pos;
            //do
            //{
            //    var screenSize = mainCamera.worldBounds;
            //    pos = new Vector2(random.Next(screenSize.X0, screenSize.X1), random.Next(screenSize.Y0, screenSize.Y1));
            //}
            //while (Vector2.DistanceSquared(pos, playerPosition) < 250 * 250);

            //return pos;
        }
    }
}
