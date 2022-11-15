using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat2 {
    public abstract class AvfxNode : AvfxWorkspaceItem {
        public uint GraphColor;
        public List<AvfxNode> ChildNodes = new();
        public List<UiNodeSelect> Parents = new();
        public List<UiNodeSelect> Selectors = new();
        public bool DepedencyImportInProgress;
        public bool IsDeleted = false;
        public UiNodeGraph Graph = null;

        public AvfxNode( string avfxName, uint graphColor, bool hasDependencies ) : base( avfxName ) {
            GraphColor = graphColor;
            DepedencyImportInProgress = hasDependencies;
        }

        public void Disconnect() {
            IsDeleted = true;
            foreach( var node in ChildNodes ) {
                node.Parents.RemoveAll( nodeSelect => nodeSelect.Node == this );
                node.Graph?.NowOutdated();
            }
            foreach( var nodeSelect in Parents ) {
                nodeSelect.DisableNode( this );
                nodeSelect.Node.ChildNodes.RemoveAll( node => node == this );
            }
            foreach( var selector in Selectors ) selector.UnlinkOnIndexChange();
        }

        public virtual void ShowTooltip() { }
    }
}
