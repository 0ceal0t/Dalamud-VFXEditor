using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Target {
    public class KdbNodeTargetBendRoll : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetBendRoll;

        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly ParsedDouble3 Unknown3 = new( "Unknown 3" );
        public readonly ParsedDouble3 Unknown4 = new( "Unknown 4" );
        public readonly ParsedDouble3 Unknown5 = new( "Unknown 5" );
        public readonly ParsedInt Unknown6 = new( "Unknown 6" );
        public readonly ParsedQuatDouble Unknown7 = new( "Unknown 7" );
        public readonly ParsedQuatDouble Unknown8 = new( "Unknown 8" );
        public readonly ParsedByteBool Unknown9 = new( "Unknown 9" );

        public KdbNodeTargetBendRoll() : base() { }

        public KdbNodeTargetBendRoll( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            reader.ReadBytes( 12 ); // padding
            Unknown6.Read( reader );
            reader.ReadBytes( 8 ); // padding
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            reader.ReadBytes( 1 ); // padding
            Unknown9.Read( reader );
            reader.ReadBytes( 6 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            FileUtils.Pad( writer, 12 ); // padding
            Unknown6.Write( writer );
            FileUtils.Pad( writer, 8 ); // padding
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            FileUtils.Pad( writer, 1 ); // padding
            Unknown9.Write( writer );
            FileUtils.Pad( writer, 6 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            Bone.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
            Unknown9.Draw();
        }

        public override void UpdateBones( List<string> boneList ) => Bone.Guess( boneList );

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.Roll ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
