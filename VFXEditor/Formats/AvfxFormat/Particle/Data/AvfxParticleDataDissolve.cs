using VfxEditor.Formats.AvfxFormat.Base;
using VfxEditor.Formats.AvfxFormat.Components.Bool;
using VfxEditor.Formats.AvfxFormat.Components.Enums;
using VfxEditor.Formats.AvfxFormat.Components.Flag;
using VfxEditor.Formats.AvfxFormat.Components.Int;
using VfxEditor.Formats.AvfxFormat.Curve;
using static VfxEditor.Formats.AvfxFormat.Base.Enums;

namespace VfxEditor.Formats.AvfxFormat.Particle.Data {
    public class AvfxParticleDataDissolve : AvfxDataWithParameters {
        // All-new to dawntrail
        public readonly AvfxBool Reverse = new( "Reverse", "bRev" );
        public readonly AvfxEnum<DissolveShape> BST = new( "Base Shape Type", "BST" );
        public readonly AvfxInt NPT = new( "NPT (Unused)", "NPT" );
        public readonly AvfxFlag<AuraFilter> DissolveTarget = new( "Dissolve Target", "DTT" );
        public readonly AvfxCurve1Axis EroR = new( "EroR", "EroR" );
        public readonly AvfxCurve1Axis EndColorWidth = new( "End Color Width", "EdW" );
        public readonly AvfxCurveDissolve Color;
        public readonly AvfxCurve1Axis MidColorWidth = new( "Mid Color Width", "EdCW" );
        public readonly AvfxCurve1Axis StartColorWidth = new( "Start Color Width", "ECMP" );
        public readonly AvfxCurve1Axis Intensity = new( "Intensity", "Int" );

        public AvfxParticleDataDissolve( AvfxFile file ) : base() {
            Color = new( file, "Color", "EdC" );

            Parsed = GetFieldsInOrder<AvfxBase>( typeof( AvfxParticleDataDissolve ) );

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
