using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Spawn;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbDocument : FileManagerBasicDocument<TmbFile> {
        public override string Id => "Tmb";
        public override string Extension => "tmb";

        public uint AnimationId = 0;
        private bool AnimationDisabled => string.IsNullOrEmpty( ReplacePath ) || AnimationId == 0;

        public TmbDocument( TmbManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public TmbDocument( TmbManager manager, string writeLocation, string localPath, WorkspaceMetaBasic data ) : base( manager, writeLocation, localPath, data ) {
            AnimationId = TmbSpawn.GetIdFromTmbPath( ReplacePath );
        }

        protected override TmbFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

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
            Plugin.TrackerManager.Tmb.DrawEye( new Vector2( 28, height ) );
        }

        protected override float GetExtraControlsWidth() => 60 + 5;

        protected override void DrawExtraControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            using( ImRaii.Disabled( AnimationDisabled ) ) {
                if( ImGui.Button( FontAwesomeIcon.Play.ToIconString(), new Vector2( 30, ImGui.GetFrameHeight() ) ) && !AnimationDisabled ) TmbSpawn.Apply( AnimationId );
            }
            UiUtils.Tooltip( "Force your character to play the currently selected animation (both source and replaced TMB must be selected)" );

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 5f, 0f ) ) ) {
                ImGui.SameLine();
            }
            if( ImGui.Button( FontAwesomeIcon.Stop.ToIconString(), new Vector2( 30, ImGui.GetFrameHeight() ) ) ) TmbSpawn.Reset();
            UiUtils.Tooltip( "Reset the currently playing animation" );
        }

        protected override void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawAnimationWarning();
            base.DrawBody();
        }
    }
}