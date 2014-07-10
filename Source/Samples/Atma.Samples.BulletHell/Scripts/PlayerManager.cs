using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Managers;
using Atma.Assets;

namespace GameName1.BulletHell.Scripts
{
    public class PlayerManager : Script
    {
        private GameObject _playerGO;

        private void reset()
        {
        }

        private void respawn()
        {
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            _playerGO = rootObject.createChild("player");

            var player = _playerGO.createScript<Player>();
            var playerSprite = _playerGO.createScript<Sprite>();
            var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            playerSprite.material = assets.getMaterial("bullethell:player"); //resources.createMaterialFromTexture("content/textures/bullethell/player.png");
        }

        private void playerdead()
        {
            _playerGO.destroy();
        }
    }
}
