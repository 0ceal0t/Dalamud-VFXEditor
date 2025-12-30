using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace VfxEditor.Utils {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/0e973ed6eace6afd31cd298f8c58f76fa8d5ef60/Files/Utility/SpanBinaryReader.cs#L17

    public unsafe ref struct SpanBinaryReader {
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private readonly ref byte _start;
        [DebuggerBrowsable( DebuggerBrowsableState.Never )]
        private ref byte _pos;

        private SpanBinaryReader( ref byte start, int length ) {
            _start = ref start;
            _pos = ref _start;
            Length = length;
            Remaining = Length;
        }

        public override string ToString()
            => $"{Position} / {Length}";

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public SpanBinaryReader( ReadOnlySpan<byte> span )
            : this( ref MemoryMarshal.GetReference( span ), span.Length ) { }

        public int Position
            => ( int )Unsafe.ByteOffset( ref _start, ref _pos );

        public readonly int Length;

        public int Remaining { get; private set; }

        public readonly int Count
            => Length;

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public void Skip( int numBytes ) {
            if( Remaining < numBytes )
                throw new EndOfStreamException();

            _pos = ref Unsafe.Add( ref _pos, numBytes );
            Remaining -= numBytes;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public T Read<T>() where T : unmanaged, allows ref struct {
            var size = Unsafe.SizeOf<T>();
            if( Remaining < size )
                throw new EndOfStreamException();

            var ret = Unsafe.ReadUnaligned<T>( ref _pos );
            _pos = ref Unsafe.Add( ref _pos, size );
            Remaining -= size;
            return ret;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public ReadOnlySpan<T> Read<T>( int num ) where T : unmanaged {
            var size = Unsafe.SizeOf<T>() * num;
            if( Remaining < size )
                throw new EndOfStreamException();

            var ptr = Unsafe.AsPointer( ref _pos );
            _pos = ref Unsafe.Add( ref _pos, size );
            Remaining -= size;
            return new ReadOnlySpan<T>( ptr, num );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public byte ReadByte()
            => Read<byte>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public short ReadInt16()
            => Read<short>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public int ReadInt32()
            => Read<int>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public long ReadInt64()
            => Read<long>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public ushort ReadUInt16()
            => Read<ushort>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public uint ReadUInt32()
            => Read<uint>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public ulong ReadUInt64()
            => Read<ulong>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public nint ReadIntPtr()
            => Read<nint>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public float ReadSingle()
            => Read<float>();

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public readonly SpanBinaryReader SliceFrom( int position, int count ) {
            if( position < 0 || count < 0 )
                throw new ArgumentOutOfRangeException($"{position} {count}");
            if( position + count > Length )
                throw new EndOfStreamException();

            return new SpanBinaryReader( ref Unsafe.Add( ref _start, position ), count );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public SpanBinaryReader SliceFromHere( int count ) {
            ArgumentOutOfRangeException.ThrowIfNegative( count );
            if( Remaining < count )
                throw new EndOfStreamException();

            var ret = new SpanBinaryReader( ref _pos, count );
            Remaining -= count;
            _pos = ref Unsafe.Add( ref _pos, count );
            return ret;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public readonly ReadOnlySpan<byte> ReadByteString( int offset = 0 ) {
            ArgumentOutOfRangeException.ThrowIfNegative( offset );
            if( Length < offset )
                throw new EndOfStreamException();

            var span = MemoryMarshal.CreateReadOnlySpan( ref Unsafe.Add( ref _start, offset ), Length - offset );
            var idx = span.IndexOf<byte>( 0 );
            if( idx < 0 )
                throw new EndOfStreamException();

            return span[..idx];
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public readonly ReadOnlySpan<byte> ReadByteString( int offset, int length ) {
            if( offset < 0 || length < 0 )
                throw new ArgumentOutOfRangeException( $"{offset} {length}" );
            if( Length < offset + length )
                throw new EndOfStreamException();

            return MemoryMarshal.CreateReadOnlySpan( ref Unsafe.Add( ref _start, offset ), length );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public readonly string ReadString( int offset = 0 )
            => Encoding.UTF8.GetString( ReadByteString( offset ) );

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization )]
        public readonly string ReadString( int offset, int length )
            => Encoding.UTF8.GetString( ReadByteString( offset, length ) );
    }
}
