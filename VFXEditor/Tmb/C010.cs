using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiNET;

namespace VFXEditor.Tmb {
    // Animation
    public class C010 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 50;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;
        private float Unk_6 = 0;
        private string Path = "";
        private int Unk_7 = 0;

        public C010( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C010] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // ? ex: 50, 60
            Unk_3 = reader.ReadInt32(); // 0
            Unk_4 = reader.ReadInt32(); // 0
            Unk_5 = reader.ReadInt32(); // 0
            Unk_6 = reader.ReadSingle(); // ?

            var offset = reader.ReadInt32(); // offset: [C0101] + offset + 8 = animation
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = TmbFile.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            Unk_7 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x28;
        public override int GetStringSize() => Path.Length + 1;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = ( int )stringWriter.BaseStream.Position + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C010" );
            entryWriter.Write( 0x28 );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );

            entryWriter.Write( offset );
            TmbFile.WriteString( stringWriter, Path, true );

            entryWriter.Write( Unk_7 );
        }

        public override string GetName() => "Animation (C010)";

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
            ImGui.InputFloat( $"Uknown 6{id}", ref Unk_6 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            ImGui.InputInt( $"Uknown 7{id}", ref Unk_7 );
        }
    }
}
