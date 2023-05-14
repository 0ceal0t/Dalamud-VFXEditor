using System.Collections.Generic;

namespace VfxEditor.Parsing {
    public interface IParsedUiBase {
        public void Draw( CommandManager manager );

        public static void DrawList<T>( List<T> items, CommandManager manager ) where T : IParsedUiBase {
            items.ForEach( x => x.Draw( manager ) );
        }
    }
}
