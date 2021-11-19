using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using VFXEditor.Helper;

namespace VFXEditor.Tmb.Tmb {
    public class C011 : TmbItem {
        private short Time = 0;
        private int Unk_2 = 1;
        private int Unk_3 = 0;

        public static readonly string Name = "Damager Text (C011)";

        public C011() { }
        public C011( BinaryReader reader ) {
            reader.ReadInt16(); // id
            Time = reader.ReadInt16();
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
        }

        public override int GetSize() => 0x14;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            FileHelper.WriteString( entryWriter, "C011" );
            entryWriter.Write( GetSize() );
            entryWriter.Write( Id );
            entryWriter.Write( Time );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
        }

        public override string GetName() => Name;

        public override void Draw( string id ) {
            FileHelper.ShortInput( $"Time{id}", ref Time );
            ImGui.InputInt( $"Uknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Uknown 3{id}", ref Unk_3 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
