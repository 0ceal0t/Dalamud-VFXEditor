using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    public class C043 : TMBItem {
        private int Unk_2 = 0;
        private int Unk_3 = 0;
        private int Unk_4 = 0;
        private short Unk_5 = 0; // WeaponId / parentId / subId
        private short Unk_6 = 0;
        private int Unk_7 = 0;

        public static readonly string DisplayName = "Call Weapons (C043)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C043";

        public C043() { }
        public C043( BinaryReader reader ) {
            ReadInfo( reader );
            Unk_2 = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            Unk_5 = reader.ReadInt16();
            Unk_6 = reader.ReadInt16();
            Unk_7 = reader.ReadInt32();
        }

        public override int GetSize() => 0x20;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Unk_2 );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( Unk_5 );
            entryWriter.Write( Unk_6 );
            entryWriter.Write( Unk_7 );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk_2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            FileHelper.ShortInput( $"Unknown 5{id}", ref Unk_5 );
            FileHelper.ShortInput( $"Unknown 6{id}", ref Unk_6 );
            ImGui.InputInt( $"Unknown 7{id}", ref Unk_7 );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
