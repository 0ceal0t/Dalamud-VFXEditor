using AVFXLib.AVFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVFXLib.Models {
    public class AVFXEmitterCreateEmitter : AVFXEmitterCreateParticle // how are these different than ItPr ? I have no idea
    {
        public new const string NAME = "ItEm";

        public AVFXEmitterCreateEmitter() : base() {
        }

        public override AVFXNode ToAVFX() {
            var res = base.ToAVFX();
            res.Name = NAME;
            return res;
        }
    }
}
