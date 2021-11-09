using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiNET;

namespace VFXEditor.Tmb {
    public class C002 : TmbItem {
        private short Time;
        private int Unk_2;
        private int Unk_3;
        private int Unk_4;
        private string Path;

        public C002( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C002] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // ? : 50
            Unk_3 = reader.ReadInt32(); // 0
            Unk_4 = reader.ReadInt32(); // 0

            var offset = reader.ReadInt32(); // offset: [C002] + offset + 8 = tmb?
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = TmbFile.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override int GetSize() => 0x1C;
        public override int GetStringSize() => Path.Length + 1;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos, ref short id ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = ( int )stringWriter.BaseStream.Position + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C002" );
            entryWriter.Write( 0x1C );
            entryWriter.Write( id++ );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );

            entryWriter.Write( offset );
            TmbFile.WriteString( stringWriter, Path, true );
        }

        public override string GetName() => "C002";

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
        }
    }
}
