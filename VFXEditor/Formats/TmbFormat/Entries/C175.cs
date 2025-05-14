using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C175 : TmbEntry {
        public const string MAGIC = "C175";
        public const string DISPLAY_NAME = "Object Scaling";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<ObjectControlPosition> ObjectScale = new( "Object Scale" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );
        private readonly ParsedEnum<ObjectControlFinal> FinalScale = new( "Final Scale" );
        private readonly ParsedInt ScaleDelay = new( "Scale Delay", value: 1 );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );

        public C175( TmbFile file ) : base( file ) { }

        public C175( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            ObjectScale,
            ObjectControl,
            FinalScale,
            ScaleDelay,
            Unk7
        ];

        /*
            Unknown 1 can either function as Duration or a general toggle. If Unknown 5 is at 0, this should be at 1 if Time is 0, or can be ignored if Time is >0.
	        Unknown 3
                (Vulon's note: seems to match object position, probably the position that the scaling setting is supposed to affect.)
                1 doesn't appear to do anything.
                2 seems to shrink your weapon to the smallest size for the length of time set in Unknown 1, and then revert to medium size (as in your weapon doesn't go back its original size).
	        Unknown 4:
                0 is main hand
                1 is off-hand
                2 affects summoned weapons
                3+ defaults to main hand
            Unknown 5 enables the use of Unknown 1 as a Duration field, with added benefits?
                0 disables Unknown 5 and allows Unknown 1 with a value of 1 to be an animation-length change.
                1 makes Unknown 1 function as Duration.
                2 will shrink your weapon for the value set in Unknown 1 before shrinking even further until your pose changes.
	                Unknown 6 seems to disable Unknown 5 if set to 1?
        */
    }
}
