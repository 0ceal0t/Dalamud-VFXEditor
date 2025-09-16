using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Library.Components;

namespace VfxEditor.Library.Node {
    public class NodeRoot : LibraryRoot {
        public NodeRoot( List<LibraryProps> items ) : base( "Nodes", items ) { }

        public override bool Draw( LibraryManager library, string searchInput ) {
            using var child = ImRaii.Child( "Child", ImGui.GetContentRegionAvail(), false );

            if( Children.Count == 0 ) ImGui.TextDisabled( "No nodes saved..." );
            return base.Draw( library, searchInput );
        }
    }
}
