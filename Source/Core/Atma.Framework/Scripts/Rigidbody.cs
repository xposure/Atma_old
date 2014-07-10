using Atma.Collections;
using Atma.Engine;
using Atma.Graphics;
using Atma.Managers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Atma.Scripts
{
    public class Rigidbody : Script, IQuadObject
    {
        private static Material _material = null;

        public float maxDistanceAllowed = 5f;
        public float maxDistanceAllowedSq = 5f * 5f;

        public AxisAlignedBox Bounds
        {
            get
            {
                if (_collider != null)
                    return _collider.Bounds;

                var p = _transform.DerivedPosition;
                var hs = Vector2.One / 2f;
                return new AxisAlignedBox(p - hs, p + hs);
            }
        }

        private List<Collider2> triggers = new List<Collider2>();
        private List<Collider2> temptriggers = new List<Collider2>();

        public Vector2 speed = Vector2.Zero;
        private Collider2 _collider;
        private Transform _transform;
        //private 
        private List<AxisAlignedBox> collisionTiles = new List<AxisAlignedBox>();

        private void init()
        {
            //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
            //removed due to API changes
            //if (_material == null)
            //    _material = resources.findMaterial("basewhite");

            _transform = gameObject.transform2();
            _collider = gameObject.collider();

            if (_collider != null)
                _collider.hasRigidbody = true;

        }

        private void fixedupdate()
        {
            var originalSpeed = speed;
            var distRemaining = originalSpeed;

            collisionTiles.Clear();

            while (distRemaining.LengthSquared() > 0)
            {
                var travel = distRemaining;
                if (distRemaining.LengthSquared() > maxDistanceAllowedSq)
                    travel = originalSpeed.ToNormalized() * maxDistanceAllowed;

                distRemaining -= travel;

                for (var i = 0; i < 2; i++)
                {
                    _transform.Position += i == 0 ? new Vector2(travel.X, 0) : new Vector2(0, travel.Y);
                    _transform.UpdateFromParent();

                    var shape = _collider.shape;
                    temptriggers.Clear();
                    var corrections = 3;
                    while (corrections-- > 0)
                    {
                        Collider2 hit;
                        var mtv = getCollision(shape, temptriggers, out hit);
                        if (!mtv.intersects)
                            break;

                        _transform.Position += mtv.smallest.normal * (float)mtv.overlap;
                        if (!collisionTiles.Contains(hit.shape.bounds))
                            collisionTiles.Add(hit.shape.bounds);
                        this.gameObject.sendMessage("onCollision", hit, mtv);
                    }

                    foreach (var trigger in temptriggers)
                    {
                        var mtv = shape.intersects(trigger.shape);
                        if (mtv.intersects)
                        {
                            if (triggers.Contains(trigger))
                                trigger.gameObject.sendMessage("onTriggerStay", this);
                            else
                            {
                                triggers.Add(trigger);
                                trigger.gameObject.sendMessage("onTriggerEnter", this);
                            }
                        }
                        else if (triggers.Contains(trigger))
                        {
                            triggers.Remove(trigger);
                            trigger.gameObject.sendMessage("onTriggerExit", this);
                        }
                    }
                }
            }
        }

        public static MinimumTranslationVector getCollision(ICollidable shape, List<Collider2> triggers, out Collider2 hit)
        {
            hit = null;

            var results = Root.instance.physicsQuadTree2.Query(shape.bounds);
            if (results.Count > 0)
            {
                var overlap = double.MaxValue;
                var smallest = Axis.Zero;
                foreach (var r in results.Where(x => !x.hasRigidbody))
                {
                    var mtv = shape.intersects(r.shape);
                    if (mtv.intersects && r.isTrigger)
                    {
                        if (!triggers.Contains(r))
                            triggers.Add(r);
                    }
                    else if (mtv.intersects && mtv.overlap < overlap)
                    {
                        overlap = mtv.overlap;
                        smallest = mtv.smallest;
                        hit = r;
                    }
                }

                if (hit != null)
                    return new MinimumTranslationVector(smallest, overlap);
            }

            return MinimumTranslationVector.Zero;
        }

        private void render()
        {
            if (_transform != null)
            {
                var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
                graphics.DrawRect(0, null, this.Bounds.minVector, this.Bounds.maxVector, Color.Green);

                foreach (var index in collisionTiles)
                    graphics.DrawRect(16, null, index.Minimum, index.Maximum, Color.Red);
            }
        }
    }
}
