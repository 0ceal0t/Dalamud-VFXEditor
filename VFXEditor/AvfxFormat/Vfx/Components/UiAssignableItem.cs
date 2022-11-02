using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiAssignableItem : UiItem {
        public abstract void DrawAssigned( string id );
        public abstract void DrawUnassigned( string id );
        public abstract bool IsAssigned();

        public override void DrawInline( string id ) {
            if( IsAssigned() ) DrawAssigned( id );
            else DrawUnassigned( id );
        }
    }
}
