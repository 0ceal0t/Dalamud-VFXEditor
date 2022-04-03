using System.Collections.Generic;
using System.IO;
using ImGuiNET;

namespace VFXEditor.Tmb.Tmb {
    public class C042 : TmbItem {
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private int Unk_5 = 0;

        public static readonly string DisplayName = "C042";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C042";

        public C042() { }
        public C042( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt32();
        }

        public override int GetSize() => 0x1C;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
            ImGui.InputInt( $"Uknown 5{id}", ref Unk_5 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
