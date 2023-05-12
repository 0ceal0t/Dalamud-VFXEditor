using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui {
    public abstract class GenericDialog {
        protected bool Visible = false;
        protected string Name;
        protected Vector2 Size;
        protected bool MenuBar;

        public bool IsVisible => Visible;

        public GenericDialog( string name, bool menuBar, int startingWidth, int startingHeight ) {
            Name = name;
            MenuBar = menuBar;
            Size = new( startingWidth, startingHeight );
        }

        public void Show() => SetVisible( true );

        public void Hide() => SetVisible( false );

        public void Toggle() => SetVisible( !Visible );

        public void SetVisible( bool visible ) { Visible = visible; }

        protected virtual void PreDraw() { }
        protected virtual void PostDraw() { }

        public void Draw() {
            if( !Visible ) return;

            PreDraw();

            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( ImGui.Begin( Name, ref Visible, ( MenuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) | ImGuiWindowFlags.NoDocking ) ) {
                Plugin.CheckClearKeyState();
                DrawBody();
            }
            ImGui.End();

            PostDraw();
        }

        public abstract void DrawBody();
    }
}
