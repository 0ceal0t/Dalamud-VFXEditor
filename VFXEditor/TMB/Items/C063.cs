using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class C063 : TMBItem {
        private int Unk_2 = 1;
        private int Unk_3 = 0;
        private string Path = "";
        private int SoundIndex = 1;
        private int Unk_5 = 0;

        public static readonly string DisplayName = "Sound (C063)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C063";

        public C063() { }
        public C063( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C063] + 8

            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // path offset: [C063] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            SoundIndex = reader.ReadInt32(); // 1
            Unk_5 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x20;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );

            entryWriter.Write( offset );

            entryWriter.Write( SoundIndex );
            entryWriter.Write( Unk_5 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            ImGui.InputInt( $"Sound Index{id}", ref SoundIndex );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk_5 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}
