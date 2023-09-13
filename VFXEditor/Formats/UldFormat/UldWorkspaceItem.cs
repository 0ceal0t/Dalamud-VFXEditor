using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.UldFormat {
    public abstract class UldWorkspaceItem : IWorkspaceUiItem {
        public readonly ParsedUInt Id = new( "Id", 1, size: 4 );
        public string Renamed = "";

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

        public void DrawRename() => IWorkspaceUiItem.DrawRenameInput( this, ref Renamed );
    }
}
