using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor.Ui {
    public abstract class DalamudWindow : Window {
        private readonly bool IsMainWindow;
        private bool ExpandNextFrame = false;

        public bool Focused => IsOpen && LastFocused;
        private bool LastFocused = false;

        private Vector2? LastPosition;
        private Vector2? LastSize;

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

            LastFocused = ImGui.IsWindowFocused();
            LastPosition = ImGui.GetWindowPos();
            LastSize = ImGui.GetWindowSize();

            DrawBody();
        }

        public abstract void DrawBody();

        public override void PreDraw() {
            LastFocused = false;

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

        public virtual WorkspaceWindow ToMeta() => new() {
            Position = LastPosition,
            Size = LastSize,
        };

        public virtual void SetMeta( WorkspaceWindow? meta ) {
            if( meta?.Size != null ) {
                SizeCondition = ImGuiCond.Once;
                Size = meta?.Size;
            }
            if( meta?.Position != null ) {
                PositionCondition = ImGuiCond.Once;
                Position = meta?.Position;
            }
        }
    }
}
