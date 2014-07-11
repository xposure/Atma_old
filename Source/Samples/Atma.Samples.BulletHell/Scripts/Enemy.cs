using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Atma;
using Atma.Samples.BulletHell.Systems;

namespace GameName1.BulletHell.Scripts
{
    public class Enemy : Entity
    {
        public GameObject player;
        public Entity playerEntity;

        public float delay = 0.25f;
        private SpriteComponent _sprite;

        public bool IsActive { get { return delay <= 0; } }

        private void init()
        {
            _sprite = gameObject.add("sprite", new SpriteComponent());
            //_sprite = gameObject.getScript<Sprite>();
            rootObject.broadcast("addEntity", (Entity)this);
            player = rootObject.find("player");
            if (player != null)
                playerEntity = player.getScript<Entity>();
        }

        private void fixedupdate()
        {
            if (delay > 0f)
            {
                var time = Atma.Engine.CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
                delay -= time.delta;

                if (delay > 0f)
                {
                    var a = 1f - (delay / 0.25f);
                    _sprite.color = new Color(_sprite.color, a);
                }
                else
                    _sprite.color = new Color(_sprite.color, 1f);
            }
            else if (delay <= 0f)
            {
                if (playerEntity != null)
                {
                    var r2 = radius * radius;
                    var p2 = playerEntity.radius * playerEntity.radius;

                    var d = Vector2.DistanceSquared(player.transform.DerivedPosition, transform.DerivedPosition);

                    if (d < r2 + p2)
                    {

                        //rootObject.broadcast("playerdead");
                    }
                }

            }
        }

        private void destroy()
        {
            rootObject.broadcast("removeEntity", (Entity)this);
        }

        private void hit()
        {
            gameObject.destroy();
        }

        private void playerdead()
        {
            hit();
        }
    }
}
