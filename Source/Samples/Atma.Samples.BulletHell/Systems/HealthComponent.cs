﻿using Atma.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class HealthComponent : Component
    {
        public int health = 10;

        public bool isAlive()
        {
            return health > 0;
        }
    }
}
