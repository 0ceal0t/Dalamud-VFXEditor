using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataLinear : AvfxData {
        public readonly AvfxCurve CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

        public AvfxBinderDataLinear() : base() {
            Parsed = new() {
                CarryOverFactor,
                CarryOverFactorRandom
            };

            DisplayTabs.Add( CarryOverFactor );
            DisplayTabs.Add( CarryOverFactorRandom );
        }
    }
}
