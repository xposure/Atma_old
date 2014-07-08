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

            _entity = CoreRegistry.get<EntityManager>(EntityManager.Uri);
            _gui = CoreRegistry.put(GUIManager.Uri, new GUIManager());
            _gui.init();

            _components = CoreRegistry.put(ComponentSystemManager.Uri, new ComponentSystemManager());
            _components.register(TrackMouseSystem.Uri, new TrackMouseSystem());

            //_components.register(PhysicsSystem.Uri, new PhysicsSystem());
            //_components.register(RenderSystem.Uri, new RenderSystem());

            _components.init();

            //var id = _entity.create();

            //var position = _entity.addComponent(id, "position", new Position());
            //position.x = 1f;
            //position.y = 1f;

            ////var velocity = _entity.addComponent(id, "velocity", new Velocity());
            ////velocity.x = 2;
            ////velocity.y = 1.5f;


            //var meshdata = new MeshData();
            //meshdata.vertices = new Vector3[] { new Vector3(-1, 1, 0), new Vector3(0, -1, 0), new Vector3(1, 1, 0) };
            //meshdata.colors = new Vector4[] { new Vector4(1, 1, 1, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 0, 1, 1) };
            //meshdata.indices = new ushort[] { 0, 1, 2 };

            //var materialdata = new MaterialData();
            //var tech = materialdata.add(new Technique(materialdata));
            //var pass = tech.add(new Pass(tech));
            //pass.setDepth(DepthState.alwaysPass);
            //pass.ambient = Color.Blue;

            ////materialdata.
            //var mesh = new Mesh()
            //{
            //    mesh = Assets.Assets.generateAsset(meshdata),
            //    material = Assets.Assets.generateAsset(materialdata)
            //};
            //_entity.addComponent(id, "mesh", mesh);
            ////OpenGL.GL.Vertex2(-1.0f, 1.0f);
            ////OpenGL.GL.Color3(Color.SpringGreen.r, Color.SpringGreen.g, Color.SpringGreen.b);
            ////OpenGL.GL.Vertex2(0.0f, -1.0f);
            ////OpenGL.GL.Color3(Color.Ivory.r, Color.Ivory.g, Color.Ivory.b);
            ////OpenGL.GL.Vertex2(1.0f, 1.0f);

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
            if (Atma.MonoGame.Graphics.MonoGL.instance != null)
                Atma.MonoGame.Graphics.MonoGL.instance.resetstatistics();
            //graphics.beginRender()
            _components.render();
            _gui.render();
        }

    }
}
