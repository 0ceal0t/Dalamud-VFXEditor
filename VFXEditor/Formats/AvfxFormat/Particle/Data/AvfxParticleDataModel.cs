using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxParticleDataModel : AvfxData {
        public readonly AvfxInt ModelNumberRandomValue = new( "Model Number Random", "MNRv" );
        public readonly AvfxEnum<RandomType> ModelNumberRandomType = new( "Model Number Random Type", "MNRt" );
        public readonly AvfxInt ModelNumberRandomInterval = new( "Model Number Random Interval", "MNRi" );
        public readonly AvfxEnum<FresnelType> FresnelType = new( "Fresnel Type", "FrsT" );
        public readonly AvfxEnum<DirectionalLightType> DirectionalLightType = new( "Directional Light Type", "DLT" );
        public readonly AvfxEnum<PointLightType> PointLightType = new( "Point Light Type", "PLT" );
        public readonly AvfxBool IsLightning = new( "Is Lightning", "bLgt" );
        public readonly AvfxBool IsMorph = new( "Is Morph", "bShp" );
        public AvfxIntList ModelIdx = new( "Model Index", "MdNo", defaultValue: -1 );
        public readonly AvfxCurve AnimationNumber = new( "Animation Number", "NoAn" );
        public readonly AvfxCurve Morph = new( "Morph", "Moph" );
        public readonly AvfxCurve FresnelCurve = new( "Fresnel Curve", "FrC" );
        public readonly AvfxCurve3Axis FresnelRotation = new( "Fresnel Rotation", "FrRt", CurveType.Angle );
        public readonly AvfxCurveColor ColorBegin = new( name: "Color Begin", "ColB" );
        public readonly AvfxCurveColor ColorEnd = new( name: "Color End", "ColE" );

        public readonly UiNodeSelectList<AvfxModel> ModelSelect;
        public readonly UiDisplayList Display;

        public AvfxParticleDataModel( AvfxParticle particle ) : base() {
            Parsed = new() {
                ModelNumberRandomValue,
                ModelNumberRandomType,
                ModelNumberRandomInterval,
                FresnelType,
                DirectionalLightType,
                PointLightType,
                IsLightning,
                IsMorph,
                ModelIdx,
                AnimationNumber,
                Morph,
                FresnelCurve,
                FresnelRotation,
                ColorBegin,
                ColorEnd
            };

            DisplayTabs.Add( Display = new UiDisplayList( "Parameters" ) );
            Display.Add( ModelSelect = new UiNodeSelectList<AvfxModel>( particle, "Model", particle.NodeGroups.Models, ModelIdx ) );
            Display.Add( ModelNumberRandomValue );
            Display.Add( ModelNumberRandomType );
            Display.Add( ModelNumberRandomInterval );
            Display.Add( FresnelType );
            Display.Add( DirectionalLightType );
            Display.Add( PointLightType );
            Display.Add( IsLightning );
            Display.Add( IsMorph );

            DisplayTabs.Add( Morph );
            DisplayTabs.Add( FresnelCurve );
            DisplayTabs.Add( FresnelRotation );
            DisplayTabs.Add( ColorBegin );
            DisplayTabs.Add( ColorEnd );
            DisplayTabs.Add( AnimationNumber );
        }

        public override void Enable() => ModelSelect.Enable();

        public override void Disable() => ModelSelect.Disable();
    }
}
