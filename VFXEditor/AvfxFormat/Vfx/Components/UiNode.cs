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

        public void DeleteNode() {
            IsDeleted = true;
            foreach( var node in Children ) {
                node.Parents.RemoveAll( x => x.Node == this );

                node.Graph?.NowOutdated();
            }
            foreach( var node in Parents ) {
                node.DeleteNode( this );
                node.Node.Children.RemoveAll( x => x == this );
            }

            foreach( var s in Selectors ) {
                s.UnlinkEvent();
            }
        }

        public void RefreshGraph() {
            Graph = new UiNodeGraph( this );
        }

        public virtual void ShowTooltip() { }

        public abstract void Write( BinaryWriter writer );
    }
}
