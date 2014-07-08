using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Atma;
using Atma.Engine;
using Atma.Managers;

namespace GameName1.BulletHell.Scripts
{
    public class PlayerStatus : Script
    {
        private int _lives = 3;
        private int _score = 0;
        private int _multiplier = 1;
        private bool waitingForRespawn = true;

        private void reset()
        {
            _lives = 3;
            _score = 0;
            _multiplier = 1;
        }

        private void respawn()
        {
            _multiplier = 1;
            waitingForRespawn = false;
        }

        private void update()
        {
            var input = CoreRegistry.require<InputManager>(InputManager.Uri);
            if (input.IsRightMouseDown && waitingForRespawn)
            {
                if (_lives < 0)
                    rootObject.broadcast("reset");

                rootObject.broadcast("respawn");
            }

            if (input.IsKeyDown(Keys.Space))
                rootObject.broadcast("playerdead");
        }

        private void ongui()
        {
            var gui = CoreRegistry.require<GUIManager>(GUIManager.Uri);
            gui.label(new Vector2(300, 0), string.Format("lives: {0}", _lives));
            gui.label(new Vector2(500, 0), string.Format("score: {0}", _score));
            gui.label(new Vector2(700, 0), string.Format("multi: {0}", _multiplier));

            if (waitingForRespawn)
            {
                if (_lives < 0)
                    gui.label(new Vector2(400, 400), 2, string.Format("game over! right click to start over!"));
                else
                    gui.label(new Vector2(400, 400), 2, string.Format("right click to respawn!"));
            }

        }

        private void removeEntity(Entity entity)
        {
            if (!waitingForRespawn)
                _score += 10 * _multiplier;
        }

        private void playerdead()
        {
            if (!waitingForRespawn)
            {
                _lives--;

                if (_lives < 0)
                {
                    rootObject.broadcast("gameover");
                }
                    
                waitingForRespawn = true;
            }
        }
    }
}
