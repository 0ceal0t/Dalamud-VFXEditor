using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/15ae65921468a2407ecdd068ca79947e596e24be/Files/StmFile.StainingTemplateEntry.cs

    public class StmEntry {
        public const int MAX = 128;

        public readonly bool IsDawntrail;

        public readonly IReadOnlyList<Triple> Diffuse;
        public readonly IReadOnlyList<Triple> Specular;
        public readonly IReadOnlyList<Triple> Emissive;
        public readonly IReadOnlyList<Half> Gloss;
        public readonly IReadOnlyList<Half> Power;

        public readonly IReadOnlyList<Half> Unknown1;
        public readonly IReadOnlyList<Half> Unknown2;
        public readonly IReadOnlyList<Half> Unknown3;
        public readonly IReadOnlyList<Half> Unknown4;
        public readonly IReadOnlyList<Half> Unknown5;
        public readonly IReadOnlyList<Half> Unknown6;
        public readonly IReadOnlyList<Half> Unknown7;

        public StmEntry( BinaryReader reader, long offset, bool isDawntrail ) {
            IsDawntrail = isDawntrail;
            reader.BaseStream.Position = offset;

            // In bytes
            var diffuseEnd = reader.ReadUInt16() * 2;
            var specularEnd = reader.ReadUInt16() * 2;
            var emissiveEnd = reader.ReadUInt16() * 2;
            var glossEnd = reader.ReadUInt16() * 2;
            var powerEnd = reader.ReadUInt16() * 2;

            var unknown1End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown2End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown3End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown4End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown5End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown6End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;
            var unknown7End = IsDawntrail ? ( reader.ReadUInt16() * 2 ) : 0;

            offset += ( IsDawntrail ? 12 : 5 ) * 2;

            Diffuse = Read( reader, offset, diffuseEnd, ReadTriple );
            Specular = Read( reader, offset + diffuseEnd, specularEnd - diffuseEnd, ReadTriple );
            Emissive = Read( reader, offset + specularEnd, emissiveEnd - specularEnd, ReadTriple );
            Gloss = Read( reader, offset + emissiveEnd, glossEnd - emissiveEnd, ReadSingle );
            Power = Read( reader, offset + glossEnd, powerEnd - glossEnd, ReadSingle );

            if( IsDawntrail ) {
                Unknown1 = Read( reader, offset + powerEnd, unknown1End - powerEnd, ReadSingle );
                Unknown2 = Read( reader, offset + unknown1End, unknown2End - unknown1End, ReadSingle );
                Unknown3 = Read( reader, offset + unknown2End, unknown3End - unknown2End, ReadSingle );
                Unknown4 = Read( reader, offset + unknown3End, unknown4End - unknown3End, ReadSingle );
                Unknown5 = Read( reader, offset + unknown4End, unknown5End - unknown4End, ReadSingle );
                Unknown6 = Read( reader, offset + unknown5End, unknown6End - unknown5End, ReadSingle );
                Unknown7 = Read( reader, offset + unknown6End, unknown7End - unknown6End, ReadSingle );
            }
        }

        private static Triple ReadTriple( BinaryReader reader ) => new() {
            R = reader.ReadHalf(),
            G = reader.ReadHalf(),
            B = reader.ReadHalf(),
        };

        public static Half ReadSingle( BinaryReader reader ) => reader.ReadHalf();

        private static IReadOnlyList<T> Read<T>( BinaryReader reader, long start, int length, Func<BinaryReader, T> read ) {
            reader.BaseStream.Position = start;

            var entrySize = Marshal.SizeOf<T>();
            var entryCount = length / entrySize;

            return entryCount switch {
                0 => new RepeatingList<T>( default!, MAX ),
                1 => new RepeatingList<T>( read( reader ), MAX ),
                MAX => ReadStandard( reader, read ),
                < MAX => new IndexedList<T>( reader, entryCount - MAX / entrySize, read ),
                _ => null
            };
        }

        private static List<T> ReadStandard<T>( BinaryReader reader, Func<BinaryReader, T> read ) {
            var ret = new List<T>();
            for( var i = 0; i < MAX; i++ ) ret.Add( read( reader ) );
            return ret;
        }
    }
}
