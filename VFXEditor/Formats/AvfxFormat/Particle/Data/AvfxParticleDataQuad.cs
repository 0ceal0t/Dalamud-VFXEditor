namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataQuad : AvfxDataWithParameters {
        public readonly AvfxInt SS = new( "Scaling Scale", "SS" );

        public AvfxParticleDataQuad() : base( true ) {
            Parsed = [
                SS
            ];

            ParameterTab.Add( SS );
        }
    }
}
