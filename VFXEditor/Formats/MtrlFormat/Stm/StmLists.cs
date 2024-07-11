using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    [StructLayout( LayoutKind.Sequential )]
    public struct Triple {
        public Half R;
        public Half G;
        public Half B;
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
