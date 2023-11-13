using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/15ae65921468a2407ecdd068ca79947e596e24be/Files/StmFile.StainingTemplateEntry.cs

    [StructLayout( LayoutKind.Sequential )]
    public struct Triple {
        public Half R;
        public Half G;
        public Half B;
    }

    public class StmEntry {
        public const int MAX = 128;

        public readonly IReadOnlyList<Triple> Diffuse;
        public readonly IReadOnlyList<Triple> Specular;
        public readonly IReadOnlyList<Triple> Emissive;
        public readonly IReadOnlyList<Half> Gloss;
        public readonly IReadOnlyList<Half> Power;

        public StmEntry( BinaryReader reader, long offset ) {
            reader.BaseStream.Seek( offset, SeekOrigin.Begin );

            // In bytes
            var diffuseEnd = reader.ReadUInt16() * 2;
            var specularEnd = reader.ReadUInt16() * 2;
            var emissiveEnd = reader.ReadUInt16() * 2;
            var glossEnd = reader.ReadUInt16() * 2;
            var powerEnd = reader.ReadUInt16() * 2;

            offset += 5 * 2;

            Diffuse = Read( reader, offset, diffuseEnd, ReadTriple );
            Specular = Read( reader, offset + diffuseEnd, specularEnd - diffuseEnd, ReadTriple );
            Emissive = Read( reader, offset + specularEnd, emissiveEnd - specularEnd, ReadTriple );
            Gloss = Read( reader, offset + emissiveEnd, glossEnd - emissiveEnd, ReadSingle );
            Power = Read( reader, offset + glossEnd, powerEnd - glossEnd, ReadSingle );
        }

        private static Triple ReadTriple( BinaryReader reader ) => new() {
            R = reader.ReadHalf(),
            G = reader.ReadHalf(),
            B = reader.ReadHalf(),
        };

        public static Half ReadSingle( BinaryReader reader ) => reader.ReadHalf();

        private static IReadOnlyList<T> Read<T>( BinaryReader reader, long start, int length, Func<BinaryReader, T> read ) {
            reader.BaseStream.Seek( start, SeekOrigin.Begin );

            var entrySize = Marshal.SizeOf( typeof( T ) );
            var entryCount = length / entrySize;

            return entryCount switch {
                0 => new RepeatingList<T>( default!, MAX ),
                1 => new RepeatingList<T>( read( reader ), MAX ),
                MAX => ReadStandard( reader, read ),
                < MAX => new IndexedList<T>( reader, entryCount - MAX / entrySize, read ),
                _ => null
            };
        }

        private static IReadOnlyList<T> ReadStandard<T>( BinaryReader reader, Func<BinaryReader, T> read ) {
            var ret = new List<T>();
            for( var i = 0; i < MAX; i++ ) ret.Add( read( reader ) );
            return ret;
        }
    }

    public class RepeatingList<T> : IReadOnlyList<T> {
        private readonly T Value;
        public int Count { get; }

        public RepeatingList( T value, int count ) {
            Value = value;
            Count = count;
        }

        public IEnumerator<T> GetEnumerator() {
            for( var i = 0; i < Count; ++i ) yield return Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int index] => index >= 0 && index < Count ? Value : throw new IndexOutOfRangeException();
    }

    public class IndexedList<T> : IReadOnlyList<T> {
        private readonly T[] Values;
        private readonly byte[] Indexes;

        public IndexedList( BinaryReader br, int count, Func<BinaryReader, T> read ) {
            Values = new T[count + 1];
            Indexes = new byte[StmEntry.MAX];
            Indexes[0] = default!;

            for( var i = 1; i < count + 1; ++i ) Values[i] = read( br );

            // Seems to be an unused 0xFF byte marker.
            // Necessary for correct offsets.
            br.ReadByte();
            for( var i = 0; i < StmEntry.MAX; ++i ) {
                Indexes[i] = br.ReadByte();
                if( Indexes[i] > count ) Indexes[i] = 0;
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for( var i = 0; i < StmEntry.MAX; ++i ) yield return Values[Indexes[i]];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => Indexes.Length;

        public T this[int index] => index >= 0 && index < Count ? Values[Indexes[index]] : default!;
    }
}
