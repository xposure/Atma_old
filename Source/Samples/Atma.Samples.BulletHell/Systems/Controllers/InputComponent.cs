using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Phsyics;
using GameName1.BulletHell;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems.Controllers
{
    public class InputComponent : Component
    {
        public Vector2 thrust = Vector2.Zero;

        public Vector2 fireDirection = Vector2.Zero;
        public bool fireWeapon = false;
        public bool disabled = false;
        public bool speedBurst = false;
    }
}
