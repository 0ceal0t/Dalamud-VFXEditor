using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class UiInt3 : IUiBase {
        public readonly UiParsedInt3 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiInt3( string name, AvfxInt l1, AvfxInt l2, AvfxInt l3 ) {
            Literals = new() { l1, l2, l3 };
            Parsed = new( name, l1.Parsed, l2.Parsed, l3.Parsed );
        }

        public void Draw( string id ) {
            // Unassigned
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name, id );
        }
    }
}
