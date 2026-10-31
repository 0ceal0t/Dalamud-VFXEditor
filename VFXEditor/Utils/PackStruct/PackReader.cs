using System;
using System.IO;

namespace VfxEditor.Utils.PackStruct {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/0e973ed6eace6afd31cd298f8c58f76fa8d5ef60/Files/PackStructs/PackReader.cs

    public unsafe ref struct PackReader {
        private ReadOnlySpan<byte> _baseData;
        private PackFooter _packFooter;

        public readonly ulong PackLength => _packFooter.TotalSize;

        public bool HasData { get; private set; }
        public long StartPos { get; private set; }

        public PackReader( BinaryReader reader ) {
            HasData = PackFooter.TryRead( reader, out _packFooter );
            if( !HasData ) {
                _packFooter.TotalSize = 0;
                return;
            }

            reader.BaseStream.Position = reader.BaseStream.Length - ( long )_packFooter.TotalSize;
            StartPos = reader.BaseStream.Position;

            _baseData = reader.ReadBytes( ( int )( reader.BaseStream.Length - reader.BaseStream.Position ) - sizeof( PackFooter ) );
            if( _packFooter.Header.PackCount < 1 )
                HasData = false;
        }

        public bool TryGetPrior( uint type, out Pack prior ) {
            if( !HasData ) {
                prior = default;
                return false;
            }

            // The pack header is usually located exactly at StartPos, but some files insert
            // alignment padding beforehand, so fall back to checking the next 16-byte boundary
            if( TryReadHeader( _baseData, type, out prior ) ) return true;

            var pad = ( int )FileUtils.NumberToPad( StartPos, 16 );
            if( pad > 0 && pad < _baseData.Length && TryReadHeader( _baseData[pad..], type, out prior ) ) {
                StartPos += pad;
                return true;
            }

            prior = default;
            return false;
        }

        private static bool TryReadHeader( ReadOnlySpan<byte> data, uint type, out Pack prior ) {
            var reader = new SpanBinaryReader( data );
            var header = reader.Read<PackHeader>();
            if( header.Type != type ) {
                prior = default;
                return false;
            }

            prior.Header = header;
            prior.Data = data[sizeof( PackHeader )..];
            return true;
        }
    }
}