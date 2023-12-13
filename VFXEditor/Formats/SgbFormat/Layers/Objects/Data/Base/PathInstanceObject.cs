using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.SgbFormat.Layers.Objects.Data.Utils;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.SgbFormat.Layers.Objects.Data {
    public class PathInstanceObject : SgbObject {
        private readonly List<PathControlPoint> Points = new();
        private readonly CommandSplitView<PathControlPoint> PointView;

        public PathInstanceObject( LayerEntryType type ) : base( type ) {
            PointView = new( "Point", Points, true, null, () => new PathControlPoint() );
        }

        public PathInstanceObject( LayerEntryType type, BinaryReader reader ) : this( type ) {
            Read( reader );
        }

        protected override void DrawBody() {
            PointView.Draw();
        }

        protected override void ReadBody( BinaryReader reader, long startPos ) {
            var offset = reader.ReadUInt32();
            var count = reader.ReadUInt32();
            reader.ReadBytes( 8 ); // padding
            var endPos = reader.BaseStream.Position;

            reader.BaseStream.Position = startPos + offset;
            for( var i = 0; i < count; i++ ) {
                Points.Add( new( reader ) );
            }

            reader.BaseStream.Position = endPos;
        }
    }
}
