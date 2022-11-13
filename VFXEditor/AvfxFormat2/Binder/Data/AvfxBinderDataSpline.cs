using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataSpline : AvfxData {
        public readonly AvfxCurve CarryOverFactor = new( "Carry Over Factor", "COF" );
        public readonly AvfxCurve CarryOverFactorRandom = new( "Carry Over Factor Random", "COFR" );

        public AvfxBinderDataSpline() : base() {
            Children = new() {
                CarryOverFactor,
                CarryOverFactorRandom
            };

            Tabs.Add( CarryOverFactor );
            Tabs.Add( CarryOverFactorRandom );
        }
    }
}
