using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.CutbFormat.Resource {
    public class CutbResource : IUiItem {
        public readonly ParsedString Path = new( "Path" );
        public readonly ParsedByte LOD = new( "LOD" );

        public CutbResource() { }

        public CutbResource( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position;
            var offset = reader.ReadInt32();
            LOD.Read( reader );
            reader.ReadBytes( 3 ); // padding

            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path.Read( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public void Draw() {
            Path.Draw( CommandManager.Cutb );
            LOD.Draw( CommandManager.Cutb );
        }
    }
}
