using Lumina.Data;
using System.Collections.Generic;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    //  https://github.com/TexTools/xivModdingFramework/blob/35d0ca49b5db25332756d2762e16c95b46a7f299/xivModdingFramework/Materials/FileTypes/STM.cs#L377

    public class StmDataFile : FileResource {
        public readonly Dictionary<ushort, StmEntry> Entries = [];

        public virtual bool IsDawntrail => false;

        public override void LoadFile() {
            Reader.BaseStream.Position = 0;

            /*(Reader.ReadUInt32();
            var numEntries = Reader.ReadUInt16();
            Reader.ReadUInt16();

            var keys = new List<ushort>();
            var offsets = new List<ushort>();

            for( var i = 0; i < numEntries; i++ ) keys.Add( Reader.ReadUInt16() );

            for( var i = 0; i < numEntries; i++ ) offsets.Add( Reader.ReadUInt16() );

            for( var i = 0; i < numEntries; i++ ) {
                var offset = offsets[i] * 2 + 8 + 4 * numEntries;
                Entries[keys[i]] = new( Reader, offset, IsDawntrail );
            }*/
        }

        public StmDyeData GetDye( int template, int idx ) {
            if( !Entries.TryGetValue( ( ushort )template, out var entry ) ) return null;
            if( idx <= 0 || idx > StmEntry.MAX ) return null;

            idx--;
            var diffuse = entry.Diffuse[idx];
            var specular = entry.Specular[idx];
            var emissive = entry.Emissive[idx];
            var gloss = entry.Gloss[idx];
            var power = entry.Power[idx];

            return new() {
                Diffuse = new( ( float )diffuse.R, ( float )diffuse.G, ( float )diffuse.B ),
                Specular = new( ( float )specular.R, ( float )specular.G, ( float )specular.B ),
                Emissive = new( ( float )emissive.R, ( float )emissive.G, ( float )emissive.B ),
                Gloss = ( float )gloss,
                Power = ( float )power
            };
        }
    }
}
