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
        public int SourceIdx { get; private set; }
        public int TargetIdx { get; private set; }

        public ConnectionType SourceType { get; private set; }
        public ConnectionType TargetType { get; private set; }

        public KdbConnection() : base( KdbNodeType.Connection ) { }

        public KdbConnection( BinaryReader reader ) : base( KdbNodeType.Connection, reader ) { }

        public override void ReadBody( BinaryReader reader ) {
            reader.ReadUInt32(); // hash
            reader.ReadUInt32(); // offset
            SourceIdx = reader.ReadInt32();
            SourceType = ( ConnectionType )reader.ReadUInt32();

            var idk2 = reader.ReadUInt32();

            reader.ReadUInt32(); // hash
            reader.ReadUInt32(); // offset
            TargetIdx = reader.ReadInt32();
            TargetType = ( ConnectionType )reader.ReadUInt32();

            var idk4 = reader.ReadUInt32();
            var idk5 = reader.ReadUInt32();
            var idk6 = reader.ReadUInt32();

            var idk7 = reader.ReadDouble();
            var idk8 = reader.ReadUInt32();
            var idk9 = reader.ReadUInt32();

            Dalamud.Log( $"> {idk2} {idk4} {idk5} {idk6} {idk7} {idk8} {idk9}" );


        }

        protected override List<KdbSlot> GetInputSlots() => [];

        protected override List<KdbSlot> GetOutputSlots() => [];
    }
}
