using AVFXLib.Models;
using System.Collections.Generic;
using ImGuiNET;
using System.Numerics;

namespace VFXEditor.UI.VFX {
    public abstract class UINode : UIWorkspaceItem {
        public uint Color;
        public List<UINode> Children = new();
        public List<UINodeSelect> Parents = new();
        public List<UINodeSelect> Selectors = new();
        public bool HasDependencies;
        public bool IsDeleted = false;
        public UINodeGraph Graph = null;

        public UINode(uint color, bool has_dependencies) {
            Color = color;
            HasDependencies = has_dependencies;
        }

        public void DeleteNode() {
            IsDeleted = true;
            foreach( var node in Children ) {
                node.Parents.RemoveAll( x => x.Node == this );

                node.Graph?.NowOutdated();
            }
            foreach( var node in Parents ) {
                node.DeleteNode(this);
                node.Node.Children.RemoveAll( x => x == this );
            }

            foreach( var s in Selectors ) {
                s.UnlinkChange();
            }
        }

        public void RefreshGraph() {
            Graph = new UINodeGraph( this );
        }

        public virtual void ShowTooltip() { }

        public abstract byte[] ToBytes();
    }
}
