using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiFloat2 : IUiItem {
        public readonly UiParsedFloat2 Parsed;
        private readonly AvfxFloat Literal;
        private readonly List<AvfxFloat> Extra;

        public UiFloat2( string name, AvfxFloat x, AvfxFloat y ) {
            Literal = x;
            Extra = new() { y };
            Parsed = new( name, x.Parsed, y.Parsed );
        }

        public void Draw() {
            Literal.AssignedCopyPaste( $"{Parsed.Name}_1" );
            Extra[0].AssignedCopyPaste( $"{Parsed.Name}_2" );
            if( Literal.DrawAddButton( Extra, Parsed.Name ) ) return;

            Parsed.Draw();

            Literal.DrawRemoveContextMenu( Extra, Parsed.Name );
        }
    }
}
