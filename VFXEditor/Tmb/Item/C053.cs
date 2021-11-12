using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiNET;

namespace VFXEditor.Tmb.Item {
    public class C053 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private short Unk_4 = 0;
        private short Unk_5 = 0;
        private int Unk_6 = 0;

        public static readonly string Name = "C053";

        public C053() { }
        public C053( BinaryReader reader ) {
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 8
            Unk_3 = reader.ReadInt32(); // 0
            Unk_4 = reader.ReadInt16(); // 0
            Unk_5 = reader.ReadInt16(); // 0
            Unk_6 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x1C;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos, int timelinePos ) {
            TmbFile.WriteString( entryWriter, "C053" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
            TmbFile.ShortInput( $"Unknown 4{id}", ref Unk_4 );
            TmbFile.ShortInput( $"Unknown 5{id}", ref Unk_5 );
            ImGui.InputInt( $"Uknown 6{id}", ref Unk_6 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
