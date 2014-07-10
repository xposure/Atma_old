using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Entity;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems.Phsyics
{
    public class PhysicsComponent : Component
    {
        public float speed = 4f;
        public Vector2 velocity;
        public float mass = 10f;
        public float maxForce = 2.4f;
        public float drag = 0.94f;
        public float radius = 20f;
        protected AxisAlignedBox _bounds = AxisAlignedBox.Null;
        //public AxisAlignedBox Bounds
        //{
        //    get
        //    {
        //        if (_needsUpdate)
        //            updateBounds();
        //        return _bounds;
        //    }
        //}


    }
}
