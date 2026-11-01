using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using System;
using System.Collections.Generic;
using System.Text;
using VfxEditor.Ui;

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
            // TODO
        }

        public override bool DrawConditions() {
            return Status != PenumbraStatus.OK;
        }
    }
}