using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VfxEditor {
    // Settings are stored by Dalamud as pluginConfigs/<InternalName>.json. The plugin used to be published as "VFXEditor"
    // (main repository) and "VFXEditor-Beta", so on first run as "vfx-editor" there is no config yet. In that case the most
    // recently used previous config is imported instead of starting from defaults.
    public static class ConfigurationMigration {
        private static readonly string[] PreviousInternalNames = ["VFXEditor", "VFXEditor-Beta"];

        // Always returns a usable configuration, with Setup() already called
        public static Configuration Load() {
            // The user already has settings for this plugin: never import over them
            var configFile = Dalamud.PluginInterface.ConfigFile;
            if( configFile.Exists ) return LoadExisting( configFile );

            var previous = FindPrevious();
            if( previous == null ) return Defaults(); // new user, nothing to import

            return Import( previous );
        }

        private static Configuration LoadExisting( FileInfo configFile ) {
            try {
                var configuration = Dalamud.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
                configuration.Setup();
                return configuration;
            }
            catch( Exception e ) {
                // Previously this stopped the plugin from loading. The unreadable file is overwritten by the next save
                Dalamud.Error( e, $"Could not read {configFile.FullName}" );
                Dalamud.WarningNotification( "Your settings could not be read and were reset to defaults" );
                return Defaults();
            }
        }

        private static FileInfo FindPrevious() {
            var configDirectory = Dalamud.PluginInterface.ConfigFile.Directory;
            if( configDirectory == null || !configDirectory.Exists ) return null;

            return PreviousInternalNames
                .Select( name => new FileInfo( Path.Combine( configDirectory.FullName, $"{name}.json" ) ) )
                .Where( file => file.Exists )
                .OrderByDescending( file => file.LastWriteTimeUtc )
                .FirstOrDefault();
        }

        private static Configuration Import( FileInfo previous ) {
            var skipped = new List<string>();
            try {
                var configuration = JsonConvert.DeserializeObject<Configuration>( File.ReadAllText( previous.FullName ), new JsonSerializerSettings {
                    // Same as Dalamud's own loader (PluginConfigurations.LoadForType): no type name handling, so the "$type"
                    // entries written for the old assembly name are ignored instead of failing to resolve
                    TypeNameHandling = TypeNameHandling.None,
                    // Older files may contain null for settings that have defaults now; keep the defaults instead of nulls
                    NullValueHandling = NullValueHandling.Ignore,
                    // A setting that changed type since the old version is reset to its default instead of failing the whole import
                    // Malformed JSON (e.g. a truncated file) is not handled here, so the whole import fails rather than importing half a file
                    Error = ( _, args ) => {
                        if( args.ErrorContext.Handled || args.ErrorContext.Error is JsonReaderException ) return;
                        skipped.Add( args.ErrorContext.Path );
                        args.ErrorContext.Handled = true;
                    },
                } ) ?? throw new Exception( "File was empty" );

                configuration.Setup();
                configuration.Save(); // write vfx-editor.json so this only happens once

                if( skipped.Count == 0 ) {
                    Dalamud.Log( $"Imported settings from {previous.FullName}" );
                    Dalamud.InfoNotification( "Imported your settings from the previous version of VFXEditor" );
                }
                else {
                    var names = skipped.Distinct().ToList();
                    Dalamud.Log( $"Imported settings from {previous.FullName}, reset: {string.Join( ", ", names )}" );
                    Dalamud.WarningNotification( $"Imported your settings from the previous version of VFXEditor, but some could not be read and were reset: {string.Join( ", ", names.Take( 5 ) )}{( names.Count > 5 ? ", ..." : "" )}" );
                }
                return configuration;
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not import settings from {previous.FullName}" );
                Dalamud.WarningNotification( "Your settings from the previous version of VFXEditor could not be imported, so default settings are being used" );

                // Save the defaults so the import is not retried (and fails again) on every start. The old file is left untouched
                var defaults = Defaults();
                defaults.Save();
                return defaults;
            }
        }

        private static Configuration Defaults() {
            var configuration = new Configuration();
            configuration.Setup();
            return configuration;
        }
    }
}
