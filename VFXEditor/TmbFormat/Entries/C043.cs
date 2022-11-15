using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C043 : TmbEntry {
        public const string MAGIC = "C043";
        public const string DISPLAY_NAME = "Summon Weapon (C043)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedShort WeaponId = new( "Weapon Id" );
        private readonly ParsedShort BodyId = new( "Body Id" );
        private readonly ParsedInt VariantId = new( "Variant Id" );

        public C043() : base() { }

        public C043( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            WeaponId.Read( reader );
            BodyId.Read( reader );
            VariantId.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            WeaponId.Write( writer );
            BodyId.Write( writer );
            VariantId.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            WeaponId.Draw( id, CommandManager.Tmb );
            BodyId.Draw( id, CommandManager.Tmb );
            VariantId.Draw( id, CommandManager.Tmb );
        }
    }
}
