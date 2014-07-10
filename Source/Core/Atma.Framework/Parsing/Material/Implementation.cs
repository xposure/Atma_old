using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Core;
using Atma.Engine;

namespace Atma.Parsing.Material
{
    public partial class Parser
    {
        internal static Logger logger = Logger.getLogger(typeof(Parser));
        protected MaterialData _material;


        public MaterialData parse()
        {
            _material = new MaterialData();
            this.Parse();

            return _material;
        }

    }

    public partial class Scanner
    {

    }
}
