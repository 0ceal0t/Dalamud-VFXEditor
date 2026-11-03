using System.IO;
using VfxEditor.Formats.MtrlFormat.Stm.Entry;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public class StmFile : StmFileBase<StmEntry, StmDyeData> {
        protected override int ColorCount => 3;
        protected override int ScalarCount => 9;

        protected override StmEntry ParseEntry( BinaryReader reader, long offset ) => new( reader, offset );

        protected override StmDyeData GetDye( StmEntry entry, int idx ) => new() {
            Diffuse = ToVector( entry.Diffuse[idx] ),
            Specular = ToVector( entry.Specular[idx] ),
            Emissive = ToVector( entry.Emissive[idx] ),
            Exposure = ( float )entry.Exposure[idx],
            Metalness = ( float )entry.Metalness[idx],
            Roughness = ( float )entry.Roughness[idx],
            SheenRate = ( float )entry.SheenRate[idx],
            SheenTintRate = ( float )entry.SheenTintRate[idx],
            SheenAperture = ( float )entry.SheenAperture[idx],
            Anisotropy = ( float )entry.Anisotropy[idx],
            SphereMapIndex = ( int )( float )entry.SphereMapIndex[idx],
            SphereMapMask = ( float )entry.SphereMapMask[idx]
        };
    }
}