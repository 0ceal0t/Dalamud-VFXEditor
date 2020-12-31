using AVFXLib.AVFX;
using AVFXLib.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public abstract class LiteralBase: Base
    {
        // remember to pad out avfx name to 4
        public int Size { get; set; }

        public LiteralBase(string jsonPath, string avfxName, int size) : base(jsonPath, avfxName)
        {
            Size = size;
        }

        public abstract void read(AVFXLeaf node);

        public abstract string stringValue();
    }
}
