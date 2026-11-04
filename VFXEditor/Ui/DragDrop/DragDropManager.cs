using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.DragDrop {
    public class DragDropManager<T> where T : class {
        public readonly string Name;

        private T DraggingItem = null;
        private T DraggingParent = null;

        private readonly Func<T, bool> IsFolder;
        private readonly Func<T, List<T>> GetFolderItems;
        private readonly Action Save;
        private readonly Func<T, string> GetText;
        private readonly Action<T, T> SetParent; // (item, newParentFolder), optional

        public DragDropManager( string name, Func<T, bool> isFolder, Func<T, List<T>> getFolderItems, Action save, Func<T, string> getText, Action<T, T> setParent = null ) {
            Name = name;
            IsFolder = isFolder;
            GetFolderItems = getFolderItems;
            Save = save;
            GetText = getText;
            SetParent = setParent;
        }

        public void StartDragging( T item, T parent ) {
            ImGui.SetDragDropPayload( Name, null, 0 );
            DraggingItem = item;
            DraggingParent = parent;
        }

        private bool Contains( T folder, T item ) {
            if( !IsFolder( folder ) ) return false;
            var children = GetFolderItems( folder );
            if( children == null ) return false;
            foreach( var child in children ) {
                if( child == item || Contains( child, item ) ) return true;
            }
            return false;
        }

        public bool DragDrop( T item, T parent ) {
            var ret = false;

            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                StartDragging( item, parent );
                ImGui.Text( GetText( item ) );
                ImGui.EndDragDropSource();
            }

            if( ImGui.BeginDragDropTarget() ) {
                if( StopDragging( item, parent ) ) ret = true;
                ImGui.EndDragDropTarget();
            }

            return ret;
        }

        public bool DrawDropTarget( T target, T targetParent, bool overridePosition, string childId ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) );
            style.Push( ImGuiStyleVar.FramePadding, new Vector2( 0 ) );

            ImGui.BeginChild( childId, new Vector2( ImGui.GetContentRegionAvail().X, 1 ), false );
            ImGui.EndChild();

            return DropTarget( target, targetParent, overridePosition );
        }

        public bool DropTarget( T target, T targetParent, bool overridePosition ) {
            using var dragDrop = ImRaii.DragDropTarget();
            if( !dragDrop ) return false;

            return StopDragging( target, targetParent, overridePosition );
        }

        public unsafe bool StopDragging( T destination, T destinationParent, bool overridePosition = false ) {
            if( DraggingItem == null || DraggingParent == null ) return false;
            var payload = ImGui.AcceptDragDropPayload( Name );
            if( payload.Handle == null ) return false;

            // Move them here
            if( DraggingItem != destination ) {
                if( IsFolder( DraggingItem ) && Contains( DraggingItem, destination ) ) {
                    Dalamud.Error( "Tried to put folder into itself" );
                }
                else if( Move( destination, destinationParent, overridePosition ) ) {
                    Save();
                }
            }

            DraggingItem = null;
            DraggingParent = null;
            return true;
        }

        private bool Move( T destination, T destinationParent, bool overridePosition ) {
            var sourceItems = GetFolderItems( DraggingParent );
            if( sourceItems == null ) return false;

            if( IsFolder( destination ) && !overridePosition ) { // Add to end of folder
                var destinationItems = GetFolderItems( destination );
                if( destinationItems == null ) return false;

                sourceItems.Remove( DraggingItem );
                destinationItems.Add( DraggingItem );
                SetParent?.Invoke( DraggingItem, destination );
            }
            else { // Insert before destination
                if( destinationParent == null ) return false;
                var destinationItems = GetFolderItems( destinationParent );
                if( destinationItems == null ) return false;

                sourceItems.Remove( DraggingItem );
                var idx = destinationItems.IndexOf( destination );
                if( idx < 0 ) idx = destinationItems.Count;
                destinationItems.Insert( idx, DraggingItem );
                SetParent?.Invoke( DraggingItem, destinationParent );
            }

            return true;
        }
    }
}