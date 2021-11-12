using System.Collections.Generic;
using System.IO;
using ImGuiNET;

namespace VFXEditor.Tmb.Item {
    public class C002 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 50;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private string Path = "";

        public static readonly string Name = "C002";

        public C002() { }
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
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            TmbFile.WriteString( entryWriter, "C002" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( offset );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}
