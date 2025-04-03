using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
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
