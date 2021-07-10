using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor {
    public struct InputTrackerItem {
        public ImGuiKey Key;
        public ImGuiKeyModFlags Mods;
        public bool LastPressed;
    }

    public partial class Plugin {
        public Dictionary<string, InputTrackerItem> InputTrackerState = new();

        public void AddInputTrackerItem(string name, ImGuiKey key, ImGuiKeyModFlags mods) {
            InputTrackerState[name] = new InputTrackerItem
            {
                Key = key,
                Mods = mods,
                LastPressed = false
            };
        }

        //public bool GetCurrentInputTrackerState(InputTrackerItem item) {
        //}
    }
}
