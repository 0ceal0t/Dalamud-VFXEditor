using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiNodeRemover<T> where T : AvfxNode {
        private readonly UiNodeGroup<T> Group;
        private readonly IUiNodeView<T> View;
        private readonly T Item;
        private readonly int Idx;

        private readonly Dictionary<AvfxNode, List<UiNodeSelect>> ChildToRemovedSelectors = new();
        private readonly Dictionary<UiNodeSelect, List<AvfxNode>> RemovedFromParents = new();
        private readonly Dictionary<UiNodeSelect, List<int>> ParentsSelectIdx = new();

        public UiNodeRemover( IUiNodeView<T> view, UiNodeGroup<T> group, T item ) {
            View = view;
            Group = group;
            Item = item;
            Idx = group.Items.IndexOf( item );
        }

        public void Add() {
            Group.AddAndUpdate( Item, Idx );

            // Enable node
            Item.IsDeleted = false;
            foreach( var entry in ChildToRemovedSelectors ) {
                var node = entry.Key;
                node.Parents.AddRange( entry.Value );
                node.Graph?.NowOutdated();
            }
            foreach( var entry in RemovedFromParents ) {
                var nodeSelect = entry.Key;
                var idx = ParentsSelectIdx[nodeSelect];
                if( idx == null ) {
                    nodeSelect.EnableNode( Item, 0 ); // single NodeSelect
                }
                else {
                    foreach( var i in idx ) nodeSelect.EnableNode( Item, i ); // NodeSelectList
                }

                nodeSelect.Node.ChildNodes.AddRange( entry.Value );
            }
            foreach( var selector in Item.Selectors ) selector.LinkOnChange();
        }

        public void Remove() {
            // Reset
            ChildToRemovedSelectors.Clear();
            RemovedFromParents.Clear();
            ParentsSelectIdx.Clear();

            // Store which of the selectors in the item will be disconnected from their children
            foreach( var node in Item.ChildNodes ) {
                ChildToRemovedSelectors[node] = node.Parents.Where( nodeSelect => nodeSelect.Node == Item ).ToList();
            }

            foreach( var nodeSelect in Item.Parents ) {
                RemovedFromParents[nodeSelect] = nodeSelect.Node.ChildNodes.Where( node => node == Item ).ToList();
                ParentsSelectIdx[nodeSelect] = nodeSelect.GetSelectedIdx( Item );
            }

            Group.RemoveAndUpdate( Item );
            // This disconnects all of the selectors and removes the parent/child linkages
            Item.Disconnect();
            View.ResetSelected();
        }
    }
}
