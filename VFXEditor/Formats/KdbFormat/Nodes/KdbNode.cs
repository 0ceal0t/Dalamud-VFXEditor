using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public abstract KdbNodeType Type { get; }
        public readonly ParsedFnvHash NameHash = new( "Name" );

        public KdbNode() : base() { // TODO
            Name = $"{Type}";
            InitName();
            Style.ColorUnique = NodeUtils.Colors.NormalBar_Grey; // TODO
        }

        protected void ReaderHeader( BinaryReader reader ) {
            // var a = reader.BaseStream.Position;
            reader.ReadByte(); // padding
            reader.ReadByte(); // 0x01
            reader.ReadByte(); // padding

            NameHash.Read( reader );

            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0

            var bodyPosition = reader.BaseStream.Position + reader.ReadUInt32();
            var savePosition = reader.BaseStream.Position;
            reader.BaseStream.Position = bodyPosition;
            ReadBody( reader );
            // Dalamud.Log( $">>> {Type} / {a:X4} [{bodyPosition:X4} -> {reader.BaseStream.Position:X4}]" );
            reader.BaseStream.Position = savePosition;
        }

        public void Write( BinaryWriter writer, Dictionary<KdbNode, long> positions ) {
            writer.Write( ( byte )0 );
            writer.Write( ( byte )0x01 );
            writer.Write( ( byte )0 );

            NameHash.Write( writer );

            writer.Write( 0 );
            writer.Write( 0 );

            positions[this] = writer.BaseStream.Position;
            writer.Write( 0 ); // placeholder
        }

        public abstract void ReadBody( BinaryReader reader );

        public abstract void WriteBody( BinaryWriter writer );

        public void Draw( List<string> bones ) {
            NameHash.Draw();
            DrawBody( bones );
        }

        protected abstract void DrawBody( List<string> bones );

        private static KdbSlot FindSlot( List<KdbSlot> slots, ConnectionType type ) => slots.FirstOrDefault( x => x.Type == type, null );

        public KdbSlot FindInput( ConnectionType type ) => FindSlot( Inputs, type );

        public KdbSlot FindOutput( ConnectionType type ) => FindSlot( Outputs, type );

        public virtual void UpdateBones( List<string> boneList ) { }
    }
}
