using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Tracker {
    public abstract class Tracker {
        public bool Enabled { get; private set; }

        public abstract void Reset();

        public abstract void RemoveStale( DateTime removeTime );

        public abstract void AddAll( HashSet<TrackerItem> displayItems );

        public abstract void Add( List<TrackerItemWithPosition> floatingItems, Dictionary<int, HashSet<TrackerItem>> actorToItems );

        public void Toggle() {
            Enabled = !Enabled;
            if( !Enabled ) Reset();
        }

        public void DrawEye( Vector2 size ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( !Enabled ? FontAwesomeIcon.Eye.ToIconString() : FontAwesomeIcon.Times.ToIconString(), size ) ) {
                Toggle();
                Plugin.PluginInterface.UiBuilder.DisableCutsceneUiHide = Plugin.Tracker.AnyEnabled;
            }
        }
    }
}
