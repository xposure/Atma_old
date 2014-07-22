﻿using Atma.Entities;
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
    public class Player
    {

    }

    public class InGameState : GameSystem, IGameState
    {
        private GUIManager _gui;
        private GameWorld _world;

        public void begin()
        {
            logger.info("begin");
            //_graphics = CoreRegistry.require<GraphicsSubsystem>(GraphicsSubsystem.Uri);
            _gui = CoreRegistry.put(new GUIManager());
            _gui.init();

            _world = CoreRegistry.put(new GameWorld());

            CoreRegistry.put( new ComponentSystemManager());
            components.register( new ExpirationSystem());
            components.register( new TrackMouseSystem());
            components.register( new ChaseController());
            components.register(new MoveController());
            components.register(new FleeController());
            components.register( new SeperationController());
            components.register(new PlayerController());
            components.register( new PhysicsSystem());
            components.register(new EnemySpawnerSystem());
            components.register(new WeaponSystem());
            components.register( new Map());
            components.register(new ShapeRenderer());
            components.register(new SpriteComponentSystem());
            components.register(new TestParticleSystem());
            components.register(new HUDSystem());
            components.register(new DebugSystem());

            //components.register(PhysicsSystem.Uri, new PhysicsSystem());
            //components.register(RenderSystem.Uri, new RenderSystem());
            components.init();
            _world.init();

            //var _floorGO = entities.createRef(entities.create());
            //_floorGO.addComponent("transform", new Transform());
            //var floorSprite = _floorGO.addComponent("sprite", new Sprite());
            //floorSprite.material = assets.getMaterial("bullethell:floor");
            //floorSprite.size = new Vector2(512, 512);
            //floorSprite.size = new Vector2(1024, 768);
            //floorSprite.origin = new Vector2(0.5f, 0.5f);


            var cursor = entities.createRef(entities.create());
            cursor.tag("cursor");
            cursor.addComponent("transform", new Transform());
            cursor.addComponent("trackmouse", new MarkerComponent());

            var cursorSprite = cursor.addComponent("sprite", new Sprite());
            cursorSprite.material = assets.getMaterial("bullethell:cursor"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            cursorSprite.origin = Vector2.Zero;


            var _playerGO = entities.createRef(entities.create());
            entities.tag(_playerGO.id, "player");
            _playerGO.addComponent("transform", new Transform());
            _playerGO.addComponent("input", new InputComponent());
            _playerGO.addComponent("physics", new PhysicsComponent() { speed = 4, maxForce = 8.4f, radius = 10, drag = 0.8f });


            var playerSprite = _playerGO.addComponent("sprite", new Sprite());
            playerSprite.material = assets.getMaterial("bullethell:player");
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            playerSprite.rotation = MathHelper.PiOver2;
            
            var playerWeapon = _playerGO.addComponent<WeaponComponent>("weapon", new WeaponComponent());
            playerWeapon.material = assets.getMaterial("bullethell:bullet");


            //createShape(new Vector2(-300, -20), 25);
            //var ang = random.NextFloat() * MathHelper.TwoPi;// random.NextFloat() * MathHelper.TwoPi;
            //for (var i = 0; i < 10; i++)
            //{

            //    var p = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)) * (random.Next(100, 500));
            //    ang += (float)(MathHelper.TwoPi / 10);
            //    ang = ang % (float)MathHelper.TwoPi;
            //    createShape(p, 50);
            //}
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
            _world.shutdown();
            logger.info("end");
            components.shutdown();
            entities.clear();
            CoreRegistry.clear();
        }

        public void update(float dt)
        {
            var player = entities.createRef(entities.getEntityByTag("player"));
            var transform = player.getComponent<Transform>("transform");

            var shaperenderer = CoreRegistry.require<ShapeRenderer>();
            shaperenderer.target = transform.DerivedPosition;

            _world.currentCamera.position = transform.DerivedPosition;


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
