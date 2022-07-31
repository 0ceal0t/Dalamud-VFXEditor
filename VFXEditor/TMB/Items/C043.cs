using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.Helper;

namespace VFXEditor.TMB.Items {
    public class C043 : TMBItem {
        private int Duration = 0;
        private int Unk_3 = 0;
        private int Unk_4 = 8;
        private short WeaponId = 0;
        private short BodyId = 0;
        private int VariantId = 0;

        public static readonly string DisplayName = "Summon Weapon (C043)";
        public override string GetDisplayName() => DisplayName;
        public override string GetName() => "C043";

        public C043() { }
        public C043( BinaryReader reader ) {
            ReadInfo( reader );
            Duration = reader.ReadInt32();
            Unk_3 = reader.ReadInt32();
            Unk_4 = reader.ReadInt32();
            WeaponId = reader.ReadInt16();
            BodyId = reader.ReadInt16();
            VariantId = reader.ReadInt32();
        }

        public override int GetSize() => 0x20;
        public override int GetExtraSize() => 0;

        public override void Write( BinaryWriter entryWriter, int entryPos, BinaryWriter extraWriter, int extraPos, Dictionary<string, int> stringPositions, int stringPos ) {
            WriteInfo( entryWriter );
            entryWriter.Write( Duration );
            entryWriter.Write( Unk_3 );
            entryWriter.Write( Unk_4 );
            entryWriter.Write( WeaponId );
            entryWriter.Write( BodyId );
            entryWriter.Write( VariantId );
        }

        public override void Draw( string id ) {
            DrawInfo( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk_3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk_4 );
            FileHelper.ShortInput( $"Weapon Id{id}", ref WeaponId );
            FileHelper.ShortInput( $"Body Id{id}", ref BodyId );
            ImGui.InputInt( $"Variant Id{id}", ref VariantId );
        }

        public override void PopulateStringList( List<string> stringList ) {
        }
    }
}
