using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.ScdFormat {
    public class ScdTrackEntry : ScdEntry, IUiItem {
        public readonly List<ScdTrackItem> Items = new();
        private readonly CollapsingHeaders<ScdTrackItem> TrackView;

        public ScdTrackEntry() {
            TrackView = new( "Item", Items, ( ScdTrackItem item, int idx ) => $"Item {idx} ({item.Type.Value})", () => new ScdTrackItem(), () => CommandManager.Scd );
        }

        public override void Read( BinaryReader reader ) {
            while( true ) {
                var newItem = new ScdTrackItem();
                newItem.Read( reader );
                Items.Add( newItem );
                if( newItem.Type.Value == TrackCmd.End || newItem.Type.Value == TrackCmd.MidiEnd || newItem.Type.Value == TrackCmd.EndForLoop ) break;
            }
        }

        public override void Write( BinaryWriter writer ) {
            Items.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            TrackView.Draw();
        }
    }
}
