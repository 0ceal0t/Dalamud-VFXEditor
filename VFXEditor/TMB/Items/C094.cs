using ImGuiNET;
using System.Collections.Generic;
using System.IO;

namespace VFXEditor.TMB.Items {
    public class C094 : TMBItem {
        private int Duration = 0;
        private int Unk_3 = 0;
        private float StartVisibility = 0;
        private float EndVisibility = 0;

        private int Unk2_1 = 0;
        private int Unk2_2 = 0;
        private int Unk2_3 = 0;
        private int Unk2_4 = 0;
        private int Unk2_5 = 0;

        public static readonly string DisplayName = "Invisibility (C094)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C094";

        public C094() { }
        public C094( BinaryReader reader ) {
            var startPos = reader.BaseStream.Position; // [C094] + 8

            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            StartVisibility = reader.ReadSingle();
            EndVisibility = reader.ReadSingle();

            var offset = reader.ReadInt32();
            var savePos = reader.BaseStream.Position;
            reader.BaseStream.Seek( startPos + offset, SeekOrigin.Begin );

            Unk2_1 = reader.ReadInt32();
            Unk2_2 = reader.ReadInt32();
            Unk2_3 = reader.ReadInt32();
            Unk2_4 = reader.ReadInt32();
            Unk2_5 = reader.ReadInt32();

            reader.BaseStream.Seek( savePos, SeekOrigin.Begin );
        }

        public override int GetSize() => 0x20;
        public override int GetExtraSize() => 0x14;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            var startPos = ( int )entryWriter.BaseStream.Position + entryPos;

            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( StartVisibility );
            entryWriter.Write( EndVisibility );

            var extraEndPos = ( int )extraWriter.BaseStream.Position + extraPos;
            var extraOffset = extraEndPos - startPos - 8;
            entryWriter.Write( extraOffset );

            extraWriter.Write( Unk2_1 );
            extraWriter.Write( Unk2_2 );
            extraWriter.Write( Unk2_3 );
            extraWriter.Write( Unk2_4 );
            extraWriter.Write( Unk2_5 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputFloat( $"Start Visibility{id}", ref StartVisibility );
            ImGui.InputFloat( $"End Visibility{id}", ref EndVisibility );

            ImGui.InputInt( $"Unknown 2,1{id}", ref Unk2_1 );
            ImGui.InputInt( $"Unknown 2,2{id}", ref Unk2_2 );
            ImGui.InputInt( $"Unknown 2,3{id}", ref Unk2_3 );
            ImGui.InputInt( $"Unknown 2,4{id}", ref Unk2_4 );
            ImGui.InputInt( $"Unknown 2,5{id}", ref Unk2_5 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
