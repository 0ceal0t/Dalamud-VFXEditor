using System;
using System.Collections.Generic;
using VfxEditor.FileManager;
using VfxEditor.Ui.Components;

namespace VfxEditor.UldFormat.Component.Node {
    public class UldNodeSplitView : SimpleSplitView<UldNode> {
        private readonly List<UldComponent> Components;
        private readonly UldWorkspaceItem Parent;

        public UldNodeSplitView( List<UldNode> items, List<UldComponent> components, UldWorkspaceItem parent ) : base( "Node", items, true, true ) {
            Components = components;
            Parent = parent;
        }

        protected override void OnNew() {
            CommandManager.Uld.Add( new GenericAddCommand<UldNode>( Items, new UldNode( Components, Parent ) ) );
        }

        protected override void OnDelete( UldNode item ) {
            CommandManager.Uld.Add( new GenericRemoveCommand<UldNode>( Items, item ) );
        }

        protected override string GetText( UldNode item, int idx ) => item.GetText();
    }
}
