using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiInt3 : IUiItem {
        public readonly UiParsedInt3 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiInt3( string name, AvfxInt l1, AvfxInt l2, AvfxInt l3 ) {
            Literals = new() { l1, l2, l3 };
            Parsed = new( name, l1.Parsed, l2.Parsed, l3.Parsed );
        }

        public void Draw() {
            // Unassigned
            AvfxBase.AssignedCopyPaste( Literals[0], $"{Parsed.Name}_1" );
            AvfxBase.AssignedCopyPaste( Literals[1], $"{Parsed.Name}_2" );
            AvfxBase.AssignedCopyPaste( Literals[2], $"{Parsed.Name}_3" );
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name ) ) return;

            Parsed.Draw( CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name );
        }
    }
}
