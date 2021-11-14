using System.Collections.Generic;
using System.IO;
using ImGuiNET;

namespace VFXEditor.Tmb.Tmb {
    public class C006 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 1;
        private int Unk_3 = 0;
        private int Unk_4 = 100;

        public static readonly string Name = "C006";

        public C006() { }
        public C006( BinaryReader reader ) {
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 1
            Unk_3 = reader.ReadInt32(); // 0
            Unk_4 = reader.ReadInt32(); // 100
        }

        public override int GetSize() => 0x18;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            TmbFile.WriteString( entryWriter, "C006" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Uknown 4{id}", ref Unk_4 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
