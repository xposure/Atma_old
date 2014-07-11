
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Atma;
using Atma.Engine;
using Atma.Managers;
using Atma.Assets;
using Atma.Common.Components;

namespace GameName1.BulletHell.Scripts
{
    public class Init : Script
    {
        private void init()
        {
            //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

            //var cursorGO = rootObject.createChild("cursor");
            //var trackMouse = cursorGO.add("trackmouse", new MarkerComponent());

            //var cursorSprite = cursorGO.createScript<Sprite>();
            //cursorSprite.material = assets.getMaterial("bullethell:cursor"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            //cursorSprite.rotation = 0f;
            //cursorSprite.origin = Vector2.Zero;

            //var t = assets.getTexture("bullethell:TEXTURE:1366856174");
            //var t = assets.getTexture("TEXTURE:bullethell:1366856174");
            //rootObject.createScript<DebugStats>();
            //gameObject.destroy();

            //var m = assets.getMaterial("bullethell:additive");

            //rootObject.createScript<EnemyManager>();
            //rootObject.createScript<WaveManager>();
            rootObject.createScript<PlayerStatus>();
            rootObject.createScript<PlayerManager>();
            rootObject.createScript<FPS>();

            //var cameraGO = Root.instance.RootObject.createChild("camera");
            //var camera = cameraGO.createScript<Camera>();
            //camera.clear = Color.Black;

            //var cameraSprite = cameraGO.createScript<Sprite>();
            //cameraSprite.material = assets.getMaterial("bullethell:cockpit1green");// resources.createMaterialFromTexture("content/textures/cockpit1green.png");
            //cameraSprite.offset = new Vector2(-1000, -1000);
        }
    }
}
