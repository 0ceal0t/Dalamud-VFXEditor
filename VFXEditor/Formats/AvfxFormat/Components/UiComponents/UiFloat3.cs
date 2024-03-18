using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiFloat3 : IUiItem {
        private readonly UiParsedFloat3 Parsed;
        private readonly AvfxFloat Literal;
        private readonly List<AvfxFloat> Extra;

        public UiFloat3( string name, AvfxFloat x, AvfxFloat y, AvfxFloat z ) {
            Literal = x;
            Extra = [y, z];
            Parsed = new( name, x.Parsed, y.Parsed, z.Parsed );
        }

        public void Draw() {
            Literal.AssignedCopyPaste( $"{Parsed.Name}_1" );
            Extra[0].AssignedCopyPaste( $"{Parsed.Name}_2" );
            Extra[1].AssignedCopyPaste( $"{Parsed.Name}_3" );
            if( Literal.DrawAssignButton( Extra, Parsed.Name ) ) return;

            Parsed.Draw();

            Literal.DrawUnassignPopup( Extra, Parsed.Name );
        }
    }
}
