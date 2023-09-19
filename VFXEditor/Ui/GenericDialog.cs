using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui {
    public abstract class GenericDialog {
        protected bool Visible = false;
        protected string Name;
        protected readonly Vector2 Size;
        protected readonly bool MenuBar;

        public bool IsVisible => Visible;

        private bool FocusNextFrame = false;

        public GenericDialog( string name, bool menuBar, int width, int height ) {
            Name = name;
            MenuBar = menuBar;
            Size = new( width, height );
        }

        public void Show() {
            SetVisible( true );
            FocusNextFrame = true;
        }

        public void Hide() => SetVisible( false );

        public void SetVisible( bool visible ) { Visible = visible; }

        protected virtual void PreDraw() { }

        protected virtual void PostDraw() { }

        public void Draw() {
            if( !Visible ) return;

            PreDraw();

            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( FocusNextFrame ) {
                ImGui.SetNextWindowCollapsed( false );
                ImGui.SetNextWindowFocus();
                FocusNextFrame = false;
            }

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
