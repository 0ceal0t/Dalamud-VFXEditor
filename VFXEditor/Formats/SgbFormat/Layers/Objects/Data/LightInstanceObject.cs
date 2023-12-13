using System.IO;
using VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class LightInstanceObject : SgbObject {
        private readonly ParsedEnum<LightType> LightType = new( "Type" );
        private readonly ParsedFloat Attenuation = new( "Attenuation" );
        private readonly ParsedFloat RangeRate = new( "Range Rate" );
        private readonly ParsedEnum<PointLightType> PointLightType = new( "Point Light Type" );
        private readonly ParsedFloat AttenuationConeCoefficient = new( "Attenuation Cone Coefficient" );
        private readonly ParsedFloat ConeDegree = new( "Cone Degree" );
        private readonly ParsedString Texture = new( "Texture" );
        private readonly ColorHDRI HDRI = new();
        private readonly ParsedByteBool FollowsDirectionalLight = new( "Follows Directional Light" );
        private readonly ParsedByteBool Specular = new( "Specular" );
        private readonly ParsedByteBool BgShadow = new( "Background Shadow" );
        private readonly ParsedByteBool CharacterShadow = new( "Character Shadow" );
        private readonly ParsedFloat ShadowClipRange = new( "Shadow Clip Range" );
        private readonly ParsedFloat2 PlaneLightRotation = new( "Plane Light Rotation" );
        private readonly ParsedShort MergeGroupId = new( "Merge Group Id" );

        public LightInstanceObject( LayerEntryType type ) : base( type ) { }

        public LightInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            LightType.Draw();
            Attenuation.Draw();
            RangeRate.Draw();
            PointLightType.Draw();
            AttenuationConeCoefficient.Draw();
            ConeDegree.Draw();
            Texture.Draw();
            HDRI.Draw();
            FollowsDirectionalLight.Draw();
            Specular.Draw();
            BgShadow.Draw();
            CharacterShadow.Draw();
            ShadowClipRange.Draw();
            PlaneLightRotation.Draw();
            MergeGroupId.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            LightType.Read( reader );
            Attenuation.Read( reader );
            RangeRate.Read( reader );
            PointLightType.Read( reader );
            AttenuationConeCoefficient.Read( reader );
            ConeDegree.Read( reader );
            Texture.Value = FileUtils.ReadStringOffset( startPos, reader );
            HDRI.Read( reader );
            FollowsDirectionalLight.Read( reader );
            reader.ReadBytes( 3 ); // padding
            Specular.Read( reader );
            BgShadow.Read( reader );
            CharacterShadow.Read( reader );
            reader.ReadByte(); // padding
            ShadowClipRange.Read( reader );
            PlaneLightRotation.Read( reader );
            MergeGroupId.Read( reader );
            reader.ReadByte(); // padding
        }
    }
}
