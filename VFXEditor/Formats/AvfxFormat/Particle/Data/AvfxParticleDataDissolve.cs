using System;

namespace VfxEditor.AvfxFormat {
    [Flags]
    public enum DissolveFilter {
        Character = 0x01,
        Weapon = 0x02,
        OffHand = 0x04
    }

    public class AvfxParticleDataDissolve : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxBool Reverse = new( "Reverse", "bRev" );
        public readonly AvfxInt BST = new( "BST", "BST" );
        public readonly AvfxInt NPT = new( "NPT", "NPT" );
        public readonly AvfxFlag<DissolveFilter> DissolveTarget = new( "Dissolve Target", "DTT" );
        public readonly AvfxCurve EroR = new( "EroR", "EroR" );
        public readonly AvfxCurve EndColorWidth = new( "End Color Width", "EdW" );
        public readonly AvfxCurveDissolve Color = new( "Color", "EdC" );
        public readonly AvfxCurve MidColorWidth = new( "Mid Color Width", "EdCW" );
        public readonly AvfxCurve StartColorWidth = new( "Start Color Width", "ECMP" );
        public readonly AvfxCurve Intensity = new( "Intensity", "Int" );

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
