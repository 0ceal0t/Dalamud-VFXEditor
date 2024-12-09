using Dalamud.Interface;
using ImGuiNET;
using Dalamud.Interface.Utility.Raii;
using System.IO;
using System.Linq;
using System.Numerics;
using VfxEditor.FileManager;
using VfxEditor.Formats.AvfxFormat.Texture;
using VfxEditor.Select;
using VfxEditor.Spawn;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat {
    public class AvfxDocument : FileManagerDocument<AvfxFile, WorkspaceMetaRenamed> {
        public override string Id => "Vfx";
        public override string Extension => "avfx";

        private string SpawnPath => ReplacePath;
        private bool SpawnDisabled => string.IsNullOrEmpty( SpawnPath );

        public AvfxDocument( AvfxManager manager, string writeLocation ) : base( manager, writeLocation ) { }

        public AvfxDocument( AvfxManager manager, string writeLocation, string localPath, WorkspaceMetaRenamed data ) : this( manager, writeLocation ) {
            LoadWorkspace( localPath, data.RelativeLocation, data.Name, data.Source, data.Replace, data.Disabled );
            File?.ReadRenamingMap( data.Renaming );
        }

        public override void CheckKeybinds() {
            base.CheckKeybinds();

            if( Plugin.Configuration.SpawnOnSelfKeybind.KeyPressed() ) {
                VfxSpawn.Clear();
                if( !SpawnDisabled ) VfxSpawn.OnSelf( SpawnPath, true );
            }

            if( Plugin.Configuration.SpawnOnGroundKeybind.KeyPressed() ) {
                VfxSpawn.Clear();
                if( !SpawnDisabled ) VfxSpawn.OnGround( SpawnPath, true );

            }

            if( Plugin.Configuration.SpawnOnTargetKeybind.KeyPressed() ) {
                VfxSpawn.Clear();
                if( !SpawnDisabled ) VfxSpawn.OnTarget( SpawnPath, true );
            }
        }

        protected override AvfxFile FileFromReader( BinaryReader reader, bool verify ) => new( reader, verify );

        public void Import( string path ) {
            if( File != null && System.IO.File.Exists( path ) ) File.Import( path );
        }

        public void ShowExportDialog( AvfxNode node ) => File.ShowExportDialog( node );

        public void OpenTemplate( string path ) =>
            SetSource( new SelectResult( SelectResultType.Local, "", "[TEMPLATE]", Path.Combine( Plugin.RootLocation, "Files", path ) ) );

        public override WorkspaceMetaRenamed GetWorkspaceMeta( string newPath ) => new() {
            Name = Name,
            RelativeLocation = newPath,
            Replace = Replace,
            Source = Source,
            Renaming = File.GetRenamingMap(),
            Disabled = Disabled
        };

        // ========= DRAWING =============

        protected override void DrawExtraColumn() {
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 4, 3 ) );
            using( var group = ImRaii.Group() ) {
                if( VfxSpawn.IsActive ) {
                    if( ImGui.Button( "Remove", new Vector2( 60, ImGui.GetFrameHeight() ) ) ) VfxSpawn.Clear();
                }
                else {
                    using var style = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, SpawnDisabled );
                    if( ImGui.Button( "Spawn", new Vector2( 60, ImGui.GetFrameHeight() ) ) && !SpawnDisabled ) ImGui.OpenPopup( "SpawnPopup" );
                }

                using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 4 ) ) ) {
                    ImGui.SameLine();
                }

                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( ImGui.Button( FontAwesomeIcon.Cog.ToIconString(), new Vector2( 28, ImGui.GetFrameHeight() ) ) ) ImGui.OpenPopup( "SettingsPopup" );
                }

                if( ImGui.Button( "Library", new Vector2( 90, ImGui.GetFrameHeight() ) ) ) Plugin.LibraryManager.Show();
            }

            var height = ImGui.GetItemRectSize().Y;

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( ImGui.GetStyle().ItemSpacing.Y, 4 ) ) ) {
                ImGui.SameLine();
            }
            Plugin.TrackerManager.Vfx.DrawEye( new Vector2( 28, height ) );

            padding.Pop(); // so it doesn't mess with the popups

            VfxSpawn.DrawPopup( SpawnPath, true );

            if( ImGui.BeginPopup( "SettingsPopup" ) ) {
                if( ImGui.Checkbox( "Loop", ref Plugin.Configuration.VfxSpawnLoop ) ) Plugin.Configuration.Save();
                ImGui.SetNextItemWidth( 150 );
                if( ImGui.InputFloat( "Delay", ref Plugin.Configuration.VfxSpawnDelay ) ) Plugin.Configuration.Save();
                ImGui.EndPopup();
            }
        }

        protected override string GetWarningText() {
            return AvfxWarning.GetWarningText(File);
        }

        protected override void DisplayFileControls() {
            base.DisplayFileControls();

            if( File.TextureView.Group.Items.Where( x => !x.FileExists() ).Any() ) {
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Images.ToIconString() ) ) {
                        var paths = File.TextureView.Group.Items
                            .Where( x => !x.FileExists() )
                            .Select( x => x.Path.Value.Trim( '\0' ) )
                            .Where( x => !string.IsNullOrEmpty( x ) ).ToList();

                        Plugin.AddModal( new ImportMissingTexturesModal( paths ) );
                    }
                }
                UiUtils.Tooltip( "Import missing textures" );
            }
        }
    }
}
