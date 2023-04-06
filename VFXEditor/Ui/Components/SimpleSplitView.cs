using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class SimpleSplitView<T> : SplitView<T> where T : class, ISimpleUiBase {
        protected readonly string ItemName;

        public SimpleSplitView( string itemName, List<T> items, bool allowNew = false ) : base( items, allowNew ) {
            ItemName = itemName;
        }

        protected override void DrawControls( string id ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) OnNew();
            if( Selected != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    OnDelete( Selected );
                    Selected = null;
                }
            }
            ImGui.PopFont();
        }

        protected virtual void OnNew() { }

        protected virtual void OnDelete( T item ) { }

        protected override void DrawLeftItem( T item, int idx, string id ) {
            if( ImGui.Selectable( $"{ItemName} {idx}{id}{idx}", item == Selected ) ) Selected = item;
        }

        protected override void DrawSelected( string id ) {
            Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
        }
    }
}
