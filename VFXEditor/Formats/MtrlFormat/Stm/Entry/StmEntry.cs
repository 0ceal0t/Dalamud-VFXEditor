using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Stm.Entry {
    public class StmEntry : StmEntryBase {
        public readonly IReadOnlyList<Triple> Diffuse;
        public readonly IReadOnlyList<Triple> Specular;
        public readonly IReadOnlyList<Triple> Emissive;
        public readonly IReadOnlyList<Half> Exposure;
        public readonly IReadOnlyList<Half> Metalness;
        public readonly IReadOnlyList<Half> Roughness;
        public readonly IReadOnlyList<Half> SheenRate;
        public readonly IReadOnlyList<Half> SheenTintRate;
        public readonly IReadOnlyList<Half> SheenAperture;
        public readonly IReadOnlyList<Half> Anisotropy;
        public readonly IReadOnlyList<Half> SphereMapIndex;
        public readonly IReadOnlyList<Half> SphereMapMask;

        public StmEntry( BinaryReader reader, long offset ) : base( reader, offset ) {
            var diffuseEnd = reader.ReadUInt16() * 2;
            var specularEnd = reader.ReadUInt16() * 2;
            var emissiveEnd = reader.ReadUInt16() * 2;

            var exposureEnd = reader.ReadUInt16() * 2;
            var metalnessEnd = reader.ReadUInt16() * 2;
            var roughnessEnd = reader.ReadUInt16() * 2;
            var sheenRateEnd = reader.ReadUInt16() * 2;
            var sheenTintRateEnd = reader.ReadUInt16() * 2;
            var sheenApertureEnd = reader.ReadUInt16() * 2;
            var anisotropyEnd = reader.ReadUInt16() * 2;
            var sphereMapIndexEnd = reader.ReadUInt16() * 2;
            var sphereMapMaskEnd = reader.ReadUInt16() * 2;

            var startPos = reader.BaseStream.Position;

            Diffuse = Read( reader, startPos, diffuseEnd, ReadTriple );
            Specular = Read( reader, startPos + diffuseEnd, specularEnd - diffuseEnd, ReadTriple );
            Emissive = Read( reader, startPos + specularEnd, emissiveEnd - specularEnd, ReadTriple );

            Exposure = Read( reader, startPos + emissiveEnd, exposureEnd - emissiveEnd, ReadSingle );
            Metalness = Read( reader, startPos + exposureEnd, metalnessEnd - exposureEnd, ReadSingle );
            Roughness = Read( reader, startPos + metalnessEnd, roughnessEnd - metalnessEnd, ReadSingle );
            SheenRate = Read( reader, startPos + roughnessEnd, sheenRateEnd - roughnessEnd, ReadSingle );
            SheenTintRate = Read( reader, startPos + sheenRateEnd, sheenTintRateEnd - sheenRateEnd, ReadSingle );
            SheenAperture = Read( reader, startPos + sheenTintRateEnd, sheenApertureEnd - sheenTintRateEnd, ReadSingle );
            Anisotropy = Read( reader, startPos + sheenApertureEnd, anisotropyEnd - sheenApertureEnd, ReadSingle );
            SphereMapIndex = Read( reader, startPos + anisotropyEnd, sphereMapIndexEnd - anisotropyEnd, ReadSingle );
            SphereMapMask = Read( reader, startPos + sphereMapIndexEnd, sphereMapMaskEnd - sphereMapIndexEnd, ReadSingle );
        }
    }
}