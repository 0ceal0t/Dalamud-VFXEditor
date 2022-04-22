using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class C118 : TMBItem {
        private int Unk_2 = 1;
        private int Unk_3 = 0;
        private int Unk_4 = 100;

        public static readonly string DisplayName = "C118";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C118";

        public C118() { }
        public C118( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
        }

        public override int GetSize() => 0x18;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
