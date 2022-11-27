using System.Collections.Generic;

namespace VfxEditor.Parsing {
    public interface IParsedUiBase {
        public void Draw( string parentId, CommandManager manager );

        public static void DrawList<T>( List<T> items, string parentId, CommandManager manager ) where T : IParsedUiBase {
            items.ForEach( x => x.Draw( parentId, manager ) );
        }
    }
}
