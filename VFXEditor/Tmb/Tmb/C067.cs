using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;

namespace VFXEditor.Tmb.Tmb {
    public class C067 : TmbItem {
        private int Unk_2 = 1;
        private int Unk_3 = 0;

        public static readonly string DisplayName = "C067";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C067";

        public C067() { }
        public C067( BinaryReader reader ) {
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
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
