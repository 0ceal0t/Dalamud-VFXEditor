using System.Collections.Generic;
using System.Linq;

namespace VfxEditor.AvfxFormat.Vfx {
    public class NodeRemover<T> where T : UiNode {
        private readonly UiNodeGroup<T> Group;
        private readonly IUiNodeView<T> View;
        private readonly T Item;
        private readonly int Idx;

        private readonly Dictionary<UiNode, List<UiNodeSelect>> RemovedFromChildren = new();
        private readonly Dictionary<UiNodeSelect, List<UiNode>> RemovedFromParents = new();
        private readonly Dictionary<UiNodeSelect, List<int>> ParentsSelectIdx = new();

        public NodeRemover( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
            Idx = group.Items.IndexOf( item );
        }

        public void Add() {
            Group.AddAndUpdate( Item, Idx );
            // Enable node
            Item.IsDeleted = false;
            foreach( var entry in RemovedFromChildren ) {
                var node = entry.Key;
                node.Parents.AddRange( entry.Value );
                node.Graph?.NowOutdated();
            }
            foreach( var entry in RemovedFromParents ) {
                var nodeSelect = entry.Key;
                var idx = ParentsSelectIdx[nodeSelect];
                if( idx == null ) {
                    nodeSelect.NodeEnabled( Item, 0 ); // single NodeSelect
                }
                else {
                    foreach( var i in idx ) nodeSelect.NodeEnabled( Item, i ); // NodeSelectList
                }

                nodeSelect.Node.Children.AddRange( entry.Value );
            }
            foreach( var selector in Item.Selectors ) selector.LinkEvent();
            // Continue
            View.AddToAvfx( Item, Idx );
        }

        public void Remove() {
            RemovedFromChildren.Clear();
            RemovedFromParents.Clear();
            ParentsSelectIdx.Clear();
            foreach( var node in Item.Children ) {
                RemovedFromChildren[node] = node.Parents.Where( nodeSelect => nodeSelect.Node == Item ).ToList();
            }
            foreach( var nodeSelect in Item.Parents ) {
                RemovedFromParents[nodeSelect] = nodeSelect.Node.Children.Where( node => node == Item ).ToList();
                ParentsSelectIdx[nodeSelect] = nodeSelect.GetNodeIdx( Item );
            }
            Group.RemoveAndUpdate( Item );
            Item.DisableNode();
            View.RemoveFromAvfx( Item );
            View.ResetSelected();
        }
    }
}
