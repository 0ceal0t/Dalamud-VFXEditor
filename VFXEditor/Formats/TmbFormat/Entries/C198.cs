using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum SummonId
    {
        Summon_0 = 0,
        Summon_1 = 1,
    }
    public enum C198AtchState {
        Default = 0,
        State_0 = 1,
        State_1 = 2,
        State_2 = 3,
        State_3 = 4,
        State_4 = 5,
        State_5 = 6,
    }
    public class C198 : TmbEntry {
        public const string MAGIC = "C198";
        public const string DISPLAY_NAME = "Lemure";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedEnum<SummonId> SummonId = new ( "Summon Id", size: 1 );
        private readonly ParsedEnum<C198AtchState> AtchState = new ( "ATCH State", size: 1 );
        private readonly ParsedShort Unk4 = new ( "Unknown 4" );
        private readonly ParsedShort ModelId = new( "Model Id" );
        private readonly ParsedShort BodyId = new( "Body Id" );
        private readonly ParsedInt Variant = new( "Variant" );

        public C198( TmbFile file ) : base( file ) { }

        public C198( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            Unk2,
            Unk3,
            SummonId,
            AtchState,
            Unk4,
            ModelId,
            BodyId,
            Variant
        ];
    }
}
