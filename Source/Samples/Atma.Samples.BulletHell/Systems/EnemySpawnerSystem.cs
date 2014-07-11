using Atma.Assets;
using Atma.Engine;
using Atma.Entity;
using Atma.Samples.BulletHell.Systems.Controllers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class EnemySpawnerSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:enemyspawner";

        private float spawnChance = 5;
        private Random random;

        public void init()
        {
            random = new Random();
        }

        public void shutdown()
        {
        }

        public void update(float delta)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            var player = em.createRef(em.getEntityByTag("player"));

            if (!player.exists || !player.hasComponent("transform"))
                return;

            var playerPosition = player.getComponent<Transform>("transform").DerivedPosition;

            if (random.Next(0f, spawnChance) < 0.5f)
            {
                var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

                var id = em.create();
                var enemyGO = em.createRef(id);

                //var enemyGO = rootObject.createChild("enemy");
                //var enemy = enemyGO.createScript<Enemy>();
                enemyGO.addComponent("input", new InputComponent());
                enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                enemyGO.addComponent("physics", new PhysicsComponent());
                enemyGO.addComponent("seperate", new SeperationComponent());
                var sprite = enemyGO.addComponent("sprite", new SpriteComponent());
                sprite.color = Color.Orange;
                sprite.material = assets.getMaterial("bullethell:enemy1");

                var transform = enemyGO.addComponent("transform", new Transform());
                transform.Position = GetSpawnPosition();

                ////var enemyai = enemyGO.createScript<AIChaseEntity>();
                ////enemyai.target = player;

                //var enemySprite = enemyGO.createScript<Sprite>();
                //enemySprite.color = Color.Orange;
                //enemySprite.material = assets.getMaterial("bullethell:enemy1");
                ////enemySprite.material.SetBlendState(BlendState.Additive);
            }

            if (random.Next(0f, spawnChance) < 0.5f)
            {
                var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

                var id = em.create();
                var enemyGO = em.createRef(id);

                //var enemyGO = rootObject.createChild("enemy");
                //var enemy = enemyGO.createScript<Enemy>();
                enemyGO.addComponent("input", new InputComponent());
                enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                enemyGO.addComponent("physics", new PhysicsComponent());
                enemyGO.addComponent("seperate", new SeperationComponent());
                var sprite = enemyGO.addComponent("sprite", new SpriteComponent());
                sprite.color = Color.Orange;
                sprite.material = assets.getMaterial("bullethell:enemy2");

                var transform = enemyGO.addComponent("transform", new Transform());
                transform.Position = GetSpawnPosition();
            }

            if (random.Next(0f, spawnChance) < 0.5f)
            {
                var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

                var id = em.create();
                var enemyGO = em.createRef(id);

                //var enemyGO = rootObject.createChild("enemy");
                //var enemy = enemyGO.createScript<Enemy>();
                enemyGO.addComponent("input", new InputComponent());
                enemyGO.addComponent("chase", new ChaseComponent() { target = player.id });
                enemyGO.addComponent("physics", new PhysicsComponent());
                enemyGO.addComponent("seperate", new SeperationComponent());
                var sprite = enemyGO.addComponent("sprite", new SpriteComponent());
                sprite.color = Color.Orange;
                sprite.material = assets.getMaterial("bullethell:enemy3");

                var transform = enemyGO.addComponent("transform", new Transform());
                transform.Position = GetSpawnPosition();
            }

            spawnChance -= 0.005f;
        }

        private Vector2 GetSpawnPosition()
        {
            return new Vector2(random.Next(0, 1024), random.Next(0, 1) * 768);
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
