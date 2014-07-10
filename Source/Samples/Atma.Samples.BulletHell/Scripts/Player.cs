using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atma;
using Atma.Managers;
using Atma.Engine;
using Atma.Common.Components;
using Atma.Assets;

namespace GameName1.BulletHell.Scripts
{
    public class Player : Entity
    {

        private void init()
        {
            //_material = resources.createMaterialFromTexture("content/textures/bullethell/player.png");
            //_material.SetBlendState(BlendState.Additive);
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            radius = 10;

            gameObject.createScript<PlayerMovement>();

            var cursorGO = rootObject.createChild("cursor");
            var trackMouse = cursorGO.add("trackmouse", new MarkerComponent());
            //var trackMouse = cursorGO.createScript<TrackMouse>();
            var cursorSprite = cursorGO.createScript<Sprite>();
            var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            cursorSprite.material = assets.getMaterial("bullethell:cursor"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            cursorSprite.rotation = 0f;
            cursorSprite.origin = Vector2.Zero;

            gameObject.createScript<PlayerWeapon>();
        }

        private void update()
        {
            var input = CoreRegistry.require<InputManager>(InputManager.Uri);
            var wp = mainCamera.screenToWorld(input.MousePosition);
            transform.LookAt(wp);
        }
    }
}
