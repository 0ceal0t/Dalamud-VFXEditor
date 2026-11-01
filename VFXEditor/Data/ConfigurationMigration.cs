using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace VfxEditor {
    // Settings are stored by Dalamud as pluginConfigs/<InternalName>.json. The plugin used to be published as "VFXEditor"
    // (main repository) and "VFXEditor-Beta", so on first run as "vfx-editor" there is no config yet. Import the most
    // recently used previous one instead of starting from defaults.
    public static class ConfigurationMigration {
        private static readonly string[] PreviousInternalNames = ["VFXEditor", "VFXEditor-Beta"];

        public static Configuration FromPreviousPlugin() {
            try {
                var configDirectory = Dalamud.PluginInterface.ConfigFile.Directory;
                if( configDirectory == null || !configDirectory.Exists ) return null;

                var previous = PreviousInternalNames
                    .Select( name => new FileInfo( Path.Combine( configDirectory.FullName, $"{name}.json" ) ) )
                    .Where( file => file.Exists )
                    .OrderByDescending( file => file.LastWriteTimeUtc )
                    .FirstOrDefault();
                if( previous == null ) return null;

                // Same as Dalamud's own loader (PluginConfigurations.LoadForType): no type name handling, so the "$type"
                // entries written for the old assembly name are ignored instead of failing to resolve
                var configuration = JsonConvert.DeserializeObject<Configuration>( File.ReadAllText( previous.FullName ) );
                if( configuration == null ) return null;

                Dalamud.Log( $"Imported settings from {previous.FullName}" );
                return configuration;
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Could not import settings from a previous version of VFXEditor" );
                return null;
            }
        }
    }
}
