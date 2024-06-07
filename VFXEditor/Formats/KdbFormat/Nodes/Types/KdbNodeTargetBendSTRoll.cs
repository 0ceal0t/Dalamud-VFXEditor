using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types {
    public class KdbNodeTargetBendSTRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendSTRoll;

        public ParsedDouble BendSource = new( "Bend Source" );
        public ParsedDouble BendTarget = new( "Bend Target" );
        public ParsedDouble Roll = new( "Roll" );
        public ParsedDouble3 Unknown4 = new( "Unknown 4" );
        public ParsedDouble3 Unknown5 = new( "Unknown 5" );
        public ParsedDouble3 Unknown6 = new( "Unknown 6" );
        public ParsedDouble Unknown7 = new( "Unknown 7" );
        public ParsedQuat Unknown8 = new( "Unknown 8", size: 8 );
        public ParsedQuat Unknown9 = new( "Unknown 9", size: 8 );
        public ParsedDouble Unknown10 = new( "Unknown 10" );
        public ParsedDouble Unknown11 = new( "Unknown 11" );
        public ParsedUInt Unknown12 = new( "Unknown 12" );
        public ParsedUInt Unknown13 = new( "Unknown 13" );

        public KdbNodeTargetBendSTRoll() : base() { }

        public KdbNodeTargetBendSTRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            BendSource.Read( reader );
            BendTarget.Read( reader );
            Roll.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            Unknown9.Read( reader );
            Unknown10.Read( reader );
            Unknown11.Read( reader );
            Unknown12.Read( reader );
            Unknown13.Read( reader );
        }

        public override void WriteBody( BinaryWriter writer ) {
            BendSource.Write( writer );
            BendTarget.Write( writer );
            Roll.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            Unknown9.Write( writer );
            Unknown10.Write( writer );
            Unknown11.Write( writer );
            Unknown12.Write( writer );
            Unknown13.Write( writer );
        }

        protected override void DrawBody( List<string> bones ) {
            BendSource.Draw();
            BendTarget.Draw();
            Roll.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
            Unknown9.Draw();
            Unknown10.Draw();
            Unknown11.Draw();
            Unknown12.Draw();
            Unknown13.Draw();
        }

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
            new( ConnectionType.BendS ),
            new( ConnectionType.BendT ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
