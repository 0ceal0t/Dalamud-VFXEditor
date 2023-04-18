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

        private static bool StopDragging<S>( IDraggableList<S> view, S destination, string id ) where S : class {
            var draggingItem = view.GetDraggingItem();
            if( draggingItem == null ) return false;

            var payload = ImGui.AcceptDragDropPayload( id );
            unsafe {
                if( payload.NativePtr != null ) {
                    if( draggingItem != destination ) {
                        var items = view.GetItems();
                        items.Remove( draggingItem );

                        var idx = items.IndexOf( destination );
                        if( idx != -1 ) items.Insert( idx, draggingItem );
                    }
                    view.SetDraggingItem( null );
                    return true;
                }
            }
            return false;
        }
    }
}
