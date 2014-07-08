using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Systems
{
    //Root.instance.addSystem(new TestSystem());
    //Root.instance.addEntity(new Test());

    public interface ITest : IEntity
    {
        string Name { get; }
    }

    public class Test : IEntity, ITest
    {
        public string Name { get { return "WOOT!"; } }
    }

    public class TestSystem : RequiresSystem
    {
        protected override void oninit()
        {
            require<Test>(x =>
            {
                //Console.WriteLine(x.Name);
            });
        }
    }
}
