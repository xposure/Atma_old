using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Entities;
using Atma.Rendering.Sprites;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Entities
{
    public class Barrel : GameSystem
    {
        public EntityRef gameObject;
        public Transform transform;
        public Sprite sprite;

        public static Barrel create(EntityManager em)
        {
            var x = new Barrel();
            x.gameObject = em.createRef(em.create());
            
            x.transform = x.gameObject.addComponent<Transform>("transform", new Transform());
            
            x.sprite = x.gameObject.addComponent<Sprite>("sprite", new Sprite());
            x.sprite.texture = x.assets.getTexture("bullethell:bullethell/barrel_3");
            x.sprite.size = new Vector2(32, 64);

            return x;
        }
    }

    public static class BarrelExtension
    {
        public static Barrel createBarrel(this EntityManager em)
        {
            return Barrel.create(em);
        }
    }
}
