using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat2 {
    public class UiFloat3 : IUiBase {
        public readonly UiParsedFloat3 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat3( string name, AvfxFloat l1, AvfxFloat l2, AvfxFloat l3 ) {
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
