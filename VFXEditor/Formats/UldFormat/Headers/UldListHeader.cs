using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Headers {
    public class UldListHeader : UldGenericHeader {
        public readonly uint ElementCount = 0;
        private readonly ParsedInt Unk1 = new( "Unknown 1" );

        public UldListHeader( string identifier, string version ) : base( identifier, version ) { }

        public UldListHeader( BinaryReader reader ) : base( reader ) {
            ElementCount = reader.ReadUInt32();
            Unk1.Read( reader );
        }

        public long Write<T>( BinaryWriter writer, List<T> items, long offsetsPosition ) {
            if( items.Count == 0 ) return 0;

            var offset = writer.BaseStream.Position - offsetsPosition;

            WriteHeader( writer );
            writer.Write( items.Count );
            Unk1.Write( writer );

            return offset;
        }

        public void Draw() {
            Unk1.Draw();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
        }
    }
}
