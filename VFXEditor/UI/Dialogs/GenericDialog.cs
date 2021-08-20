using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI {

    public abstract class GenericDialog {
        public bool Visible = false;
        public string DialogName;
        public Vector2 Size = new( 600, 400 );

        public GenericDialog( string name ) {
            DialogName = name;
        }
        public void Show() {
            Visible = true;
        }
        public void Draw() {
            if( !Visible ) return;
            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( ImGui.Begin( DialogName, ref Visible ) ) {
                OnDraw();
            }
            ImGui.End();
        }

        public abstract void OnDraw();
    }
}
