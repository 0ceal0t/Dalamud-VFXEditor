using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class C173 : TMBItem {
        private int Unk_1 = 30;
        private int Unk_2 = 0;
        private string Path = "";
        private short BindPoint_1 = 1;
        private short BindPoint_2 = 0xFF;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;
        private int Unk_6 = 0;
        private int Unk_7 = 0;
        private int Unk_8 = 0;
        private int Unk_9 = 0;
        private int Unk_10 = 0;
        private int Unk_11 = 0;
        private int Unk_12 = 0;

        public static readonly string DisplayName = "VFX (C173)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C173";

        public C173() { }
        public C173( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C173] + 8

            ReadInfo( reader );
            Unk_1 = reader.ReadInt32();
            Unk_2 = reader.ReadInt32();

            var offset = reader.ReadInt32(); // path offset: [C173] + offset + 8 = path
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );
            Path = FileHelper.ReadString( reader );
            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );

            BindPoint_1 = reader.ReadInt16();
            BindPoint_2 = reader.ReadInt16();

            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
            Unk_6 = reader.ReadInt32();
            Unk_7 = reader.ReadInt32();
            Unk_8 = reader.ReadInt32();
            Unk_9 = reader.ReadInt32();
            Unk_10 = reader.ReadInt32();
            Unk_11 = reader.ReadInt32();
            Unk_12 = reader.ReadInt32();
        }

        public override int GetSize() => 0x44;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;
            var endPos = stringPositions[Path] + stringPos;
            var offset = endPos - startPos - 8;

            WriteInfo( entryWriter );
            entryWriter.Write( Unk_1 );
            entryWriter.Write( Unk_2 );

            entryWriter.Write( offset );

            entryWriter.Write( BindPoint_1 );
            entryWriter.Write( BindPoint_2 );

            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );
            entryWriter.Write( Unk_7 );
            entryWriter.Write( Unk_8 );
            entryWriter.Write( Unk_9 );
            entryWriter.Write( Unk_10 );
            entryWriter.Write( Unk_11 );
            entryWriter.Write( Unk_12 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk_1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );

            ImGui.InputText( $"Path{id}", ref Path, 255 );
            FileHelper.ShortInput( $"Bind Point 1{id}", ref BindPoint_1 );
            FileHelper.ShortInput( $"Bind Point 2{id}", ref BindPoint_2 );

            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk_5 );
            ImGui.InputInt( $"Unknown 6{id}", ref Unk_6 );
            ImGui.InputInt( $"Unknown 7{id}", ref Unk_7 );
            ImGui.InputInt( $"Unknown 8{id}", ref Unk_8 );
            ImGui.InputInt( $"Unknown 9{id}", ref Unk_9 );
            ImGui.InputInt( $"Unknown 10{id}", ref Unk_10 );
            ImGui.InputInt( $"Unknown 11{id}", ref Unk_11 );
            ImGui.InputInt( $"Unknown 12{id}", ref Unk_12 );
        }

        public override void PopulateStringList( List<string> stringList ) {
            AddString( Path, stringList );
        }
    }
}