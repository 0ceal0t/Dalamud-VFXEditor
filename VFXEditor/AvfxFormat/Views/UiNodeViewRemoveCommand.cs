using System;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiNodeViewRemoveCommand<T> : ICommand where T : UiNode {
        private readonly UiNodeGroup<T> Group;
        private readonly IUiNodeView<T> View;
        private readonly T Item;
        private readonly int Idx;

        private readonly Dictionary<UiNode, List<UiNodeSelect>> RemovedFromChildren = new();
        private readonly Dictionary<UiNodeSelect, List<UiNode>> RemovedFromParents = new();
        private readonly Dictionary<UiNodeSelect, List<int>> ParentsSelectIdx = new();

        public UiNodeViewRemoveCommand( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
            Idx = group.Items.IndexOf( item );
        }

        public void Execute() {
            /*
             *             IsDeleted = true;
            foreach( var node in Children ) {
                node.Parents.RemoveAll( x => x.Node == this );
                node.Graph?.NowOutdated();
            }
            foreach( var node in Parents ) {
                node.DeleteNode( this );
                node.Node.Children.RemoveAll( x => x == this );
            }

            foreach( var s in Selectors ) s.UnlinkEvent();
             */

            foreach ( var node in Item.Children ) {
                RemovedFromChildren[node] = node.Parents.Where( nodeSelect => nodeSelect.Node == Item ).ToList();
            }
            foreach ( var nodeSelect in Item.Parents ) {
                RemovedFromParents[nodeSelect] = nodeSelect.Node.Children.Where( node => node == Item ).ToList();
                ParentsSelectIdx[nodeSelect] = nodeSelect.GetNodeIdx( Item );
            }
            Remove();
        }

        public void Redo() {
            Remove();
        }

        public void Undo() {
            Group.AddAndUpdate( Item, Idx );
            // Enable node
            Item.IsDeleted = false;
            foreach ( var entry in RemovedFromChildren ) {
                var node = entry.Key;
                node.Parents.AddRange( entry.Value );
                node.Graph?.NowOutdated();
            }
            foreach ( var entry in RemovedFromParents ) {
                var nodeSelect = entry.Key;
                var idx = ParentsSelectIdx[nodeSelect];
                if (idx == null ) {
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

        private void Remove() {
            Group.RemoveAndUpdate( Item );
            Item.DisableNode();
            View.RemoveFromAvfx( Item );
            View.ResetSelected();
        }
    }
}
