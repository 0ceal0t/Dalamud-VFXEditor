using System;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
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
