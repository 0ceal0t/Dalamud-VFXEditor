using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public abstract class UIGenericSplitView : UIBase {
        public int LeftSize;

        public bool AllowNew;
        public bool AllowDelete;

        public UIGenericSplitView( bool allowNew, bool allowDelete, int leftSize ) {
            AllowNew = allowNew;
            AllowDelete = allowDelete;
            LeftSize = leftSize;
        }

        public abstract void DrawLeftCol( string parentId );
        public abstract void DrawRightCol( string parentId );
        public abstract void DrawNewButton( string parentId );
        public abstract void DrawDeleteButton( string parentId );

        bool DrawOnce = false;
        public override void Draw( string parentId = "" ) {
            ImGui.Columns( 2, parentId + "/Cols", true );
            // ===== C1 =========
            if( AllowNew ) {
                DrawNewButton( parentId );
            }
            ImGui.BeginChild( parentId + "/Tree" );
            // assigned, good to go
            DrawLeftCol( parentId );
            ImGui.EndChild();
            if( !DrawOnce ) {
                ImGui.SetColumnWidth( 0, LeftSize );
                DrawOnce = true;
            }
            // ===== C2 ============
            ImGui.NextColumn();
            ImGui.BeginChild( parentId + "/Split" );
            if( AllowDelete ) {
                DrawDeleteButton(parentId);
            }
            DrawRightCol( parentId );
            ImGui.EndChild();
            ImGui.Columns( 1 );
        }
    }
}
