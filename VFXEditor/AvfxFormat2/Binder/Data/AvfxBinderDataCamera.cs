using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataCamera : AvfxData {
        public readonly AvfxCurve Distance = new( "Distance", "Dst" );
        public readonly AvfxCurve DistanceRandom = new( "Distance Random", "DstR" );

        public AvfxBinderDataCamera() : base() {
            Children = new() {
                Distance,
                DistanceRandom
            };

            Tabs.Add( Distance );
            Tabs.Add( DistanceRandom );
        }
    }
}
