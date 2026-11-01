using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System.Numerics;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Interop.Penumbra {
    // Shown whenever Penumbra is missing, unloaded, or has mods disabled, including when that happens mid-session
    public class PenumbraRequiredWindow : DalamudWindow {
        public const string RepoUrl = "https://raw.githubusercontent.com/xivdev/Penumbra/master/repo.json";

        private enum Status {
            Ok,
            NotLoaded,
            ModsDisabled,
        }

        private Status LastStatus = Status.Ok;

        public PenumbraRequiredWindow( WindowSystem windowSystem ) : base( "Penumbra Required##VFXEditor", false, new( 450, 220 ), windowSystem ) {
            SizeCondition = ImGuiCond.Appearing;
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        private static Status GetStatus() {
            var ipc = Plugin.PenumbraIpc;
            if( ipc == null || !ipc.PenumbraEnabled ) return Status.NotLoaded;
            if( !ipc.ModsEnabled ) return Status.ModsDisabled;
            return Status.Ok;
        }

        // Called every frame. Opens when the status gets worse (including on startup), closes once Penumbra is usable.
        // If the user closes it, it stays closed until the status changes again
        public void Update() {
            var status = GetStatus();
            if( status == LastStatus ) return;

            LastStatus = status;
            if( status == Status.Ok ) {
                IsOpen = false;
                PenumbraPreview.Recover();
            }
            else {
                Show();
            }
        }

        public override void DrawBody() {
            var status = GetStatus();

            if( status == Status.Ok ) {
                ImGui.TextColored( UiUtils.GREEN_COLOR, "Penumbra is loaded, previews are working again." );
                if( ImGui.Button( "Close" ) ) IsOpen = false;
                return;
            }

            ImGui.TextWrapped( "VFXEditor uses Penumbra to preview changes in-game. Editing and exporting still work, but nothing will show up in-game until this is fixed." );
            ImGui.Separator();

            if( status == Status.ModsDisabled ) {
                ImGui.TextColored( UiUtils.DALAMUD_YELLOW, "Mods are disabled in Penumbra." );
                ImGui.TextWrapped( "Enable mods in Penumbra's settings to see your changes." );
                return;
            }

            ImGui.TextColored( UiUtils.DALAMUD_YELLOW, "Penumbra is not installed or not loaded." );
            ImGui.TextWrapped( "If it is installed, make sure it is enabled in the plugin installer. Otherwise, add Penumbra's repository to Dalamud's custom plugin repositories (Settings > Experimental), then install it:" );

            ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X - 30 );
            var url = RepoUrl;
            ImGui.InputText( "##Repo", ref url, 255, ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Clipboard.ToIconString() ) ) ImGui.SetClipboardText( RepoUrl );
            }
            UiUtils.Tooltip( "Copy" );

            if( ImGui.Button( "Open Custom Repositories" ) ) Dalamud.PluginInterface.OpenDalamudSettingsTo( SettingsOpenKind.Experimental );
            ImGui.SameLine();
            if( ImGui.Button( "Open Plugin Installer" ) ) Dalamud.PluginInterface.OpenPluginInstallerTo( PluginInstallerOpenKind.AllPlugins, "Penumbra" );
        }
    }
}
