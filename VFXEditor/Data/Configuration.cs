using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using VFXSelect.UI;
using ImGuiFileDialog;

namespace VFXEditor {
    [Serializable]
    public class Configuration : IPluginConfiguration {
        public static Configuration Config => Instance;
        private static Configuration Instance = null;

        public static void Initialize( DalamudPluginInterface pluginInterface ) {
            Instance = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Instance.InitializeInstance( pluginInterface );
            Directory.CreateDirectory( Instance.WriteLocation );
            PluginLog.Log( "Write location: " + Instance.WriteLocation );
        }

        public static void Dispose() {
            Instance = null;
        }

        // ====== INSTANCE ======

        public int Version { get; set; } = 0;
        public bool IsEnabled { get; set; } = true;

        public bool VerifyOnLoad = true;
        public bool LogAllFiles = false;
        public bool HideWithUI = true;
        public int SaveRecentLimit = 10;
        public bool OverlayLimit = true;
        public string WriteLocation = Path.Combine( new[] {
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XIVLauncher",
            "pluginConfigs",
            "VFXEditor",
        } );
        public List<VFXSelectResult> RecentSelects = new();
        public bool FilepickerImagePreview = true;

        [NonSerialized]
        private DalamudPluginInterface PluginInterface;

        private void InitializeInstance( DalamudPluginInterface pluginInterface ) {
            PluginInterface = pluginInterface;
            PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
            FileDialogManager.ImagePreview = FilepickerImagePreview;
        }

        public void AddRecent( VFXSelectResult result ) {
            if( RecentSelects.Contains( result ) ) {
                RecentSelects.Remove( result ); // want to move it to the top
            }
            RecentSelects.Add( result );
            if( RecentSelects.Count > SaveRecentLimit ) {
                RecentSelects.RemoveRange( 0, RecentSelects.Count - SaveRecentLimit );
            }
            Save();
        }

        public void Save() {
            PluginInterface.SavePluginConfig( this );
            PluginInterface.UiBuilder.DisableUserUiHide = !HideWithUI;
        }
    }
}