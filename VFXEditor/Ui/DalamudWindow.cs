using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
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

        // ImGui identifies windows by name (or by everything after "###"). If the same id gets drawn twice in one frame,
        // ImGui appends the second pass to the first, which shows up as duplicated content further down the window.
        // Track which id was drawn on which frame so a second pass can be skipped and logged.
        private static readonly Dictionary<string, (int Frame, DalamudWindow Owner)> DrawnThisFrame = [];
        private static readonly HashSet<string> ReportedDuplicates = [];

        private static string GetImGuiId( string name ) {
            var idx = name.IndexOf( "###", StringComparison.Ordinal );
            return idx >= 0 ? name[idx..] : name;
        }

        private bool CheckDuplicateDraw() {
            var id = GetImGuiId( WindowName );
            var frame = ImGui.GetFrameCount();

            if( DrawnThisFrame.TryGetValue( id, out var last ) && last.Frame == frame ) {
                if( ReportedDuplicates.Add( id ) ) {
                    Dalamud.Error( $"Window \"{WindowName}\" was drawn more than once in frame {frame} " +
                        $"({( ReferenceEquals( last.Owner, this ) ? "same window object" : $"different window objects: {last.Owner.GetType().Name} and {GetType().Name}" )}). " +
                        $"Skipping the duplicate. Stack:\n{Environment.StackTrace}" );
                }
                return true;
            }

            DrawnThisFrame[id] = (frame, this);
            return false;
        }

        public override void Draw() {
            if( CheckDuplicateDraw() ) return;

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
