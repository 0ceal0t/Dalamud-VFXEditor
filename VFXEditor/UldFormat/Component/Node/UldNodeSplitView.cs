using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Component.Node {
    public class UldNodeSplitView : SimpleSplitView<UldNode> {
        private readonly List<UldComponent> Components;

        public UldNodeSplitView( List<UldNode> items, List<UldComponent> components) : base( "Node", items, true ) {
            Components = components;
        }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldNode>( Items, new UldNode( Components ) ) );
        }

        protected override void OnDelete( UldNode item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldNode>( Items, item ) );
        }

        protected override string GetText( UldNode item, int idx ) => $"Node {item.Id.Value}";
    }
}
