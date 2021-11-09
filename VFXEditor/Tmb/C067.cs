using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiNET;

namespace VFXEditor.Tmb {
    public class C067 : TmbItem {
        private short Time;
        private int Unk_2;
        private int Unk_3;

        public C067( BinaryReader reader ) {
            reader.ReadInt16(); // id
            Time = reader.ReadInt16(); // ?
            Unk_2 = reader.ReadInt32(); // 1
            Unk_3 = reader.ReadInt32(); // 0
        }

        public override int GetSize() => 0x14;
        public override int GetStringSize() => 0;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, BinaryWriter stringWriter, int stringPos, int timelinePos, ref short id ) {
            TmbFile.WriteString( entryWriter, "C067" );
            entryWriter.Write( 0x14 );
            entryWriter.Write( id++ );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
        }

        public override string GetName() => "C067";

        public override void Draw( string id ) {
            TmbFile.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
        }
    }
}
