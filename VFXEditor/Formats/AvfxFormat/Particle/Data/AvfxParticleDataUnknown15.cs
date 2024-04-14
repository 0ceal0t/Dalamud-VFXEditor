namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataUnknown15 : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxBool bRev = new( "Rev Enabled", "bRev" );
        public readonly AvfxInt BST = new( "BST", "BST" );
        public readonly AvfxInt NPT = new( "NPT", "NPT" );
        public readonly AvfxInt DTT = new( "DTT", "DTT" );
        public readonly AvfxCurve EroR = new( "EroR", "EroR" );
        public readonly AvfxCurve EdW = new( "EdW", "EdW" );
        public readonly AvfxCurveUnknown15 EdC = new( "EdC", "EdC" );
        public readonly AvfxCurve EdCW = new( "EdCW", "EdCW" );
        public readonly AvfxCurve ECMP = new( "ECMP", "ECMP" );
        public readonly AvfxCurve Int = new( "Int", "Int" );

        public AvfxParticleDataUnknown15() : base() {
            Parsed = [
                bRev,
                BST,
                NPT,
                DTT,
                EroR,
                EdW,
                EdC,
                EdCW,
                ECMP,
                Int
            ];

            ParameterTab.Add( bRev );
            ParameterTab.Add( BST );
            ParameterTab.Add( NPT );
            ParameterTab.Add( DTT );

            Tabs.Add( EroR );
            Tabs.Add( EdW );
            Tabs.Add( EdC );
            Tabs.Add( EdCW );
            Tabs.Add( ECMP );
            Tabs.Add( Int );
        }
    }
}
