using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiFloat2 : IUiItem {
        public readonly UiParsedFloat2 Parsed;
        private readonly List<AvfxBase> Literals;

        public UiFloat2( string name, AvfxFloat l1, AvfxFloat l2 ) {
            Literals = new() { l1, l2 };
            Parsed = new( name, l1.Parsed, l2.Parsed );
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
