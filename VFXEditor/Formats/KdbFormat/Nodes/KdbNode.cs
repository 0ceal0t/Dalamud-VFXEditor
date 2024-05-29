using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.NodeGraphViewer;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public enum KdbNodeType : byte {
        Unknown = 0,
        SyncPose = 1,
        Source = 2,
        SourceTranslate = 3,
        SourceRotate = 4,
        SourceOther = 5,
        TargetTranslate = 6,
        TargetScale = 7,
        TargetRotate = 8,
        TargetBendRoll = 9,
        TargetBendSTRoll = 10,
        TargetExpmap = 11,
        TargetPosContraint = 12,
        TargetOrientationConstraint = 13,
        TargetDirectionConstraint = 14,
        TargetOther = 15,
        EffectorInverse = 16,
        EffectorLinkWith = 17,
        EffectorEZParamLink = 18,
        EffectorEZParamLinkLinear = 19,
        EffectorRBFInterp = 20,
        EffectorExpr = 21,
        Connection = 22
    }

    public abstract class KdbNode : Node<KdbSlot> { // TODO: body stuff
        public readonly KdbNodeType Type;
        public readonly ParsedByte Unknown1 = new( "Unknown 1" );
        public readonly ParsedUInt Unknown2 = new( "Unknown 2" );
        public readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedFnvHash NameHash = new( "Name" );

        public KdbNode( KdbNodeType type ) : base( $"{type}" ) { // TODO
            Type = type;
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey; // TODO
        }

        public KdbNode( KdbNodeType type, BinaryReader reader ) : this( type ) {
            reader.ReadByte(); // padding
            Unknown1.Read( reader );
            reader.ReadByte(); // padding

            NameHash.Read( reader );
            reader.ReadUInt16(); // offset
            reader.ReadUInt16(); // padding

            Unknown2.Read( reader );
            Unknown3.Read( reader );

            var bodyPosition = reader.BaseStream.Position + reader.ReadUInt32();
            var savePosition = reader.BaseStream.Position;
            reader.BaseStream.Position = bodyPosition;
            ReadBody( reader );
            reader.BaseStream.Position = savePosition;
        }

        public abstract void ReadBody( BinaryReader reader );

        public void Draw() {
            NameHash.Draw();
            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();

            // TODO
        }
    }
}
