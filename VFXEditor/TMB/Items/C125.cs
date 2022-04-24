using ImGuiNET;
using System.Collections.Generic;
using System.IO;

namespace VFXEditor.TMB.TMB {
    public class C125 : TMBItem {
        private int Unk_2 = 1;
        private int Unk_3 = 0;

        public static readonly string DisplayName = "C125";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C125";

        public C125() { }
        public C125( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
        }

        public override int GetSize() => 0x14;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
