using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.TMB.TMB {
    public class C203 : TMBItem {
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;
        private int Unk_6 = 0;
        private int Unk_7 = 0;
        private int Unk_8 = 0;
        private float Unk_9 = 0;

        public static readonly string DisplayName = "C203";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C203";

        public C203() { }
        public C203( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
            Unk_6 = reader.ReadInt32();
            Unk_7 = reader.ReadInt32();
            Unk_8 = reader.ReadInt32();
            Unk_9 = reader.ReadSingle();
        }

        public override int GetSize() => 0x2C;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );
            entryWriter.Write( Unk_7 );
            entryWriter.Write( Unk_8 );
            entryWriter.Write( Unk_9 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
            ImGui.InputInt( $"Uknown 6{id}", ref Unk_6 );
            ImGui.InputInt( $"Uknown 7{id}", ref Unk_7 );
            ImGui.InputInt( $"Uknown 8{id}", ref Unk_8 );
            ImGui.InputFloat( $"Uknown 9{id}", ref Unk_9 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
