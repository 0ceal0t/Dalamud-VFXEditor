using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.FileManager;

namespace VfxEditor.Ui.Interfaces {
    public interface IDraggableList<T> where T : class {
        public T GetDraggingItem();

        public void SetDraggingItem( T item );

        public List<T> GetItems();

        public string GetDraggingText( T item );

        public static bool DrawDragDrop<S>( IDraggableList<S> view, S item, string id, CommandManager manager = null ) where S : class {
            var listModified = false;

            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                ImGui.SetDragDropPayload( id, IntPtr.Zero, 0 );
                view.SetDraggingItem( item );
                ImGui.Text( view.GetDraggingText( item ) );
                ImGui.EndDragDropSource();
            }
            if( ImGui.BeginDragDropTarget() ) {
                if( StopDragging( view, item, id, manager ) ) listModified = true;
                ImGui.EndDragDropTarget();
            }

            return listModified;
        }

        private static unsafe bool StopDragging<S>( IDraggableList<S> view, S destination, string id, CommandManager manager ) where S : class {
            var draggingItem = view.GetDraggingItem();
            if( draggingItem == null ) return false;

            var payload = ImGui.AcceptDragDropPayload( id );
            if( payload.NativePtr == null ) return false;

            if( draggingItem != destination ) {
                var command = new GenericMoveCommand<S>( view.GetItems(), draggingItem, destination );
                if( manager == null ) {
                    command.Execute();
                }
                else {
                    manager.Add( command );
                }
            }
            view.SetDraggingItem( null );
            return true;
        }
    }
}
