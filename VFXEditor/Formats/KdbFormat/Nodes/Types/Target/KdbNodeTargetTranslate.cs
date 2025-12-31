using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes.Types.Target {
    public class KdbNodeTargetTranslate : KdbNode {
        public override KdbNodeType Type => KdbNodeType.TargetTranslate;

        public readonly ParsedFnvHash Bone = new( "Bone" );
        public readonly ParsedDouble3 Translate = new( "Translate" );
        public readonly ParsedDouble3 NeutralTranslate = new( "Neutral Translate" );
        public readonly ParsedQuatDouble NeutralRotate = new( "Neutral Rotate" );

        public KdbNodeTargetTranslate() : base() { }

        public KdbNodeTargetTranslate( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            Bone.Read( reader );
            Translate.Read( reader );
            NeutralTranslate.Read( reader );
            NeutralRotate.Read( reader );
            reader.ReadBytes( 8 ); // padding
        }

        public override void WriteBody( BinaryWriter writer ) {
            Bone.Write( writer );
            Translate.Write( writer );
            NeutralTranslate.Write( writer );
            NeutralRotate.Write( writer );
            FileUtils.Pad( writer, 8 ); // padding
        }

        protected override void DrawBody( List<string> bones ) {
            Bone.Draw();
            Translate.Draw();
            NeutralTranslate.Draw();
            NeutralRotate.Draw();
        }

        public override void UpdateBones( List<string> boneList ) => Bone.Guess( boneList );

        protected override List<KdbSlot> GetInputSlots() => [
            new( ConnectionType.TranslateX ),
            new( ConnectionType.TranslateY ),
            new( ConnectionType.TranslateZ ),
            new( ConnectionType.Translate ),
        ];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
