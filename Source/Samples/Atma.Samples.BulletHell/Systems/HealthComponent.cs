using Atma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class HealthComponent : Component
    {
        public int health = 1;

        public bool isAlive()
        {
            return health > 0;
        }
    }
}
