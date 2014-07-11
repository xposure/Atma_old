//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Atma;
//using Microsoft.Xna.Framework.Graphics;
//using Atma.Engine;
//using Atma.Managers;
//using Atma.Graphics;
//using Atma.Assets;
//using Atma.Samples.BulletHell.Systems.Controllers;
//using Atma.Samples.BulletHell.Systems.Phsyics;


//namespace GameName1.BulletHell.Scripts
//{
//    public class WaveManager : Script
//    {
//        private float spawnChance = 5;

//        private int entityCount = 0;
//        private bool waitingForRespawn = true;

//        private void respawn()
//        {
//            waitingForRespawn = false;
//        }

//        private void playerdead()
//        {
//            waitingForRespawn = true;
//        }

//        private void fixedupdate()
//        {
//            if (!waitingForRespawn && entityCount < 200)
//            {
//                var player =  rootObject.find("player");
//                var playerPosition = player.transform.DerivedPosition;

//                if (random.Next(0f, spawnChance) < 0.5f)
//                {
//                    var enemyGO = rootObject.createChild("enemy");
//                    var enemy = enemyGO.createScript<Enemy>();
//                    enemyGO.add("input", new InputComponent());
//                    enemyGO.add("chase", new ChaseComponent() { target = player.id });
//                    enemyGO.add("physics", new PhysicsComponent());
//                    enemyGO.add("seperate", new SeperationComponent());
//                    //var enemyai = enemyGO.createScript<AIChaseEntity>();
//                    //enemyai.target = player;

//                    var enemySprite = enemyGO.createScript<Sprite>();
//                    enemySprite.color = Color.Orange;
//                    var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
//                    enemySprite.material = assets.getMaterial("bullethell:enemy1");
//                    //enemySprite.material.SetBlendState(BlendState.Additive);
//                    enemyGO.transform.Position = GetSpawnPosition(playerPosition);
//                }

//                if (random.Next(0f, spawnChance) < 0.5f)
//                {
//                    var enemyGO = rootObject.createChild("enemy");
//                    var enemy = enemyGO.createScript<Enemy>();

//                    enemyGO.add("input", new InputComponent());
//                    enemyGO.add("chase", new ChaseComponent() { target = player.id });
//                    enemyGO.add("physics", new PhysicsComponent());
//                    enemyGO.add("seperate", new SeperationComponent());

//                    var enemySprite = enemyGO.createScript<Sprite>();
//                    enemySprite.color = Color.Orange;
//                    var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
//                    enemySprite.material = assets.getMaterial("bullethell:enemy2");
//                    //enemySprite.material.SetBlendState(BlendState.Additive);
//                    enemyGO.transform.Position = GetSpawnPosition(playerPosition);
//                }

//                if (random.Next(0f, spawnChance) < 0.5f)
//                {
//                    //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
//                    var enemyGO = rootObject.createChild("enemy");
//                    var enemy = enemyGO.createScript<Enemy>();

//                    enemyGO.add("input", new InputComponent());
//                    enemyGO.add("chase", new ChaseComponent() { target = player.id });
//                    enemyGO.add("physics", new PhysicsComponent());
//                    enemyGO.add("seperate", new SeperationComponent());

//                    var enemySprite = enemyGO.createScript<Sprite>();
//                    enemySprite.color = Color.Orange;
//                    var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
//                    enemySprite.material = assets.getMaterial("bullethell:enemy3");

//                    //enemySprite.material.SetBlendState(BlendState.Additive);
//                    enemyGO.transform.Position = GetSpawnPosition(playerPosition);
//                }

//                spawnChance -= 0.005f;
//            }
//        }

//        private Vector2 GetSpawnPosition(Vector2 playerPosition)
//        {
//            Vector2 pos;
//            do
//            {
//                var screenSize = mainCamera.worldBounds;
//                pos = new Vector2(random.Next(screenSize.X0, screenSize.X1), random.Next(screenSize.Y0, screenSize.Y1));
//            }
//            while (Vector2.DistanceSquared(pos, playerPosition) < 250 * 250);

//            return pos;
//        }

//        private void reset()
//        {
//            entityCount = 0;
//            spawnChance = 60;
//        }

//        private void addEntity(Entity entity)
//        {
//            entityCount++;
//        }

//        private void removeEntity(Entity entity)
//        {
//            entityCount--;
//        }
//    }
//}
