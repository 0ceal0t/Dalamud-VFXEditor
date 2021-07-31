using System;
using System.Collections.Generic;
using VFXEditor.Data;

namespace VFXEditor.UI.VFX {
    public abstract class UIBase {
        public bool Assigned = true;

        public virtual void Init() {
            Assigned = true;
        }
        public abstract void Draw( string parentId );

        public static void DrawList( List<UIBase> items, string parentId ) {
            foreach( var item in items ) {
                if( item.Assigned ) {
                    item.Draw( parentId );
                }
            }
            foreach( var item in items ) {
                if( !item.Assigned ) {
                    item.Draw( parentId );
                }
            }
        }
    }
}
