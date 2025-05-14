using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum ObjectControlPosition {
        Stowed_State1 = 0,
        Drawn_State0 = 1,
        CraftGather_State2 = 2,
        SwitchHand_State3 = 3,
        n_throw_State4 = 4,
        SwitchReverse_State5 = 5
    }

    public enum ObjectControlFinal {
        Stowed_State1 = 0,
        Drawn_State0 = 1,
        CraftGather_State2 = 2,
        SwitchHand_State3 = 3,
        n_throw_State4 = 4,
        SwitchReverse_State5 = 5,
        Original = 6,
    }

    public enum ObjectControl {
        Weapon_or_Pet = 0,
        OffHand = 1,
        Lemure_or_Summon = 2
    }

    public class C174 : TmbEntry {
        public const string MAGIC = "C174";
        public const string DISPLAY_NAME = "Object Control";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<ObjectControlPosition> ObjectPosition = new( "Object Position" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );
        private readonly ParsedEnum<ObjectControlFinal> FinalPosition = new( "Final Position" );
        private readonly ParsedInt PositionDelay = new( "Position Delay" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C174( TmbFile file ) : base( file ) { }

        public C174( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            ObjectPosition,
            ObjectControl,
            FinalPosition,
            PositionDelay,
            Unk6
        ];
    }
}
