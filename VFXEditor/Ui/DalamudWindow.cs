using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace VfxEditor.Ui {
    public abstract class DalamudWindow : Window {
        private bool ExpandNextFrame = false;

        public DalamudWindow( string name, bool menuBar, Vector2 size ) : base( name, ( menuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) | ImGuiWindowFlags.NoDocking ) {
            Size = size;
            SizeCondition = ImGuiCond.FirstUseEver;
            Plugin.WindowSystem.AddWindow( this );
        }

        public void Show() {
            IsOpen = true;
            ExpandNextFrame = true;
            BringToFront();
        }

        public void Hide() => IsOpen = false;

        public override void Draw() {
            Plugin.CheckClearKeyState();
            DrawBody();
        }

        public abstract void DrawBody();

        public override void PreDraw() {
            if( ExpandNextFrame ) {
                ImGui.SetNextWindowCollapsed( false );
                ExpandNextFrame = false;
            }
        }
    }
}
