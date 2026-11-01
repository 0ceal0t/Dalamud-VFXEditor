using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.Formats.AvfxFormat.Binder.Data {
    public class AvfxBinderDataCamera : AvfxData {
        public readonly AvfxCurve1Axis Distance = new( "Distance", "Dst" );
        public readonly AvfxCurve1Axis DistanceRandom = new( "Distance Random", "DstR" );

        public AvfxBinderDataCamera() : base() {
            Parsed = [
                Distance,
                DistanceRandom
            ];

            Tabs.Add( Distance );
            Tabs.Add( DistanceRandom );
        }
    }
}
