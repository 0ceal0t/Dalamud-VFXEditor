using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxParticleDataPolygon : AvfxData {
        public readonly AvfxCurve Count = new( "Count", "Cnt" );

        public AvfxParticleDataPolygon() : base() {
            Parsed = new() {
                Count
            };

            DisplayTabs.Add( Count );
        }
    }
}
