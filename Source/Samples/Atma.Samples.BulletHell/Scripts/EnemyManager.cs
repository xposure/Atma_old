using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Atma;
using Atma.Common;

namespace GameName1.BulletHell.Scripts
{
    public class EnemyManager : Script
    {
        private int count = 0;
        private Entity[] entities = new Entity[256];
        private List<Entity> _add = new List<Entity>();
        private List<Entity> _remove = new List<Entity>();

        private void addEntity(Entity entity)
        {
            //if (count == entities.Length)
            //    Array.Resize(ref entities, entities.Length * 3 / 2);

            //entities[count++] = entity;
            _add.Add(entity);
        }

        private void removeEntity(Entity entity)
        {
            //var indexof = indexOf(entity);
            //if (indexof == -1)
            //    throw new ArgumentOutOfRangeException("entity");

            //entities[indexof] = entities[count--];

            _remove.Add(entity);
        }

        private int indexOf(Entity entity)
        {
            if (count == 0)
                return -1;

            if (entities[0] == entity)
                return 0;
            
            var index = entities.Length >> 1;
            var sample = index;

            while (sample > 0)
            {
                var other = entities[index];
                if (other == null || entity.minX < other.minX)
                {
                    if (sample == 0)
                    {
                        if ((index & 1) == 1)
                            index--;
                        else
                            return -1;
                    }
                    sample >>= 1;
                    index -= sample;
                }
                else if (entity.minX > other.minX)
                {
                    if (sample == 0)
                    {
                        if ((index & 1) == 1)
                            index++;
                        else
                            return -1;
                    }
                    else
                    {
                        sample >>= 1;
                        index += sample;
                    }
                }
                else
                {
                    if (entity == other)
                        return index;

                    var start = index;
                    while (entity.minX == other.minX && --index >= 0)
                    {
                        other = entities[index];
                        if (entity == other)
                            return index;
                    }

                    index = start;
                    other = entities[index];
                    while (entity.minX == other.minX && ++index < count)
                    {
                        other = entities[index];
                        if (entity == other)
                            return index;
                    }

                    return -1;
                }
            }

            //if (entities[index] == entity)
            //    return index;

            return -1;
        }

        private void beforefixedupdate()
        {
            while (_remove.Count > 0)
            {
                var next = _remove[_remove.Count - 1];
                var index = indexOf(next);

                if (index == -1)
                    throw new ArgumentOutOfRangeException("index");

                if (_add.Count > 0)
                {
                    entities[index] = _add[_add.Count - 1];
                    _add.RemoveAt(_add.Count - 1);
                }

                _remove.RemoveAt(_remove.Count - 1);
            }

            while (_add.Count > 0)
            {
                if (count == entities.Length)
                    Array.Resize(ref entities, entities.Length * 2);

                entities[count++] = _add[_add.Count - 1];
                _add.RemoveAt(_add.Count - 1);
            }

            entities.quickSort(0, count - 1);
            for (var i = 0; i < count; i++)
            {
                entities[i].collisionIndex = i;
            }
        }

        private void checkCollision(Vector2 p, float radius, int index, List<Entity> results)
        {
            //var index = indexOf(entity);
            //if (index == -1)
            //    throw new ArgumentOutOfRangeException("entity");

            //return;
            var minX = p.X - ((int)radius >> 1);
            var maxX = p.X + ((int)radius >> 1);
            var r2 = radius * radius;
            var start = index;
            while (--index >= 0 && index > start - 5)
            {
                var other = entities[index];
                if (other.maxX < minX)
                    break;

                var or2 = other.radius * other.radius;
                var d = other.transform.DerivedPosition - p;
                if (d.LengthSquared() < r2 + or2)
                    results.Add(other);
            }

            index = start;
            while (++index < count && index < start + 5)
            {
                var other = entities[index];
                if (other.minX > maxX)
                    break;

                var or2 = other.radius * other.radius;
                var d = other.transform.DerivedPosition - p;
                if (d.LengthSquared() < r2 + or2)
                    results.Add(other);
            }

            //for (var i = 0; i < count; i++)
            //{
            //    var t2 = entities[i].radius * entities[i].radius;
            //    var d = (p - entities[i].transform.DerivedPosition).LengthSquared();
            //    if (d < r2 + t2)
            //        results.Add(entities[i]);

            //    if (results.Count >= max)
            //        break;
            //}
        }

        private void checkCollision(Entity entity, List<Entity> results)
        {
            //var index = indexOf(entity);
            //if (index == -1)
            //    throw new ArgumentOutOfRangeException("entity");

            //return;
            var index = entity.collisionIndex;

            var p = entity.transform.DerivedPosition;
            var minX = entity.minX;
            var maxX = entity.maxX;
            var r2 = entity.radius * entity.radius;
            var start = index;
            while (--index >= 0 && index > start - 5)
            {
                var other = entities[index];
                if (other.maxX < minX)
                    break;

                var or2 = other.radius * other.radius;
                var d = other.transform.DerivedPosition - p;
                if (d.LengthSquared() < r2 + or2)
                    results.Add(other);
            }

            index = start;
            while (++index < count && index < start + 5)
            {
                var other = entities[index];
                if (other.minX > maxX)
                    break;

                var or2 = other.radius * other.radius;
                var d = other.transform.DerivedPosition - p;
                if (d.LengthSquared() < r2 + or2)
                    results.Add(other);
            }

            //for (var i = 0; i < count; i++)
            //{
            //    var t2 = entities[i].radius * entities[i].radius;
            //    var d = (p - entities[i].transform.DerivedPosition).LengthSquared();
            //    if (d < r2 + t2)
            //        results.Add(entities[i]);

            //    if (results.Count >= max)
            //        break;
            //}
        }


        private void getAllEntities(List<Entity> results)
        {
            results.AddRange(entities);
        }


    }
}
