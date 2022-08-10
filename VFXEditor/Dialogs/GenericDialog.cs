using Dalamud.Logging;
using ImGuiNET;
using System.Numerics;

namespace VFXEditor.Dialogs {

    public abstract class GenericDialog {
        protected bool Visible = false;
        protected string Name;
        protected Vector2 Size;
        protected bool MenuBar;

        public bool IsVisible => Visible;

        public GenericDialog( string name, bool menuBar = false, int startingHeight = 600, int startingWidth = 400 ) {
            Name = name;
            MenuBar = menuBar;
            Size = new( startingHeight, startingWidth );
        }

        public void Show() {
            Visible = true;
        }

        public void Hide() {
            Visible = false;
        }

        public void Toggle() {
            Visible = !Visible;
        }

        public void SetVisible( bool visible ) {
            Visible = visible;
        }

        public void Draw() {
            if( !Visible ) return;
            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( ImGui.Begin( Name, ref Visible, ( MenuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) | ImGuiWindowFlags.NoDocking ) ) {
                Plugin.CheckClearKeyState();
                DrawBody();
            }
            ImGui.End();
        }

        public abstract void DrawBody();
    }
}
