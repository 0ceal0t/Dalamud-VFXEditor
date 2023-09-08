using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataWindmill : AvfxData {
        public readonly AvfxEnum<WindmillUVType> WindmillUVType = new( "Windmill UV Type", "WUvT" );

        public readonly UiDisplayList Display;

        public AvfxParticleDataWindmill() : base() {
            Parsed = new() {
                WindmillUVType
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( WindmillUVType );
        }
    }
}
