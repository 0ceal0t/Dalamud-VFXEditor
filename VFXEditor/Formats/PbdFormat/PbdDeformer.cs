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
            reader.ReadUInt16(); // connection index
            var offset = reader.ReadInt32();
            Scale.Read( reader );
        }

        public void Draw() {
            SkeletonId.Draw();
            Scale.Draw();
        }
    }
}
