using Lumina.Data;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Stm.Entry;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    //  https://github.com/Ottermandias/Penumbra.GameData/blob/e93093f/Files/StmFile.cs

    public abstract class StmFileBase<T, TDye> : FileResource where T : StmEntryBase where TDye : StmDyeDataBase {
        public bool IsLegacy { get; private set; } = false;

        public readonly Dictionary<int, T> Entries = [];

        public int[] Templates { get; private set; } = [0];

        protected abstract int ColorCount { get; }
        protected abstract int ScalarCount { get; }

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;

            Reader.ReadUInt16(); // magic
            var version = Reader.ReadUInt16();
            var numEntries = Reader.ReadUInt16();
            int numColors = Reader.ReadByte();
            int numScalars = Reader.ReadByte();

            switch( version ) {
                case 0x0101:
                    IsLegacy = true;
                    numColors = 3;
                    numScalars = 2;
                    break;
            }

            if( numColors != ColorCount || numScalars != ScalarCount ) {
                Dalamud.Error( $"Unexpected STM layout v{version:X4}: {numColors} colors and {numScalars} scalars, expected {ColorCount} and {ScalarCount}" );
                return;
            }

            var keys = new List<uint>();
            var offsets = new List<uint>();

            for( var i = 0; i < numEntries; i++ ) keys.Add( Reader.ReadUInt32() );
            for( var i = 0; i < numEntries; i++ ) offsets.Add( Reader.ReadUInt32() );

            var startPos = Reader.BaseStream.Position;
            var templates = new List<int> { 0 };
            for( var i = 0; i < numEntries; i++ ) {
                Entries[( int )keys[i]] = ParseEntry( Reader, ( offsets[i] * 2 ) + startPos );
                templates.Add( ( int )keys[i] );
            }
            Templates = [.. templates];
        }

        protected abstract T ParseEntry( BinaryReader reader, long offset );

        public TDye GetDye( int template, int stainId ) {
            if( stainId <= 0 || stainId > StmEntryBase.MAX ) return null;
            if( !Entries.TryGetValue( template, out var entry ) ) return null;
            return GetDye( entry, stainId - 1 ); // the 0th stain is skipped
        }

        protected abstract TDye GetDye( T entry, int idx );

        protected static System.Numerics.Vector3 ToVector( Triple value ) => new( ( float )value.R, ( float )value.G, ( float )value.B );
    }
}