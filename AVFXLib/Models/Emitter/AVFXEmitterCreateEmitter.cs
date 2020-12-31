using AVFXLib.AVFX;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models
{
    public class AVFXEmitterCreateEmitter : AVFXEmitterCreateParticle // how are these different than ItPr ? I have no idea
    {
        public new const string NAME = "ItEm";

        public AVFXEmitterCreateEmitter() : base()
        {
        }

        public override AVFXNode toAVFX()
        {
            AVFXNode res = base.toAVFX();
            res.Name = NAME;
            return res;
        }
    }
}
