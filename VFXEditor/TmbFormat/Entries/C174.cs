using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum C174_ObjectPosition {
        StowedPosition = 0,
        RightHand = 1,
        RightHand_2 = 2,
        Switch_LeftHand = 3,
        RootBone = 4,
        Switch_LeftHand_2 = 5
    }

    public enum C174_ObjectControl {
        MainHand = 0,
        OffHand = 1,
        Summoned = 2
    }

    public class C174 : TmbEntry {
        public const string MAGIC = "C174";
        public const string DISPLAY_NAME = "Object Control (C174)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedSimpleEnum<C174_ObjectPosition> ObjectPosition = new( "Object Position", ObjectPositions, defaultValue: 5 );
        private readonly ParsedSimpleEnum<C174_ObjectControl> ObjectControl = new( "Object Control", ObjectControls, defaultValue: 1 );
        private readonly ParsedInt Unk4 = new( "Unknown 4", defaultValue: 1 );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public static readonly C174_ObjectPosition[] ObjectPositions = new[] {
            C174_ObjectPosition.StowedPosition,
            C174_ObjectPosition.RightHand,
            C174_ObjectPosition.RightHand_2,
            C174_ObjectPosition.Switch_LeftHand,
            C174_ObjectPosition.RootBone,
            C174_ObjectPosition.Switch_LeftHand_2
        };

        public static readonly C174_ObjectControl[] ObjectControls = new[] {
            C174_ObjectControl.MainHand,
            C174_ObjectControl.OffHand,
            C174_ObjectControl.Summoned
        };

        /*
            0 - The control object returns to the stowed weapon state.

            1 - When the control object is a summoned general weapon, bind the weapon to the right hand.

            2 - When the control object is a summoned general weapon, bind the weapon to the right hand.
            When the control object is the second hand and the second hand is the standby second hand, it seems to rotate 90 ° upward on the original bone (doubtful)

            3 - Switch the control object to the left hand when it is the main hand weapon.
            Or switch the off hand weapon to the right hand.
            When the control object is a general summoning weapon, bind it to the left hand.

            4 - In most cases, bind the object to hara bone

            5 - When the control object is a main hand weapon, switch it to the left hand.
            Or switch the off hand weapon to the right hand.
            When the control object is a general summoning weapon, bind it to the left hand. (The samurai sword will be held reversely)


            0 - Control the main weapon
            1 - Control the second hand weapon
            2 - Control summoning weapons
        */

        public C174( bool papEmbedded ) : base( papEmbedded ) { }

        public C174( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            ObjectPosition,
            ObjectControl,
            Unk4,
            Unk5,
            Unk6
        };

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
