using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public enum ConnectionType {
        Other,
        TranslateX,
        TranslateY,
        TranslateZ,
        Distance,
        RotateQuatX_DEPRECATED,
        RotateQuatY_DEPRECATED,
        RotateQuatZ_DEPRECATED,
        RotateQuatW_DEPRECATED,
        RotateAngle,
        BendingQuatX_DEPRECATED,
        BendingQuatY_DEPRECATED,
        BendingQuatZ_DEPRECATED,
        BendingQuatW_DEPRECATED,
        BendingAngle,
        BendS,
        BendT,
        Roll,
        QuatAngle,
        ScaleX,
        ScaleY,
        ScaleZ,
        Input,
        Output,
        Translate,
        RollBend,
        RotateQuat,
        BendingQuat,
        Expmap,
        ExpmapX,
        ExpmapY,
        ExpmapZ,
        Scale
    }

    public class KdbConnection : KdbNode {
        // Really jank placeholder for connections stuff

        // https://github.com/Irastris/ValkyrieUproject/blob/main/VALKYRIE_ELYSIUM/Source/KineDriverRt/Public/SQEX_KineDriverConnectEquals.h

        public override KdbNodeType Type => KdbNodeType.Connection;

        public int SourceIdx { get; private set; }
        public int TargetIdx { get; private set; }

        public double Coeff { get; private set; }
        public uint Unknown { get; private set; }

        public ConnectionType SourceType { get; private set; }
        public ConnectionType TargetType { get; private set; }

        // jank stuff for writing stuff
        private readonly KdbNode SourceNode;
        private readonly KdbNode TargetNode;
        private readonly int ConnectionIdx;

        public KdbConnection(
            ParsedFnvHash name, KdbNode sourceNode, KdbNode targetNode,
            int connectionIdx, ConnectionType sourceType, ConnectionType targetType, double coeff, uint unknown ) {
            NameHash.Value = name.Value;
            NameHash.NameOffset = name.NameOffset;

            SourceNode = sourceNode;
            TargetNode = targetNode;
            ConnectionIdx = connectionIdx;
            SourceType = sourceType;
            TargetType = targetType;
            Coeff = coeff;
            Unknown = unknown;
        }

        public void UpdateIndexes( List<KdbNode> nodes ) {
            SourceIdx = nodes.IndexOf( SourceNode );
            TargetIdx = nodes.IndexOf( TargetNode );
        }

        public KdbConnection() : base() { }

        public KdbConnection( BinaryReader reader ) : this() { ReaderHeader( reader ); }

        public override void ReadBody( BinaryReader reader ) {
            reader.ReadUInt32(); // hash
            reader.ReadUInt32(); // offset
            SourceIdx = reader.ReadInt32();
            SourceType = ( ConnectionType )reader.ReadUInt32();

            reader.ReadUInt32(); // 0

            reader.ReadUInt32(); // hash
            reader.ReadUInt32(); // offset
            TargetIdx = reader.ReadInt32();
            TargetType = ( ConnectionType )reader.ReadUInt32();

            reader.ReadUInt32(); // index of connection to input, if there are multiple

            reader.ReadUInt32(); // 0
            reader.ReadUInt32(); // 0

            Coeff = reader.ReadDouble();
            Unknown = reader.ReadUInt32();

            reader.ReadUInt32(); // 0
        }

        public override void WriteBody( BinaryWriter writer ) {
            SourceNode.NameHash.Write( writer );
            writer.Write( SourceIdx );
            writer.Write( ( uint )SourceType );

            writer.Write( 0 );

            TargetNode.NameHash.Write( writer );
            writer.Write( TargetIdx );
            writer.Write( ( uint )TargetType );

            writer.Write( ConnectionIdx );

            writer.Write( 0 );
            writer.Write( 0 );

            writer.Write( Coeff );
            writer.Write( Unknown );

            writer.Write( 0 );
        }

        protected override List<KdbSlot> GetInputSlots() => [];
        protected override List<KdbSlot> GetOutputSlots() => [];
        protected override void DrawBody( List<string> bones ) { }
    }
}
