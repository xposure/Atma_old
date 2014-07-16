//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Atma.Samples.BulletHell.World.Generator
//{
//    public class Region : IRegion
//    {
//        public Region()
//        {
//            Rooms = new RoomCollection();
//        }

//        //private static LinkedList<Region> regions = new LinkedList<Region>();
//        public static Region Create(string name, int width, int height)
//        {
//            Region region = new Region();
//            region.Name = name;
//            region.Width = width;
//            region.Height = height;

//            //regions.AddFirst(r);
//            RegionDatabase.Instance.Insert(region);
//            return region;
//        }

//        public string Name { get; private set; }
//        public int Width { get; private set; }
//        public int Height { get; private set; }

//        public RoomCollection Rooms { get; private set; }

//        public Room GetRoom(int x, int y)
//        {
//            foreach (Room room in Rooms)
//                if (room.X == x && room.Y == y)
//                    return room;

//            throw new ArgumentOutOfRangeException("x, y");
//        }

//        //public static Region FindByID(long id)
//        //{
//        //    foreach (var r in regions)
//        //        if (r.ID == id)
//        //            return r;

//        //    return null;
//        //}


//    }
//}
