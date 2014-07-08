using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Assets.Sources
{
    public class NullSource : AbstractSource
    {
        public NullSource(string id)
            : base(id)
        {

        }

        protected override void load()
        {
            //addEntry
        }
    }
}
