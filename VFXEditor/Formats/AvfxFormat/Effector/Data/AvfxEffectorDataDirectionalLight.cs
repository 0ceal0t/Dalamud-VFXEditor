using VFXEditor.Formats.AvfxFormat.Curve;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataDirectionalLight : AvfxData {
        public readonly AvfxCurveColor Ambient;
        public readonly AvfxCurveColor Color;
        public readonly AvfxCurve1Axis Power = new( "Power", "Pow" );
        public readonly AvfxCurve1Axis PowerRandom = new( "Power Random", "PowR" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot", CurveType.Angle );

        public AvfxEffectorDataDirectionalLight( AvfxFile file ) : base() {
            Ambient = new( file, "Ambient", "Amb" );
            Color = new( file, "Color" );

            Parsed = [
                Ambient,
                Color,
                Power,
                PowerRandom,
                Rotation
            ];

            Tabs.Add( Ambient );
            Tabs.Add( Color );
            Tabs.Add( Power );
            Tabs.Add( PowerRandom );
            Tabs.Add( Rotation );
        }
    }
}
