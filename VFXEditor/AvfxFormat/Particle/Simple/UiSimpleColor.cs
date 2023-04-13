using System;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiSimpleColor : IUiItem {
        public readonly ParsedInt Frame;
        public readonly ParsedIntColor Color;

        public UiSimpleColor( ParsedInt frame, ParsedIntColor color ) {
            Frame = frame;
            Color = color;
        }

        public void Draw( string parentId ) {
            Frame.Draw( parentId, CommandManager.Avfx );
            Color.Draw( parentId, CommandManager.Avfx );
        }
    }
}
