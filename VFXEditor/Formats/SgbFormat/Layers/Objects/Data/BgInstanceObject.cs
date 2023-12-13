using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class BgInstanceObject : SgbObject {
        private readonly ParsedString AssetPath = new( "Path" );
        private readonly ParsedString CollisionPath = new( "Collision Path" );
        private readonly ParsedEnum<ModelCollisionType> CollisionType = new( "Collision Type" );
        private readonly ParsedUInt AttributeMask = new( "Attribute Mask" );
        private readonly ParsedUInt Attribute = new( "Attribute" );
        private readonly ParsedUInt CollisionConfig = new( "Collision Config" );
        private readonly ParsedByteBool Visible = new( "Visible" );
        private readonly ParsedByteBool RenderShadow = new( "Render Shadow" );
        private readonly ParsedByteBool RenderLightShadow = new( "Render Light Shadow" );
        private readonly ParsedFloat RenderModelClipRange = new( "Render Model Clip Range" );

        public BgInstanceObject( LayerEntryType type ) : base( type ) { }

        public BgInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            AssetPath.Draw();
            CollisionPath.Draw();
            CollisionType.Draw();
            AttributeMask.Draw();
            Attribute.Draw();
            CollisionConfig.Draw();
            Visible.Draw();
            RenderShadow.Draw();
            RenderLightShadow.Draw();
            RenderModelClipRange.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            AssetPath.Value = FileUtils.ReadStringOffset( startPos, reader );
            CollisionPath.Value = FileUtils.ReadStringOffset( startPos, reader );
            CollisionType.Read( reader );
            AttributeMask.Read( reader );
            Attribute.Read( reader );
            CollisionConfig.Read( reader );
            Visible.Read( reader );
            RenderShadow.Read( reader );
            RenderLightShadow.Read( reader );
            reader.ReadByte(); // padding
            RenderModelClipRange.Read( reader );
        }
    }
}
