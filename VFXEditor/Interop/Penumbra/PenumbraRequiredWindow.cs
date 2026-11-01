using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using VfxEditor.Ui;
using VfxEditor.Utils;

namespace VfxEditor.Interop.Penumbra {
    public class PenumbraRequiredWindow : DalamudWindow {
        public enum PenumbraStatus {
            OK,
            NOT_LOADED,
            MODS_DISABLED
        }

        public PenumbraStatus Status { get; private set; } = PenumbraStatus.OK;

        public PenumbraRequiredWindow() : base( "Penumbra Required", false, new( 450, 220 ), Plugin.WindowSystem ) {
            SizeCondition = ImGuiCond.Appearing;
            Flags |= ImGuiWindowFlags.NoCollapse;
        }

        private static PenumbraStatus GetStatus() {
            var ipc = Plugin.PenumbraIpc;
            if( ipc == null || !ipc.PenumbraEnabled ) return PenumbraStatus.NOT_LOADED;
            if( !ipc.ModsEnabled ) return PenumbraStatus.MODS_DISABLED;
            return PenumbraStatus.OK;
        }

        public void Tick() {
            var lastStatus = Status;
            Status = GetStatus();

            if( Status != lastStatus && Status != PenumbraStatus.OK ) {
                Show();
            }
        }

        public override void DrawBody() {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 10f, 10f ) );
            using var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 10f, 10f ) );
            using var child = ImRaii.Child( "PenumbraRequired", new( -1, -1 ), true );

            if( Status == PenumbraStatus.MODS_DISABLED ) {
                ImGui.TextColored( UiUtils.DALAMUD_YELLOW, "Mods are disabled in Penumbra" );
            }
            else if( Status == PenumbraStatus.NOT_LOADED ) {
                ImGui.TextColored( UiUtils.DALAMUD_YELLOW, "Penumbra is not installed or not loaded" );
            }

            ImGui.TextWrapped( "VFXEditor now uses Penumbra to preview changes in-game. This plugin will not function until Penumbra is installed and mods are enabled" );

            if( ImGui.Button( "Custom Repositories" ) ) Dalamud.PluginInterface.OpenDalamudSettingsTo( SettingsOpenKind.Experimental );
            ImGui.SameLine();
            if( ImGui.Button( "Plugin Installer" ) ) Dalamud.PluginInterface.OpenPluginInstallerTo( PluginInstallerOpenKind.AllPlugins, "Penumbra" );
            ImGui.SameLine();
            using var disabled = ImRaii.Disabled( Status == PenumbraStatus.NOT_LOADED );
            if( ImGui.Button( "Open Penumbra" ) ) Plugin.PenumbraIpc.OpenSettings();
        }

        public override bool DrawConditions() {
            return Status != PenumbraStatus.OK;
        }
    }
}