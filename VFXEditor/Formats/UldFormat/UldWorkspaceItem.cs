using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.UldFormat {
    public abstract class UldWorkspaceItem : IWorkspaceUiItem {
        public readonly ParsedUInt Id = new( "Id", 1, size: 4 );
        public string Renamed = "";

        public UldWorkspaceItem( uint id ) {
            Id.Value = id;
        }

        public int GetIdx() => ( int )Id.Value;

        public void SetIdx( int idx ) { Id.Value = ( uint )idx; }

        public abstract void Draw();

        public string GetText() => string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;

        public abstract string GetDefaultText();

        public abstract string GetWorkspaceId();

        public string GetRenamed() => Renamed;

        public void SetRenamed( string renamed ) => Renamed = renamed ?? "";

        public virtual void GetChildrenRename( Dictionary<string, string> renameDict ) { }

        public virtual void SetChildrenRename( Dictionary<string, string> renameDict ) { }

        public void DrawRename( string label = "Name##Rename" ) => IWorkspaceUiItem.DrawRenameInput( this, label, ref Renamed );

        public static uint GetNextId<T>( List<T> items, uint defaultId = 1 ) where T : UldWorkspaceItem {
            if( items.Count == 0 ) return defaultId;
            return items[^1].Id.Value + 1;
        }
    }
}
