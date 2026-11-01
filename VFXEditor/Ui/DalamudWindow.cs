using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Utils;

namespace VfxEditor.Ui {
    public abstract class DalamudWindow : Window {
        private readonly bool IsMainWindow;
        private bool ExpandNextFrame = false;

        public bool Focused => IsOpen && LastFocused;
        private bool LastFocused = false;

        private Vector2? LastPosition;
        private Vector2? LastSize;

        // Used so we can override the frames, since ImGuiCond.Once can only be fired 1 time per session
        private int OverrideFrames = 0;

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

            if( OverrideFrames > 0 && --OverrideFrames == 0 ) {
                Size = null;
                Position = null;
            }
        }

        public virtual WorkspaceWindow ToMeta() => new() {
            Position = LastPosition,
            Size = LastSize,
        };

        public virtual void SetMeta( WorkspaceWindow? meta ) {
            if( meta?.Size != null ) {
                Size = meta?.Size;
                SizeCondition = ImGuiCond.Always;
                OverrideFrames = 2;
            }
            if( meta?.Position != null ) {
                Position = meta?.Position;
                PositionCondition = ImGuiCond.Always;
                OverrideFrames = 2;
            }
        }
    }
}
