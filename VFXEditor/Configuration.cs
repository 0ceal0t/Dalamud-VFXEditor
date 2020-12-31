using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;

namespace VFXEditor
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool IsEnabled { get; set; } = true;

        // the below exist just to make saving less cumbersome

        [NonSerialized]
        private DalamudPluginInterface _pluginInterface;

        public void Initialize( DalamudPluginInterface pluginInterface )
        {
            _pluginInterface = pluginInterface;
        }

        public void Save()
        {
            _pluginInterface.SavePluginConfig( this );
        }
    }
}