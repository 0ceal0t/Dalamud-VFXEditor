using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum ObjectControlPosition {
        Stowed = 0,
        RightHand = 1,
        Unknown = 2,
        SwitchHand = 3,
        RootBone = 4,
        SwitchReverse = 5
    }

    public enum ObjectControlFinal {
        Stowed = 0,
        Drawn = 1,
        MainHand = 2,
        OffHand = 3,
        RootBone = 4,
        UncontrolledRoot = 5,
        Original = 6,
    }

    public enum ObjectControl {
        Weapon_or_Pet = 0,
        OffHand = 1,
        Lemure_or_Summon = 2
    }

    public class C174 : TmbEntry {
        public const string MAGIC = "C174";
        public const string DISPLAY_NAME = "Object Control (C174)";
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

        public C174( bool papEmbedded ) : base( papEmbedded ) { }

        public C174( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk2,
            ObjectPosition,
            ObjectControl,
            FinalPosition,
            PositionDelay,
            Unk6
        };
    }
}
