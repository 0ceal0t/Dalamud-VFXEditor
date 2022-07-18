using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    // Animation
    public class C010 : TMBItem {
        private int Duration = 50;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;
        private float Unk_6 = 0;
        private string Path = "";
        private int Unk_7 = 0;

        public static readonly string DisplayName = "Animation (C010)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C010";

        public C010() { }
        public C010( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C010] + 8

            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
            Unk_6 = reader.ReadSingle();

            var offset = reader.ReadInt32(); // offset: [C010] + offset + 8 = animation
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            Unk_7 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x28;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );

            entryWriter.Write( offset );

            entryWriter.Write( Unk_7 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk_5 );
            ImGui.InputFloat( $"Unknown 6{id}", ref Unk_6 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            ImGui.InputInt( $"Unknown 7{id}", ref Unk_7 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}
