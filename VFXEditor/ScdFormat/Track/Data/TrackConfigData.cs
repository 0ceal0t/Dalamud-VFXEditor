using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public enum TrackCmdConfigType {
        IntervalType,
        IntervalTypeFloat = 1
    }

    public class TrackConfigData : ScdTrackData {
        public readonly ParsedEnum<TrackCmdConfigType> Type = new( "Type", size: 2 );
        private ushort Count;
        private ushort DataSingle;
        private readonly List<ushort> DataList = new();

        public override void Read( BinaryReader reader ) {
            Type.Read( reader );
            Count = reader.ReadUInt16();

            if( Type.Value == TrackCmdConfigType.IntervalTypeFloat ) {
                DataSingle = reader.ReadUInt16();
            }
            else if( Type.Value > TrackCmdConfigType.IntervalTypeFloat ) {
                for( var i = 0; i < Count; i++ ) DataList.Add( reader.ReadUInt16() );
            }
        }

        public override void Write( BinaryWriter writer ) {
            Type.Write( writer );
            writer.Write( Count );

            if( Type.Value == TrackCmdConfigType.IntervalTypeFloat ) {
                writer.Write( DataSingle );
            }
            else if( Type.Value > TrackCmdConfigType.IntervalTypeFloat ) {
                DataList.ForEach( writer.Write );
            }
        }

        public override void Draw() {
            Type.Draw( CommandManager.Scd );
        }
    }
}
