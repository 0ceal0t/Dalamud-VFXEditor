using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class TargetMarkerInstanceObject : SgbObject {
        private readonly ParsedFloat NamePlateOffset = new( "Nameplate Offset" );
        private readonly ParsedEnum<TargetMarkerType> TargetMarkerType = new( "Type" );

        public TargetMarkerInstanceObject( LayerEntryType type ) : base( type ) { }

        public TargetMarkerInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            NamePlateOffset.Draw();
            TargetMarkerType.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            NamePlateOffset.Read( reader );
            TargetMarkerType.Read( reader );
        }
    }
}
