namespace VfxEditor.AvfxFormat {
    public class AvfxBinderDataCamera : AvfxData {
        public readonly AvfxCurve Distance = new( "Distance", "Dst" );
        public readonly AvfxCurve DistanceRandom = new( "Distance Random", "DstR" );

        public AvfxBinderDataCamera() : base() {
            Parsed = [
                Distance,
                DistanceRandom
            ];

            DisplayTabs.Add( Distance );
            DisplayTabs.Add( DistanceRandom );
        }
    }
}
