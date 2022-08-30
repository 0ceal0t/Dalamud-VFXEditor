using ImGuiNET;
using VFXEditor.Utils;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C043 : TmbEntry {
        public const string MAGIC = "C043";
        public const string DISPLAY_NAME = "Summon Weapon (C043)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private int Duration = 0;
        private int Unk1 = 0;
        private int Unk2 = 8;
        private short WeaponId = 0;
        private short BodyId = 0;
        private int VariantId = 0;

        public C043() : base() { }

        public C043( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            WeaponId = reader.ReadInt16();
            BodyId = reader.ReadInt16();
            VariantId = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( WeaponId );
            writer.Write( BodyId );
            writer.Write( VariantId );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            FileUtils.ShortInput( $"Weapon Id{id}", ref WeaponId );
            FileUtils.ShortInput( $"Body Id{id}", ref BodyId );
            ImGui.InputInt( $"Variant Id{id}", ref VariantId );
        }
    }
}
