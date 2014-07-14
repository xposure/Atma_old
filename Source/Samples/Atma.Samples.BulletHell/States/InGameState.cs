using Atma.Entity;
using Atma.Systems;
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
using Atma.Common.Components;
using Atma.Assets;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.States
{
    public class InGameState : IGameState
    {
        private readonly static Logger logger = Logger.getLogger(typeof(InGameState));

        // private GraphicsSubsystem _graphics;
        private EntityManager _entity;
        private ComponentSystemManager _components;
        private GUIManager _gui;

        public void begin()
        {
            logger.info("begin");
            //_graphics = CoreRegistry.require<GraphicsSubsystem>(GraphicsSubsystem.Uri);

            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            _entity = CoreRegistry.get<EntityManager>(EntityManager.Uri);
            _gui = CoreRegistry.put(GUIManager.Uri, new GUIManager());
            _gui.init();

            _components = CoreRegistry.put(ComponentSystemManager.Uri, new ComponentSystemManager());
            _components.register(ExpirationSystem.Uri, new ExpirationSystem());
            _components.register(TrackMouseSystem.Uri, new TrackMouseSystem());
            _components.register(ChaseController.Uri, new ChaseController());
            _components.register(MoveController.Uri, new MoveController());
            _components.register(FleeController.Uri, new FleeController());
            _components.register(SeperationController.Uri, new SeperationController());
            _components.register(PlayerController.Uri, new PlayerController());
            _components.register(PhysicsSystem.Uri, new PhysicsSystem());
            _components.register(EnemySpawnerSystem.Uri, new EnemySpawnerSystem());
            _components.register(WeaponSystem.Uri, new WeaponSystem());
            _components.register(SpriteRenderer.Uri, new SpriteRenderer());
            _components.register(CameraSystem.Uri, new CameraSystem());
            _components.register(TestParticleSystem.Uri, new TestParticleSystem());
            _components.register(HUDSystem.Uri, new HUDSystem());
            _components.register(DebugSystem.Uri, new DebugSystem());

            //_components.register(PhysicsSystem.Uri, new PhysicsSystem());
            //_components.register(RenderSystem.Uri, new RenderSystem());

            _components.init();

            var cameraEntity = _entity.createRef(_entity.create());
            var camera = cameraEntity.addComponent("camera", new CameraComponent() { clear = Color.Black });
            camera.init(cameraEntity.addComponent<Transform>("transform", new Transform()));

            var cursor = _entity.createRef(_entity.create());
            cursor.tag("cursor");
            cursor.addComponent("transform", new Transform());
            cursor.addComponent("trackmouse", new MarkerComponent());

            var cursorSprite = cursor.addComponent("sprite", new SpriteComponent());
            cursorSprite.material = assets.getMaterial("bullethell:cursor"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            cursorSprite.rotation = 0f;
            cursorSprite.origin = Vector2.Zero;


            var _playerGO = _entity.createRef(_entity.create());
            _entity.tag(_playerGO.id, "player");
            _playerGO.addComponent("transform", new Transform());
            _playerGO.addComponent("input", new InputComponent());
            _playerGO.addComponent("physics", new PhysicsComponent() { speed = 8, maxForce = 5.4f, radius = 10 });
            
            
            var playerSprite = _playerGO.addComponent("sprite", new SpriteComponent());
            playerSprite.material = assets.getMaterial("bullethell:player");
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            var playerWeapon = _playerGO.addComponent<WeaponComponent>("weapon", new WeaponComponent());
            playerWeapon.material = assets.getMaterial("bullethell:bullet");
        }

        public void end()
        {
            logger.info("end");
            _components.shutdown();
            _entity.clear();
            CoreRegistry.clear();
        }

        public void update(float dt)
        {
            _components.update(dt);
        }

        public void input(float dt)
        {

        }

        public void render()
        {
            //if (Atma.MonoGame.Graphics.MonoGL.instance != null)
            //    Atma.MonoGame.Graphics.MonoGL.instance.resetstatistics();
            //graphics.beginRender()
            _components.render();
            _gui.render();
        }

    }
}
