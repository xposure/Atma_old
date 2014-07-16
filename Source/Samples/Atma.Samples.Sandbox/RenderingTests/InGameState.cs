using Atma.Entity;
using Atma.Systems;
using Atma.Core;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Managers;
using Atma.Common.Components;
using Atma.Assets;
using Microsoft.Xna.Framework;
using Atma.Graphics;
using Atma.Rendering;

namespace Atma.Samples.Sandbox.RenderingTests
{
    public class InGameState : IGameState
    {
        private readonly static Logger logger = Logger.getLogger(typeof(InGameState));

        // private GraphicsSubsystem _graphics;
        private EntityManager _entity;
        private DisplayDevice _display;
        private ComponentSystemManager _components;
        private GUIManager _gui;
        private WorldRenderer _world;

        public void begin()
        {
            logger.info("begin");
            //_graphics = CoreRegistry.require<GraphicsSubsystem>(GraphicsSubsystem.Uri);

            _display = this.display();

            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            _entity = CoreRegistry.get<EntityManager>(EntityManager.Uri);
            _gui = CoreRegistry.put(GUIManager.Uri, new GUIManager());
            _gui.init();

            _world = CoreRegistry.put(WorldRenderer.Uri, new WorldRenderer());
            _components = CoreRegistry.put(ComponentSystemManager.Uri, new ComponentSystemManager());

            _components.register("twod:rendersystem", new RenderSystem());
            _components.init();
            _world.init();


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
            if (!_gui.input())
                _components.input();
        }

        public void render()
        {
            _display.prepareToRender();

            _world.render();
            //_components.render();
            _gui.render();
        }

    }
}
