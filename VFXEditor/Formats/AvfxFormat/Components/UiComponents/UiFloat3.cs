using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiFloat3 : IUiItem {
        public readonly UiParsedFloat3 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat3( string name, AvfxFloat x, AvfxFloat y, AvfxFloat z ) {
            Literals = new() { x, y, z };
            Parsed = new( name, x.Parsed, y.Parsed, z.Parsed );
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
