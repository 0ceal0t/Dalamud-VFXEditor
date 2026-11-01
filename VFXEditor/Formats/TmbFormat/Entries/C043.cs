using System.Collections.Generic;
using Dalamud.Interface.Utility.Raii;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Base;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C043 : TmbEntry {
        public const string MAGIC = "C043";
        public const string DISPLAY_NAME = "Summon Weapon";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedEnum<C043Type> Type = new( "Type" );
        private readonly ParsedShort WeaponId = new( "Weapon Id" );
        private readonly ParsedShort BodyId = new( "Body Id" );
        private readonly ParsedInt VariantId = new( "Variant Id" );

        public C043( TmbFile file ) : base( file ) { }

        public C043( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            Type,
            WeaponId,
            BodyId,
            VariantId
        ];

        public override void DrawBody() {
            DrawHeader();
            Unk1.Draw();
            Type.Draw();

            using( var disabled = ImRaii.Disabled( Type.Value != C043Type.Use_Weapon_Id ) ) {
                WeaponId.Draw();
                BodyId.Draw();
                VariantId.Draw();
            }
        }
    }
}
