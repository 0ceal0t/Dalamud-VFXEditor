using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.AVFX.VFX {
    public abstract class UIAssignableItem : UIItem {
        public abstract void DrawAssigned( string id );
        public abstract void DrawUnassigned( string id );
        public abstract bool IsAssigned();

        public override void DrawInline( string id ) {
            if( IsAssigned() ) DrawAssigned( id );
            else DrawUnassigned( id );
        }
    }
}
