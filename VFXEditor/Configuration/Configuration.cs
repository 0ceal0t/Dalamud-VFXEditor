using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using VFXEditor.UI;

namespace VFXEditor{
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
        public bool PreviewTextures { get; set; } = true;
        public bool VerifyOnLoad { get; set; } = true;
        public bool LogAllFiles { get; set; } = false;
        public bool HideWithUI { get; set; } = true;

        public List<VFXSelectResult> RecentSelects = new List<VFXSelectResult>();
        public Dictionary<string, SavedItem> SavedItems = new Dictionary<string, SavedItem>();

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;
        [NonSerialized]
        public static Configuration Config;

        public void Initialize( DalamudPluginInterface pluginInterface, string writeLocation )
        {
            _pluginInterface = pluginInterface;
            Config = this;
            _pluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }

        public void AddRecent(VFXSelectResult result ) {
            if( !RecentSelects.Contains( result ) ) {
                RecentSelects.Add( result );
            }
            if(RecentSelects.Count > 10 ) {
                RecentSelects.RemoveRange( 0, RecentSelects.Count - 10 );
            }
            Save();
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig( this );
            _pluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }
    }
}