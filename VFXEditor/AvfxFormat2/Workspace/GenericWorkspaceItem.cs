using Dalamud.Interface;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat2 {
    public abstract class GenericWorkspaceItem : GenericSelectableItem, IUiWorkspaceItem {
        public string Renamed;
        private string RenamedTemp;
        private bool CurrentlyRenaming = false;

        public override string GetText() => string.IsNullOrEmpty( Renamed ) ? GetDefaultText() : Renamed;

        public abstract string GetWorkspaceId();

        public string GetRenamed() => Renamed;
        public void SetRenamed( string renamed ) => Renamed = renamed;

        public virtual void PopulateWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) { }

        public virtual void ReadWorkspaceMetaChildren( Dictionary<string, string> RenameDict ) { }

        public void DrawRename( string parentId ) => IUiWorkspaceItem.DrawRenameBox( this, parentId, ref Renamed, ref RenamedTemp, ref CurrentlyRenaming );
    }
}
