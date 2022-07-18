using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    public class C002 : TMBItem {
        private int Duration = 50;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private string Path = "";

        public static readonly string DisplayName = "TMB (C002)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C002";

        public C002() { }
        public C002( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C002] + 8

            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // offset: [C002] + offset + 8 = tmb?
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override int GetSize() => 0x1C;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( offset );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}
