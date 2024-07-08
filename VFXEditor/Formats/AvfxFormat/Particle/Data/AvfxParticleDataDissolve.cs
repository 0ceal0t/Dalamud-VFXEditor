namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDissolve : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxBool Reverse = new( "Reverse", "bRev" );
        public readonly AvfxInt BST = new( "BST", "BST" );
        public readonly AvfxInt NPT = new( "NPT", "NPT" );
        public readonly AvfxInt DTT = new( "DTT", "DTT" );
        public readonly AvfxCurve EroR = new( "EroR", "EroR" );
        public readonly AvfxCurve EndWidth = new( "End Width", "EdW" );
        public readonly AvfxCurveDissolve EndColor = new( "End Color", "EdC" );
        public readonly AvfxCurve EdCW = new( "EdCW", "EdCW" );
        public readonly AvfxCurve ECMP = new( "ECMP", "ECMP" );
        public readonly AvfxCurve Intensity = new( "Intensity", "Int" );

        public AvfxParticleDataDissolve() : base() {
            Parsed = [
                Reverse,
                BST,
                NPT,
                DTT,
                EroR,
                EndWidth,
                EndColor,
                EdCW,
                ECMP,
                Intensity
            ];

            ParameterTab.Add( Reverse );
            ParameterTab.Add( BST );
            ParameterTab.Add( NPT );
            ParameterTab.Add( DTT );

            Tabs.Add( EroR );
            Tabs.Add( EndWidth );
            Tabs.Add( EndColor );
            Tabs.Add( EdCW );
            Tabs.Add( ECMP );
            Tabs.Add( Intensity );
        }
    }
}
