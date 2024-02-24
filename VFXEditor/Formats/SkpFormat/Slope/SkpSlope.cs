using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.SkpFormat.Slope {
    public class SkpSlope {
        private readonly ParsedInt Unknown1 = new( "Unknown 1" );
        private readonly ParsedInt Unknown2 = new( "Unknown 2" );
        private readonly ParsedFloat3 CenterOffset = new( "Center Offset" );

        private readonly List<SkpSlopePoint> Points = new();
        private readonly CollapsingHeaders<SkpSlopePoint> PointView;

        public SkpSlope() {
            PointView = new( "Point", Points, null, () => new() );
        }

        public void Read( BinaryReader reader ) {
            reader.ReadInt32(); // Magic
            reader.ReadInt32(); // Version

            Unknown1.Read( reader );
            Unknown2.Read( reader );
            CenterOffset.Read( reader );

            var numPoints = reader.ReadInt32();
            for( var i = 0; i < numPoints; i++ ) Points.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            writer.Write( 0x00667370 );
            writer.Write( 0x02000000 );

            Unknown1.Write( writer );
            Unknown2.Write( writer );
            CenterOffset.Write( writer );

            writer.Write( Points.Count );
            Points.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            Unknown1.Draw();
            Unknown2.Draw();
            CenterOffset.Draw();

            PointView.Draw();
        }
    }
}
