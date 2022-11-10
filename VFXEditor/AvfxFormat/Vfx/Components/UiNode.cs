using System.Collections.Generic;
using System.IO;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiNode : UiWorkspaceItem {
        public uint Color;
        public List<UiNode> Children = new();
        public List<UiNodeSelect> Parents = new();
        public List<UiNodeSelect> Selectors = new();
        public bool HasDependencies;
        public bool IsDeleted = false;
        public UiNodeGraph Graph = null;

        public UiNode( uint color, bool hasDependencies ) {
            Color = color;
            HasDependencies = hasDependencies;
        }

        public void DisableNode() {
            IsDeleted = true;
            foreach( var node in Children ) {
                node.Parents.RemoveAll( nodeSelect => nodeSelect.Node == this );
                node.Graph?.NowOutdated();
            }
            foreach( var nodeSelect in Parents ) {
                nodeSelect.NodeDisabled( this );
                nodeSelect.Node.Children.RemoveAll( node => node == this );
            }
            foreach( var selector in Selectors ) selector.UnlinkEvent();
        }

        public virtual void ShowTooltip() { }

        public abstract void Write( BinaryWriter writer );
    }
}
