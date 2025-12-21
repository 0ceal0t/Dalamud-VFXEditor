using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Formats.KdbFormat.Nodes;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.KdbFormat.Components {
    public class ParsedFnvKdbNodes : ParsedFnvHash {
        private readonly List<KdbNode> Nodes;

        public ParsedFnvKdbNodes( string name, List<KdbNode> nodes ) : base( name ) {
            Nodes = nodes;
        }

        protected override void DrawBody() {
            var selected = Nodes.FirstOrDefault( x => x.NameHash.Hash == Hash, null );
            var text = selected == null ? $"[NONE] 0x{Hash:X8}" : $"{selected.GetText()} [0x{selected.NameHash.Hash:X8}]";

            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;
            if( ImGui.Selectable( "[NONE]" ) ) Update( ("", 0) );
            foreach( var (node, idx) in Nodes.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                if( ImGui.Selectable( $"{node.GetText()} [0x{node.NameHash.Hash:X8}]", node == selected ) ) Update( node.NameHash.Value );
                if( node == selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
