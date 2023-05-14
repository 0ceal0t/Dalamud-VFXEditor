using System;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public abstract class GenericWorkspaceItem : GenericSelectableItem, IWorkspaceUiItem {
        public string Renamed;
        private string RenamedTemp;
        private bool CurrentlyRenaming = false;

        public override string GetText() => string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;

        public abstract string GetWorkspaceId();

        public string GetRenamed() => Renamed;

        public void SetRenamed( string renamed ) => Renamed = renamed;

        public virtual void GetChildrenRename( Dictionary<string, string> renameDict ) { }

        public virtual void SetChildrenRename( Dictionary<string, string> renameDict ) { }

        public void DrawRename() => IWorkspaceUiItem.DrawRenameBox( this, ref Renamed, ref RenamedTemp, ref CurrentlyRenaming );
    }
}
