using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataPoint : AvfxData {
        public readonly AvfxCurve SpringStrength = new( "Spring Strength", "SpS" );

        public AvfxBinderDataPoint() : base() {
            Parsed = new() {
                SpringStrength
            };

            DisplayTabs.Add( SpringStrength );
        }
    }
}
