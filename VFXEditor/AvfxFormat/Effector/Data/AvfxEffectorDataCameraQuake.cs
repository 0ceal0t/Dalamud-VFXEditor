using System;

namespace VfxEditor.AvfxFormat {
    public class AvfxEffectorDataCameraQuake : AvfxData {
        public readonly AvfxCurve Attenuation = new( "Attenuation", "Att" );
        public readonly AvfxCurve RadiusOut = new( "Radius Out", "RdO" );
        public readonly AvfxCurve RadiusIn = new( "Radius In", "RdI" );
        public readonly AvfxCurve3Axis Rotation = new( "Rotation", "Rot" );
        public readonly AvfxCurve3Axis Position = new( "Position", "Pos" );

        public AvfxEffectorDataCameraQuake() : base() {
            Parsed = new() {
                Attenuation,
                RadiusOut,
                RadiusIn,
                Rotation,
                Position
            };

            DisplayTabs.Add( Attenuation );
            DisplayTabs.Add( RadiusOut );
            DisplayTabs.Add( RadiusIn );
            DisplayTabs.Add( Rotation );
            DisplayTabs.Add( Position );
        }
    }
}
