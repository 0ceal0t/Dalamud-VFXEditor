using System.Collections.Generic;
using System.IO;

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

    public class KdbConnection : KdbNode { // placeholder for edges and stuff
        // https://github.com/Irastris/ValkyrieUproject/blob/main/VALKYRIE_ELYSIUM/Source/KineDriverRt/Public/SQEX_KineDriverConnectEquals.h

        public int SourceIdx { get; private set; }
        public int TargetIdx { get; private set; }

        public double Coeff { get; private set; }
        public uint Unknown { get; private set; }

        public ConnectionType SourceType { get; private set; }
        public ConnectionType TargetType { get; private set; }

        public KdbConnection() : base( KdbNodeType.Connection ) { }

        public KdbConnection( BinaryReader reader ) : base( KdbNodeType.Connection, reader ) { }

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

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
