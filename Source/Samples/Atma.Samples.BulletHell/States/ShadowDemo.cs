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
    public class ShadowDemo : IGameState
    {
        private readonly static Logger logger = Logger.getLogger(typeof(InGameState));

        // private GraphicsSubsystem _graphics;
        private EntityManager _entity;
        private ComponentSystemManager _components;
        private GUIManager _gui;
        private WorldRenderer _world;
        private DisplayDevice _display;

        public void begin()
        {
            logger.info("begin");
            _display = this.display();
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            _entity = CoreRegistry.get<EntityManager>(EntityManager.Uri);
            _gui = CoreRegistry.put(GUIManager.Uri, new GUIManager());
            _gui.init();

            _world = CoreRegistry.put(WorldRenderer.Uri, new WorldRenderer());

            _components = CoreRegistry.put(ComponentSystemManager.Uri, new ComponentSystemManager());
            _components.register(TrackMouseSystem.Uri, new TrackMouseSystem());
            _components.register(SpriteComponentSystem.Uri, new SpriteComponentSystem());
            _components.register(ShapeRenderer.Uri, new ShapeRenderer());
            //_components.register(DebugSystem.Uri, new DebugSystem());

            _components.init();
            _world.init();


            var cursor = _entity.createRef(_entity.create());
            cursor.tag("cursor");
            cursor.addComponent("transform", new Transform()).Depth = 1f;
            cursor.addComponent("trackmouse", new MarkerComponent());

            var cursorSprite = cursor.addComponent("sprite", new Sprite());
            cursorSprite.material = assets.getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            cursorSprite.rotation = 0f;
            cursorSprite.origin = Vector2.Zero;
            


            var _playerGO = _entity.createRef(_entity.create());
            _entity.tag(_playerGO.id, "player");
            _playerGO.addComponent("transform", new Transform());
            _playerGO.addComponent("input", new InputComponent());
            _playerGO.addComponent("physics", new PhysicsComponent() { speed = 8, maxForce = 5.4f, radius = 10 });


            var playerSprite = _playerGO.addComponent("sprite", new Sprite());
            playerSprite.material = assets.getMaterial("bullethell:player");
            playerSprite.color = new Color(1f, 1f, 1f, 1f);
            var playerWeapon = _playerGO.addComponent<WeaponComponent>("weapon", new WeaponComponent());
            playerWeapon.material = assets.getMaterial("bullethell:bullet");

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

            var shape = _entity.create();
            var shaperef = _entity.createRef(shape);

            var shapecomponent = shaperef.addComponent("shape", new ShapeComponent());
            shapecomponent.shape = new Shape(verts);

            var transform = shaperef.addComponent("transform", new Transform());
            transform.Position = p;
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
            var p = Microsoft.Xna.Framework.Input.Mouse.GetState().Position;
            var vp = new Vector2(p.X, p.Y);
            var mp = _world.currentCamera.screenToWorld(vp);
            CoreRegistry.require<ShapeRenderer>(ShapeRenderer.Uri).target = mp;
            //var player = _entity.createRef(_entity.getEntityByTag("player"));
            //var transform = player.getComponent<Transform>("transform");

            //_world.currentCamera.position = transform.DerivedPosition;
            _components.update(dt);
        }

        public void input(float dt)
        {

        }

        public void render()
        {
            this.graphics().spritesRendered = 0;

            _display.prepareToRender();

            _world.render();
            //if (Atma.MonoGame.Graphics.MonoGL.instance != null)
            //    Atma.MonoGame.Graphics.MonoGL.instance.resetstatistics();
            //graphics.beginRender()
            //_components.render();
            _gui.render();
        }

    }
}
