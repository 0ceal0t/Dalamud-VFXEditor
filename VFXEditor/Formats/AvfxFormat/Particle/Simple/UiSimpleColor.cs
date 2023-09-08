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

        public void Draw() {
            Frame.Draw( CommandManager.Avfx );
            Color.Draw( CommandManager.Avfx );
        }
    }
}
