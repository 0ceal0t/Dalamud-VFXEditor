using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public abstract class AVFXBinderData : Base
    {
        public const string NAME = "Data";
        public AVFXBinderData(string avfxName) : base(avfxName)
        {

        }
    }
}
