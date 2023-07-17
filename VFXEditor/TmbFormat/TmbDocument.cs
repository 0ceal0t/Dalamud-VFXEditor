using ImGuiNET;
using OtterGui.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Select;
using VfxEditor.Spawn;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbDocument : FileManagerDocument<TmbFile, WorkspaceMetaBasic> {
        public uint AnimationId = 0;
        private bool AnimationDisabled => string.IsNullOrEmpty( ReplacePath ) || AnimationId == 0;

        public TmbDocument( TmbManager manager, string writeLocation ) : base( manager, writeLocation, "Tmb", "tmb" ) { }

        public TmbDocument( TmbManager manager, string writeLocation, string localPath, string name, SelectResult source, SelectResult replace ) :
            base( manager, writeLocation, localPath, name, source, replace, "Tmb", "tmb" ) {
            AnimationId = TmbSpawn.GetIdFromTmbPath( ReplacePath );
        }

        protected override TmbFile FileFromReader( BinaryReader reader ) => new( reader );

        public override WorkspaceMetaBasic GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source
        };

        protected override void DrawExtraColumn() {
            using var framePadding = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 4, 3 ) );
            using( var group = ImRaii.Group() ) {
                using( var style = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, AnimationDisabled ) ) {
                    if( ImGui.Button( "Play", new Vector2( 90, ImGui.GetFrameHeight() ) ) && !AnimationDisabled ) TmbSpawn.Apply( AnimationId );
                }

                if( ImGui.Button( "Reset", new Vector2( 90, ImGui.GetFrameHeight() ) ) ) TmbSpawn.Reset();
            }

            var height = ImGui.GetItemRectSize().Y;

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemSpacing.Y, 4 ) ) ) {
                ImGui.SameLine();
            }
            Plugin.Tracker.Action.DrawEye( new Vector2( 28, height ) );
        }

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DisplayAnimationWarning();
            base.DrawBody();
        }
    }
}
