using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PbdFormat {
    public class PbdDeformer : IUiItem {
        public readonly ParsedShort SkeletonId = new( "Skeleton Id" );
        public readonly ParsedFloat Scale = new( "Scale" );

        public PbdDeformer() { }

        public PbdDeformer( BinaryReader reader ) : this() {
            SkeletonId.Read( reader );
            var treeIndex = reader.ReadUInt16();
            var offset = reader.ReadInt32();
            Scale.Read( reader );
            Dalamud.Log( $"Deformer: {treeIndex} {offset:X4} | {reader.BaseStream.Position:X4} | {SkeletonId.Value}" );
        }

        public void Draw() {

        }
    }
}
