using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdTrackEntry : ScdEntry, IUiItem {
        public readonly List<ScdTrackItem> Items = new();

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
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( ImGui.CollapsingHeader( $"Item {idx} ({item.Type.Value})###{idx}" ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();

                    if( UiUtils.RemoveButton( "Delete", true ) ) { // REMOVE
                        CommandManager.Scd.Add( new GenericRemoveCommand<ScdTrackItem>( Items, item ) );
                        break;
                    }

                    item.Draw();
                }
            }

            if( ImGui.Button( "+ New" ) ) { // NEW
                CommandManager.Scd.Add( new GenericAddCommand<ScdTrackItem>( Items, new ScdTrackItem() ) );
            }
        }
    }
}
