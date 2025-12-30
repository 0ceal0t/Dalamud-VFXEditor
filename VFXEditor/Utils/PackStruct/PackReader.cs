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

            reader.BaseStream.Position = reader.BaseStream.Length - (long)_packFooter.TotalSize;
            FileUtils.PadTo( reader, 16 );
            StartPos = reader.BaseStream.Position;

            _baseData = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position) - sizeof( PackFooter ));
            if( _packFooter.Header.PackCount < 1 )
                HasData = false;
        }

        public bool TryGetPrior( uint type, out Pack prior ) {
            if( !HasData ) {
                prior = default;
                return false;
            }

            var reader = new SpanBinaryReader( _baseData );
            var newFooter = reader.Read<PackHeader>();
            if( newFooter.Type != type ) {
                prior = default;
                return false;
            }

            prior.Header = newFooter;
            prior.Data = _baseData[sizeof( PackHeader )..];

            return true;
        }
    }
}
