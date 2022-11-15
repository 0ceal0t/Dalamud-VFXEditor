using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class UiFloat2 : IAvfxUiBase {
        public readonly UiParsedFloat2 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat2( string name, AvfxFloat l1, AvfxFloat l2 ) {
            Literals = new() { l1, l2 };
            Parsed = new( name, l1.Parsed, l2.Parsed );
        }

        public void Draw( string id ) {
            // Unassigned
            AvfxBase.AssignedCopyPaste( Literals[0], $"{Parsed.Name}_1" );
            AvfxBase.AssignedCopyPaste( Literals[1], $"{Parsed.Name}_2" );
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name, id ) ) return;

            Parsed.Draw( id, CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name, id );
        }
    }
}
