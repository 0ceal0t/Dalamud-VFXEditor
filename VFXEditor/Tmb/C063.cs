using System.IO;
using ImGuiNET;

namespace VFXEditor.Tmb {
    // Sound
    public class C063 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 1;
        private int Unk_3 = 0;
        private string Path = "";
        private int Unk_4 = 1;
        private int Unk_5 = 0;

        public static readonly string Name = "Sound (C063)";

        public C063() { }
        public C063( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C063] + 8

            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 1
            Unk_3 = reader.ReadInt32(); // 0

            var offset = reader.ReadInt32(); // path offset: [C063] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = TmbFile.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            Unk_4 = reader.ReadInt32(); // 1
            Unk_5 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x20;
        public override int GetStringSize() => Path.Length + 1;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = ( int )stringWriter.BaseStream.Position + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C063" );
            entryWriter.Write( 0x20 );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );
            TmbFile.WriteString( stringWriter, Path, true );

            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
        }
    }
}
