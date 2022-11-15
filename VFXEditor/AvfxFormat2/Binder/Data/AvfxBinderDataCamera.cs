namespace VfxEditor.AvfxFormat2 {
    public class AvfxBinderDataCamera : AvfxData {
        public readonly AvfxCurve Distance = new( "Distance", "Dst" );
        public readonly AvfxCurve DistanceRandom = new( "Distance Random", "DstR" );

        public AvfxBinderDataCamera() : base() {
            Parsed = new() {
                Distance,
                DistanceRandom
            };

            DisplayTabs.Add( Distance );
            DisplayTabs.Add( DistanceRandom );
        }
    }
}
