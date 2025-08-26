using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace VfxEditor.Ui {
    public abstract class DalamudWindow : Window {
        private readonly bool IsMainWindow;
        private bool ExpandNextFrame = false;

        public DalamudWindow( string name, bool menuBar, Vector2 size, WindowSystem windowSystem, bool isMainWindow = false ) :
            base( name, ( menuBar ? ImGuiWindowFlags.MenuBar : ImGuiWindowFlags.None ) | ImGuiWindowFlags.NoDocking ) {

            Size = size;
            SizeCondition = ImGuiCond.FirstUseEver;
            windowSystem?.AddWindow( this );
            IsMainWindow = isMainWindow;
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

            if( IsMainWindow ) {
                if( Plugin.Configuration.LockMainWindows )
                    Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
                else
                    Flags &= ~( ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove );
            }
        }
    }
}
