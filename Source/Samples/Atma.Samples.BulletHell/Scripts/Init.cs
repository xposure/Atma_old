
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Atma;
using Atma.Engine;
using Atma.Managers;
using Atma.Assets;

namespace GameName1.BulletHell.Scripts
{
    public class Init : Script
    {
        private void init()
        {
            var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

            //var t = assets.getTexture("bullethell:TEXTURE:1366856174");
            //var t = assets.getTexture("TEXTURE:bullethell:1366856174");
            //rootObject.createScript<DebugStats>();
            //gameObject.destroy();

            var m = assets.getMaterial("bullethell:test");

            rootObject.createScript<EnemyManager>();
            rootObject.createScript<WaveManager>();
            rootObject.createScript<PlayerStatus>();
            rootObject.createScript<PlayerManager>();
            rootObject.createScript<FPS>();

            var cameraGO = Root.instance.RootObject.createChild("camera");
            var camera = cameraGO.createScript<Camera>();
            camera.clear = Color.Black;

            var cameraSprite = cameraGO.createScript<Sprite>();
            cameraSprite.material = resources.createMaterialFromTexture("content/textures/cockpit1green.png");
            cameraSprite.offset = new Vector2(-1000, -1000);
        }
    }
}
