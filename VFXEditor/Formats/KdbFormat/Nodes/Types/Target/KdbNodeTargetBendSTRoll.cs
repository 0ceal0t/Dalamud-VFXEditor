using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Target {
    public class KdbNodeTargetBendSTRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendSTRoll;

        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly ParsedDouble BendSource = new( "Bend Source" );
        public readonly ParsedDouble BendTarget = new( "Bend Target" );
        public readonly ParsedDouble Roll = new( "Roll" );
        public readonly ParsedDouble3 PivotOffset = new( "Pivot Offset" );
        public readonly ParsedDouble3 Aim = new( "Aim" );
        public readonly ParsedDouble3 Up = new( "Up" );
        public readonly ParsedDouble Unknown3 = new( "Unknown 3" );
        public readonly ParsedQuatDouble Unknown4 = new( "Unknown 4" );
        public readonly ParsedQuatDouble Unknown5 = new( "Unknown 5" );
        public readonly ParsedDouble Unknown6 = new( "Unknown 6" );
        public readonly ParsedUInt Unknown7 = new( "Unknown 7" );
        public readonly ParsedByteBool Unknown8 = new( "Unknown 8" );

        public KdbNodeTargetBendSTRoll() : base() { }

        public KdbNodeTargetBendSTRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );
            BendSource.Read( reader );
            BendTarget.Read( reader );
            Roll.Read( reader );
            PivotOffset.Read( reader );
            Aim.Read( reader );
            Up.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            reader.ReadBytes( 3 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );
            BendSource.Write( writer );
            BendTarget.Write( writer );
            Roll.Write( writer );
            PivotOffset.Write( writer );
            Aim.Write( writer );
            Up.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            FileUtils.Pad( writer, 3 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            Bone.Draw();
            BendSource.Draw();
            BendTarget.Draw();
            Roll.Draw();
            PivotOffset.Draw();
            Aim.Draw();
            Up.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
        }

        public override void UpdateBones( List<string> boneList ) => Bone.Guess( boneList );

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
            new( ConnectionType.BendS ),
            new( ConnectionType.BendT ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
