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

        public C043() : base() {
            Parsed = new() {
                Duration,
                Unk1,
                Unk2,
                WeaponId,
                BodyId,
                VariantId
            };
        }

        public C043( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            DrawParsed( id );
        }
    }
}
