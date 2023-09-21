using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Tracker {
    public abstract class Tracker {
        public bool Enabled { get; private set; }

        public abstract void PopulateAll( HashSet<TrackerItem> displayItems );

        public abstract void Populate( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems, Dictionary<IntPtr, HashSet<TrackerItem>> addressToItems );

        public abstract void RemoveStale( DateTime removeTime );

        public abstract void Reset();

        public void Toggle() {
            Enabled = !Enabled;
            if( !Enabled ) Reset();
        }

        public void DrawEye( Vector2 size ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( !Enabled ? FontAwesomeIcon.Eye.ToIconString() : FontAwesomeIcon.Times.ToIconString(), size ) ) {
                Toggle();
                Dalamud.PluginInterface.UiBuilder.DisableCutsceneUiHide = Plugin.TrackerManager.AnyEnabled;
            }
        }
    }
}
