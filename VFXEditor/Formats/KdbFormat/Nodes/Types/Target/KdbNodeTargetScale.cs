using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Target {
    public class KdbNodeTargetScale : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetScale;

        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly ParsedDouble3 Scale = new( "Scale" );
        public readonly ParsedByteBool Unknown3 = new( "Unknown 3" );
        public readonly ParsedByteBool Unknown4 = new( "Unknown 4" );
        public readonly ParsedByteBool Unknown5 = new( "Unknown 5" );

        public KdbNodeTargetScale() : base() { }

        public KdbNodeTargetScale( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );
            Scale.Read( reader );
            reader.ReadBytes( 12 ); // padding
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            reader.ReadBytes( 1 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );
            Scale.Write( writer );
            FileUtils.Pad( writer, 12 ); // padding
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            FileUtils.Pad( writer, 1 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            Bone.Draw();
            Scale.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
        }

        public override void UpdateBones( List<string> boneList ) => Bone.Guess( boneList );

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.ScaleX ),
            new( ConnectionType.ScaleY ),
            new( ConnectionType.ScaleZ ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
