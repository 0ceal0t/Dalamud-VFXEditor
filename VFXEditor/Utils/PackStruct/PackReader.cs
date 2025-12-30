using System;
using System.IO;

namespace VFXEditor.Utils.PackStruct {
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

            reader.BaseStream.Position = reader.BaseStream.Length - (long)_packFooter.TotalSize;
            StartPos = reader.BaseStream.Position;

            _baseData = reader.ReadBytes((int)_packFooter.TotalSize - sizeof( PackFooter ));
            if( _packFooter.Header.PriorOffset + _baseData.Length >= _baseData.Length )
                HasData = false;
            if( _packFooter.Header.PackCount < 1 )
                HasData = false;
        }

        public bool TryGetPrior( uint type, out Pack prior ) {
            if( !HasData ) {
                prior = default;
                return false;
            }

            var start = ( int )( _baseData.Length + _packFooter.Header.PriorOffset );
            var reader = new SpanBinaryReader( _baseData[start..] );
            var newFooter = reader.Read<PackHeader>();
            if( newFooter.Type != type ) {
                prior = default;
                return false;
            }

            prior.Header = newFooter;
            prior.Data = _baseData[( start + sizeof( PackHeader ) )..];
            _packFooter.Header = newFooter;
            _baseData = _baseData[..start];
            if( _packFooter.Header.PriorOffset + _baseData.Length >= _baseData.Length )
                HasData = false;
            if( _packFooter.Header.PackCount < 1 )
                HasData = false;
            return true;
        }
    }
}
