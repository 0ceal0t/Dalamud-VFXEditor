using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public abstract class UIBase {
        public const uint LightGreen = 0xFFCCFFCE;
        public const uint LightRed = 0xFFCCD4FF;

        public abstract void Draw( string parentId );

        public static void DrawList( List<UIBase> items, string parentId ) {
            foreach( var item in items ) { // Draw assigned items
                if( item is UIItem optionalItem && !optionalItem.IsAssigned() ) continue;
                item.Draw( parentId );
            }

            foreach( var item in items ) { // Draw unassigned items
                if( item is UIItem optionalItem && !optionalItem.IsAssigned() ) item.Draw( parentId );
            }
        }

        public static void PushAssignedColor( bool assigned ) {
            if( Plugin.Configuration.ShowVfxAssigned ) ImGui.PushStyleColor( ImGuiCol.Text, assigned ? LightGreen : LightRed );
        }

        public static void PopAssignedColor() {
            if( Plugin.Configuration.ShowVfxAssigned ) ImGui.PopStyleColor();
        }
    }
}
