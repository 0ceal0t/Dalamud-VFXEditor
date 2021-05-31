using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using VFXSelect.UI;

namespace VFXEditor {
    public enum SavedItemType {
        None,
        Binder,
        Emitter,
        Effector,
        Particle,
        Timeline
    }

    public struct SavedItem {
        public string Name;
        public SavedItemType Type;
        public string Path;
    }

    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 0;
        public bool IsEnabled { get; set; } = true;

        // ==================
        public bool PreviewTextures = true;
        public bool VerifyOnLoad = true;
        public bool LogAllFiles = false;
        public bool HideWithUI = true;
        public int SaveRecentLimit = 10;
        public bool OverlayLimit = true;

        public List<VFXSelectResult> RecentSelects = new List<VFXSelectResult>();
        public Dictionary<string, SavedItem> SavedItems = new Dictionary<string, SavedItem>();

        public string WriteLocation = Path.Combine( new[] {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XIVLauncher",
            "pluginConfigs",
            "VFXEditor",
        });

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;
        [NonSerialized]
        public static Configuration Config;

        public void Initialize( DalamudPluginInterface pluginInterface ) {
            _pluginInterface = pluginInterface;
            Config = this;
            _pluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }

        public void AddRecent(VFXSelectResult result ) {
            if( RecentSelects.Contains( result ) ) {
                RecentSelects.Remove( result ); // want to move it to the top
            }
            RecentSelects.Add( result );
            if(RecentSelects.Count > SaveRecentLimit ) {
                RecentSelects.RemoveRange( 0, RecentSelects.Count - SaveRecentLimit );
            }
            Save();
        }

        public void Save() {
            _pluginInterface.SavePluginConfig( this );
            _pluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }
    }
}