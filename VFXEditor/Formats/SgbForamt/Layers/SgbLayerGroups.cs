using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Utils;

namespace VfxEditor.Formats.SgbForamt.Layers {
    public class SgbLayerGroups {
        private readonly ParsedUInt Id = new( "Id" );
        private readonly ParsedString Name = new( "Name" );
        private readonly List<SgbLayer> Layers = new();

        public SgbLayerGroups() { }

        public SgbLayerGroups( BinaryReader reader ) : this() {
            var startPos = reader.BaseStream.Position;
            Id.Read( reader );
            var nameOffset = reader.ReadUInt32();
            var layersOffsets = reader.ReadUInt32();
            var layersCount = reader.ReadUInt32();
            var endPos = reader.BaseStream.Position;

            reader.BaseStream.Seek( startPos + nameOffset, SeekOrigin.Begin );
            Name.Value = FileUtils.ReadString( reader );

            for( var i = 0; i < layersCount; i++ ) {
                reader.BaseStream.Seek( startPos + layersOffsets + ( i * 4 ), SeekOrigin.Begin );
                reader.BaseStream.Seek( startPos + layersOffsets + reader.ReadInt32(), SeekOrigin.Begin );
                Layers.Add( new( reader ) );
            }

            reader.BaseStream.Seek( endPos, SeekOrigin.Begin ); // reset position
        }
    }
}
