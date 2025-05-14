using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum SummonWeaponObjectControl {
        Weapon = 0,
        OffHand = 1,
        Summon_or_Lemure = 2,
    }

    public class C203 : TmbEntry {
        public const string MAGIC = "C203";
        public const string DISPLAY_NAME = "Summon Weapon Visibility";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x2C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" ); // chara/action/magic/2ff_sage/mgc007.tmb => 48
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt BindPointId = new( "Bind Point Id" );
        private readonly ParsedInt Rotation = new( "Rotation" );
        private readonly ParsedEnum<SummonWeaponObjectControl> ObjectControl = new( "Object Control" );
        private readonly ParsedBool NoFollow = new( "Disable Follow" );
        private readonly ParsedBool ScaleEnabled = new( "Scale Enabled", size: 2 );
        private readonly ParsedShort Unk3 = new( "Unknown 3" );
        private readonly ParsedFloat Scale = new( "Scale" );

        public C203( TmbFile file ) : base( file ) { }

        public C203( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            BindPointId,
            Rotation,
            ObjectControl,
            NoFollow,
            ScaleEnabled,
            Unk3,
            Scale
        ];
    }
}
