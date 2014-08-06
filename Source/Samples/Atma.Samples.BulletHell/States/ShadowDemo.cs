using Atma.Entities;
using Atma.Core;
using Atma.Engine;
using Atma.Samples.BulletHell.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Managers;
using Atma.Samples.BulletHell.Systems.Controllers;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Assets;
using Microsoft.Xna.Framework;
using Atma.Rendering;
using Atma.Graphics;
using Atma.Rendering.Sprites;
using Atma.Samples.BulletHell.World;

namespace Atma.Samples.BulletHell.States
{
    public class ShadowDemo : GameSystem, IGameState
    {
        private GUIManagerOld _gui;
        private GameWorld _world;

        public void begin()
        {
            logger.info("begin");
            _gui = CoreRegistry.put( new GUIManagerOld());
            _gui.init();

            _world = CoreRegistry.put(new GameWorld());

            CoreRegistry.put( new ComponentSystemManager());
            components.register(new TrackMouseSystem());
            components.register( new TestParticleSystem());
            components.register( new SpriteComponentSystem());
            components.register( new ShapeRenderer());
            //components.register(DebugSystem.Uri, new DebugSystem());

            components.init();
            _world.init();


            var cursor = entities.createRef(entities.create());
            cursor.tag("cursor");
            cursor.addComponent("transform", new Transform()).Depth = 1f;
            cursor.addComponent("trackmouse", new MarkerComponent());

            //var cursorSprite = cursor.addComponent("sprite", new Sprite());
            //cursorSprite.material = assets.getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            //cursorSprite.rotation = 0f;
            //cursorSprite.origin = Vector2.Zero;

            var _floorGO = entities.createRef(entities.create());
            _floorGO.addComponent("transform", new Transform());
            var floorSprite = _floorGO.addComponent("sprite", new Sprite());
            floorSprite.texture = assets.getTexture("bullethell:floor");
            floorSprite.size = new Vector2(512, 512);
            floorSprite.size = new Vector2(1024,768);
            floorSprite.origin = new Vector2(0.5f, 0.5f);



            var _playerGO = entities.createRef(entities.create());
            entities.tag(_playerGO.id, "player");
            _playerGO.addComponent("transform", new Transform());
            _playerGO.addComponent("input", new InputComponent());
            _playerGO.addComponent("physics", new PhysicsComponent() { speed = 8, maxForce = 5.4f, radius = 10 });


            var playerSprite = _playerGO.addComponent("sprite", new Sprite());
            playerSprite.texture = assets.getTexture("bullethell:player");
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            var playerWeapon = _playerGO.addComponent<WeaponComponent>("weapon", new WeaponComponent());
            playerWeapon.material = assets.getTexture("bullethell:bullet");

            createShape(new Vector2(-300, -20), 25);
            var ang = random.NextFloat() * MathHelper.TwoPi;// random.NextFloat() * MathHelper.TwoPi;
            for (var i = 0; i < 10; i++)
            {

                var p = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * (random.Next(100, 500));
                ang += (float)(MathHelper.TwoPi / 10);
                ang = ang % (float)MathHelper.TwoPi;
                createShape(p, 50);
            }
            //createShape(new Vector2(40, -40), 25);
            //createShape(new Vector2(25, 500), 25);
            //createShape(new Vector2(-40, -40), 25);
            //createShape(new Vector2(-40, -40), 25);
        }

        private Random random = new Random(1235);
        private void createShape(Vector2 p, float size)
        {
            var ang = random.NextFloat() * MathHelper.TwoPi;// random.NextFloat() * MathHelper.TwoPi;
            var verts = new Vector2[random.Next(3, 5)];

            for (var i = 0; i < verts.Length; i++)
            {
                verts[i] = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * (random.Next(10, 50) + size) + p;
                ang += (float)(MathHelper.TwoPi / verts.Length);
                ang = ang % (float)MathHelper.TwoPi;
            }

            var shape = entities.create();
            var shaperef = entities.createRef(shape);

            var shapecomponent = shaperef.addComponent("shape", new ShapeComponent());
            shapecomponent.shape = new Shape(verts);

            var transform = shaperef.addComponent("transform", new Transform());
            transform.Position = p;
        }

        public void end()
        {
            logger.info("end");
            components.shutdown();
            entities.clear();
            CoreRegistry.clear();
        }

        public void update(float dt)
        {
            var p = Microsoft.Xna.Framework.Input.Mouse.GetState().Position;
            var vp = new Vector2(p.X, p.Y);
            var mp = _world.currentCamera.screenToWorld(vp);
            CoreRegistry.require<ShapeRenderer>().target = mp;
            //var player = _entity.createRef(_entity.getEntityByTag("player"));
            //var transform = player.getComponent<Transform>("transform");

            //_world.currentCamera.position = transform.DerivedPosition;
            components.update(dt);
        }

        public void input(float dt)
        {

        }

        public void render()
        {
            graphics.resetStatistics();

            display.prepareToRender();

            _world.render();
            //if (Atma.MonoGame.Graphics.MonoGL.instance != null)
            //    Atma.MonoGame.Graphics.MonoGL.instance.resetstatistics();
            //graphics.beginRender()
            //components.render();
            _gui.render();
        }

    }
}
