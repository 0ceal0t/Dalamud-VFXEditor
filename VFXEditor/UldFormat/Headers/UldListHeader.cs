using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Headers {
    public class UldListHeader : UldGenericHeader {
        public uint ElementCount;
        private readonly ParsedInt Unk1 = new( "Unknown 1" );

        public UldListHeader( BinaryReader reader ) : base( reader ) {
            ElementCount = reader.ReadUInt32();
            Unk1.Read( reader );
        }

        public void Write( BinaryWriter writer, uint elementCount ) {
            ElementCount = elementCount;

            WriteHeader( writer );
            writer.Write( ElementCount );
            Unk1.Write( writer );
        }

        public void Draw( string id ) {
            Unk1.Draw( id, CommandManager.Uld );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 1 );
        }
    }
}
