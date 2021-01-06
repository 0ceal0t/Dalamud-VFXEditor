using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using VFXEditor.UI;

namespace VFXEditor
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;
        public bool IsEnabled { get; set; } = true;

        // ==================
        public bool PreviewTextures { get; set; } = true;
        public bool VerifyOnLoad { get; set; } = true;

        public List<VFXSelectResult> RecentSelects = new List<VFXSelectResult>();

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;

        public void Initialize( DalamudPluginInterface pluginInterface )
        {
            _pluginInterface = pluginInterface;
        }

        public void AddRecent(VFXSelectResult result )
        {
            if( !RecentSelects.Contains( result ) )
            {
                RecentSelects.Add( result );
            }
            if(RecentSelects.Count > 10 )
            {
                // cut it back down to 10
                RecentSelects.RemoveRange( 0, RecentSelects.Count - 10 );
            }
            Save();
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig( this );
        }
    }
}