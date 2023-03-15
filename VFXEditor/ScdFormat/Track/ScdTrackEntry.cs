using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.ScdFormat {
    public class ScdTrackEntry : ScdEntry, IScdSimpleUiBase {
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

        public void Draw( string id ) {
            for( var idx = 0; idx < Items.Count; idx++ ) {
                var item = Items[idx];
                if( ImGui.CollapsingHeader( $"Item #{idx} ({item.Type.Value}){id}{idx}" ) ) {
                    ImGui.Indent();

                    if( UiUtils.RemoveButton( $"Delete{id}{idx}", true ) ) { // REMOVE
                        CommandManager.Scd.Add( new GenericRemoveCommand<ScdTrackItem>( Items, item ) );
                        ImGui.Unindent(); break;
                    }

                    item.Draw( $"{id}{idx}" );
                    ImGui.Unindent();
                }
            }

            if( ImGui.Button( $"+ New{id}" ) ) { // NEW
                CommandManager.Scd.Add( new GenericAddCommand<ScdTrackItem>( Items, new ScdTrackItem() ) );
            }
        }
    }
}
