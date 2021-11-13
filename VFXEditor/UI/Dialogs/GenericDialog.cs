using System.Numerics;
using ImGuiNET;

namespace VFXEditor.UI {

    public abstract class GenericDialog {
        protected bool Visible = false;
        protected string DialogName;
        protected Vector2 Size = new( 600, 400 );
        protected bool MenuBar = false;

        public GenericDialog( string name, bool menuBar = false ) {
            DialogName = name;
            MenuBar = menuBar;
         }

        public void Show() {
            Visible = true;
        }

        public void Draw() {
            if( !Visible ) return;
            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( ImGui.Begin( DialogName, ref Visible, MenuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) ) {
                DrawBody();
            }
            ImGui.End();
        }

        public abstract void DrawBody();
    }
}
