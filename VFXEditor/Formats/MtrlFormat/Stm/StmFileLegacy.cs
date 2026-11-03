using System.IO;
using VfxEditor.Formats.MtrlFormat.Stm.Entry;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public class StmFileLegacy : StmFileBase<StmEntryLegacy, StmDyeDataLegacy> {
        protected override int ColorCount => 3;
        protected override int ScalarCount => 2;

        protected override StmEntryLegacy ParseEntry( BinaryReader reader, long offset ) => new( reader, offset );

        protected override StmDyeDataLegacy GetDye( StmEntryLegacy entry, int idx ) => new() {
            Diffuse = ToVector( entry.Diffuse[idx] ),
            Specular = ToVector( entry.Specular[idx] ),
            Emissive = ToVector( entry.Emissive[idx] ),
            Gloss = ( float )entry.Gloss[idx],
            Power = ( float )entry.SpecularMask[idx]
        };
    }
}