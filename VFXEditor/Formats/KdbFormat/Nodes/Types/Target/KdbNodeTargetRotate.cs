using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Target {
    public class KdbNodeTargetRotate : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetRotate;

        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly ParsedDouble2 Unknown1 = new( "Unknown 1" );
        public readonly ParsedDouble3 Unknown2 = new( "Unknown 2" );
        public readonly ParsedInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedQuatDouble Unknown4 = new( "Unknown 4" );
        public readonly ParsedQuatDouble Unknown5 = new( "Unknown 5" );
        public readonly ParsedByteBool Unknown6 = new( "Unknown 6" );

        public KdbNodeTargetRotate() : base() { }

        public KdbNodeTargetRotate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            reader.ReadBytes( 12 ); // padding
            Unknown3.Read( reader );
            reader.ReadBytes( 8 ); // padding
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            reader.ReadBytes( 1 ); // padding
            Unknown6.Read( reader );
            reader.ReadBytes( 6 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            FileUtils.Pad( writer, 12 ); // padding
            Unknown3.Write( writer );
            FileUtils.Pad( writer, 8 ); // padding
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            FileUtils.Pad( writer, 1 ); // padding
            Unknown6.Write( writer );
            FileUtils.Pad( writer, 6 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            Bone.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
        }

        public override void UpdateBones( List<string> boneList ) => Bone.Guess( boneList );

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.RotateQuat ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
