using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public interface IUiBase {
        public void Draw( string parentId );

        public static void DrawList<T>( List<T> items, string parentId ) where T : IUiBase {
            foreach( var item in items ) { // Draw assigned items
                if( item is AvfxAssignable optionalItem && !optionalItem.IsAssigned() ) continue;
                item.Draw( parentId );
            }

            foreach( var item in items ) { // Draw unassigned items
                if( item is AvfxAssignable optionalItem && !optionalItem.IsAssigned() ) item.Draw( parentId );
            }
        }
    }
}
