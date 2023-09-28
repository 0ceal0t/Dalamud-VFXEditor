using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiFloat2 : IUiItem {
        public readonly UiParsedFloat2 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat2( string name, AvfxFloat x, AvfxFloat y ) {
            Literals = new() { x, y };
            Parsed = new( name, x.Parsed, y.Parsed );
        }

        public void Draw() {
            // Unassigned
            AvfxBase.AssignedCopyPaste( Literals[0], $"{Parsed.Name}_1" );
            AvfxBase.AssignedCopyPaste( Literals[1], $"{Parsed.Name}_2" );
            if( AvfxBase.DrawAddButton( Literals, Parsed.Name ) ) return;

            Parsed.Draw( CommandManager.Avfx );

            AvfxBase.DrawRemoveContextMenu( Literals, Parsed.Name );
        }
    }
}
