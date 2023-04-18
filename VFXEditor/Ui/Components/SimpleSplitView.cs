using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class SimpleSplitView<T> : SplitView<T> where T : class, IUiItem {
        protected readonly string ItemName;
        protected readonly bool AllowReorder;

        private T DraggingItem = null;

        public SimpleSplitView( string itemName, List<T> items, bool allowNew, bool allowReorder ) : base( items, allowNew ) {
            ItemName = itemName;
            AllowReorder = allowReorder;
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

        protected override bool DrawLeftItem( T item, int idx, string id ) {
            var listModified = false;

            if( ImGui.Selectable( $"{GetText( item, idx )}{id}{idx}", item == Selected ) ) Selected = item;

            if( AllowReorder ) {
                if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                    StartDragging( item, id );
                    ImGui.Text( "..." );
                    ImGui.EndDragDropSource();
                }
                if( ImGui.BeginDragDropTarget() ) {
                    if( StopDragging( item, id ) ) listModified = true;
                    ImGui.EndDragDropTarget();
                }
            }

            return listModified;
        }

        private void StartDragging( T item, string id ) {
            ImGui.SetDragDropPayload( $"{id}-SPLIT", IntPtr.Zero, 0 );
            DraggingItem = item;
        }

        private bool StopDragging( T destination, string id ) {
            if( DraggingItem == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( $"{id}-SPLIT" );
            unsafe {
                if( payload.NativePtr != null ) {
                    if( DraggingItem != destination ) {
                        Items.Remove( DraggingItem );
                        var idx = Items.IndexOf( destination );
                        if ( idx != -1 ) {
                            Items.Insert( idx, DraggingItem );
                        }
                    }
                    DraggingItem = null;
                    return true;
                }
            }
            return false;
        }

        protected virtual string GetText( T item, int idx ) => $"{ItemName} {idx}";

        protected override void DrawSelected( string id ) {
            Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );
        }
    }
}
