using System.IO;
using VfxEditor.Formats.MtrlFormat.Data.Color;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public class MtrlTable : MtrlTableBase<MtrlColorRow> {
        // 8 vec4s wide, 32 rows tall
        public const int DimensionLogs = 0x53;

        public MtrlTable( MtrlFile file ) : base( file ) { }

        public MtrlTable( MtrlFile file, BinaryReader reader, long dataEnd ) : base( file, reader, dataEnd ) { }

        public override bool IsValidDimensionLogs( int logs ) => logs == DimensionLogs;

        public override int DefaultDimensionLogs => DimensionLogs;

        protected override DyeTableSize GetDyeTableSize() => DyeTableSize.Extended;

        protected override MtrlColorRow GetNew() => new( File, this );

        protected override int GetRowCount() => 32;

        protected override int GetDyeChannelCount() => 2;
    }
}