using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.Ui.Interfaces {
    public interface IDraggableList<T> where T : class {
        public T GetDraggingItem();

        public void SetDraggingItem( T item );

        public List<T> GetItems();

        public string GetDraggingText( T item );

        public static bool DrawDragDrop<S>( IDraggableList<S> view, S item, string id ) where S : class {
            var listModified = false;

            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                ImGui.SetDragDropPayload( id, IntPtr.Zero, 0 );
                view.SetDraggingItem( item );
                ImGui.Text( view.GetDraggingText( item ) );
                ImGui.EndDragDropSource();
            }
            if( ImGui.BeginDragDropTarget() ) {
                if( StopDragging( view, item, id ) ) listModified = true;
                ImGui.EndDragDropTarget();
            }

            return listModified;
        }

        private static unsafe bool StopDragging<S>( IDraggableList<S> view, S destination, string id ) where S : class {
            var draggingItem = view.GetDraggingItem();
            if( draggingItem == null ) return false;

            var payload = ImGui.AcceptDragDropPayload( id );
            if( payload.NativePtr == null ) return false;

            if( draggingItem != destination ) {
                var items = view.GetItems();
                var destIdx = items.IndexOf( destination );
                var sourceIdx = items.IndexOf( draggingItem );

                if( destIdx == ( sourceIdx + 1 ) ) { // weird case, just swap them
                    items[destIdx] = draggingItem;
                    items[sourceIdx] = destination;
                }
                else {
                    items.Remove( draggingItem );
                    var idx = items.IndexOf( destination );
                    if( idx != -1 ) items.Insert( idx, draggingItem );
                }
            }
            view.SetDraggingItem( null );
            return true;
        }
    }
}
