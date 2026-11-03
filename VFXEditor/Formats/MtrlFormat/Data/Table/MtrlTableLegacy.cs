using System.IO;
using VfxEditor.Formats.MtrlFormat.Data.Color;

namespace VfxEditor.Formats.MtrlFormat.Data.Table {
    public class MtrlTableLegacy : MtrlTableBase<MtrlColorRowLegacy> {
        public const int DimensionLogs = 0x42;

        public MtrlTableLegacy( MtrlFile file ) : base( file ) { }

        public MtrlTableLegacy( MtrlFile file, BinaryReader reader, long dataEnd ) : base( file, reader, dataEnd ) { }

        public override bool IsValidDimensionLogs( int logs ) => logs == 0 || logs == DimensionLogs;

        public override int DefaultDimensionLogs => 0;

        protected override DyeTableSize GetDyeTableSize() => DyeTableSize.Legacy;

        protected override MtrlColorRowLegacy GetNew() => new( File, this );

        protected override int GetRowCount() => 16;

        protected override int GetDyeChannelCount() => 1;
    }
}