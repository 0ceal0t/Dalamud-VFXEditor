using VFXEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataDissolve : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxBool Reverse = new( "Reverse", "bRev" );
        public readonly AvfxInt BST = new( "BST", "BST" );
        public readonly AvfxInt NPT = new( "NPT", "NPT" );
        public readonly AvfxFlag<AuraFilter> DissolveTarget = new( "Dissolve Target", "DTT" );
        public readonly AvfxCurve1Axis EroR = new( "EroR", "EroR" );
        public readonly AvfxCurve1Axis EndColorWidth = new( "End Color Width", "EdW" );
        public readonly AvfxCurveDissolve Color = new( "Color", "EdC" );
        public readonly AvfxCurve1Axis MidColorWidth = new( "Mid Color Width", "EdCW" );
        public readonly AvfxCurve1Axis StartColorWidth = new( "Start Color Width", "ECMP" );
        public readonly AvfxCurve1Axis Intensity = new( "Intensity", "Int" );

        public AvfxParticleDataDissolve() : base() {
            Parsed = [
                Reverse,
                BST,
                NPT,
                DissolveTarget,
                EroR,
                EndColorWidth,
                Color,
                MidColorWidth,
                StartColorWidth,
                Intensity
            ];

            ParameterTab.Add( Reverse );
            ParameterTab.Add( BST );
            ParameterTab.Add( NPT );
            ParameterTab.Add( DissolveTarget );

            Tabs.Add( EroR );
            Tabs.Add( EndColorWidth );
            Tabs.Add( Color );
            Tabs.Add( MidColorWidth );
            Tabs.Add( StartColorWidth );
            Tabs.Add( Intensity );
        }
    }
}
