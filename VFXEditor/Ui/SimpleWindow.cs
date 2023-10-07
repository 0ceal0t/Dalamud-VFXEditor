using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui {
    public abstract class SimpleWindow {
        protected bool IsOpen = false;
        protected string Name;
        protected readonly ImGuiWindowFlags Flags;
        protected readonly Vector2 Size;

        private bool FocusNextFrame = false;

        public SimpleWindow( string name, bool menuBar, Vector2 size ) {
            Name = name;
            Flags = ( menuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) | ImGuiWindowFlags.NoDocking;
            Size = size;
        }

        public void Show() {
            IsOpen = true;
            FocusNextFrame = true;
        }

        public void Hide() => IsOpen = false;

        public void Draw() {
            if( !IsOpen ) return;

            ImGui.SetNextWindowSize( Size, ImGuiCond.FirstUseEver );

            if( FocusNextFrame ) {
                ImGui.SetNextWindowCollapsed( false );
                ImGui.SetNextWindowFocus();
                FocusNextFrame = false;
            }

            if( ImGui.Begin( Name, ref IsOpen, Flags ) ) {
                Plugin.CheckClearKeyState();
                DrawBody();
            }
            ImGui.End();
        }

        public abstract void DrawBody();
    }
}
