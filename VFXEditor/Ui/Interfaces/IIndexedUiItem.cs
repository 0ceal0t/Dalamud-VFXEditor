using OtterGui;
using System.Collections.Generic;

namespace VfxEditor.Ui.Interfaces {
    public interface IIndexUiItem : INamedUiItem {
        public int GetIdx();

        public void SetIdx( int idx );

        public static void UpdateIdx<T>( List<T> items ) where T : IIndexUiItem {
            foreach( var (item, idx) in items.WithIndex() ) item.SetIdx( idx );
        }
    }
}
