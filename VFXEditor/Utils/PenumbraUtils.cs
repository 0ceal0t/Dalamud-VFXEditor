using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Ui.Export;

namespace VfxEditor.Utils {
    public class PenumbraMod {
        public Dictionary<string, List<string>> SourceFiles;
        public Dictionary<string, List<string>> ReplaceFiles;
        public PenumbraMeta Meta;
    }

    public static class PenumbraUtils {
        public static void WriteBytes( byte[] data, string modRootFolder, string groupOption, string gamePath, Dictionary<string, string> files ) {
            var filePath = string.IsNullOrEmpty( groupOption ) ? gamePath : Path.Combine( groupOption.ToLower(), gamePath );
            var path = Path.Combine( modRootFolder, filePath );
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            File.WriteAllBytes( path, data );

            files[gamePath] = filePath.Replace( '/', '\\' );
        }

        public static void CopyFile( string localPath, string modRootFolder, string groupOption, string gamePath, Dictionary<string, string> files ) {
            var filePath = string.IsNullOrEmpty( groupOption ) ? gamePath : Path.Combine( groupOption.ToLower(), gamePath );
            var path = Path.Combine( modRootFolder, filePath );
            Directory.CreateDirectory( Path.GetDirectoryName( path ) );
            File.Copy( localPath, path, true );

            files[gamePath] = filePath.Replace( '/', '\\' );
        }

        // $"{group} {option}" -> (gamePath, localPath)
        public static void LoadFromName( string itemName, List<string> extensions, out PenumbraMod loaded ) {
            loaded = new();
            var files = new Dictionary<string, List<(string, string)>>();

            var baseModPath = Plugin.PenumbraIpc.GetModDirectory();
            if( string.IsNullOrEmpty( baseModPath ) ) return;

            try {
                var modPath = Path.Join( baseModPath, itemName );
                var metaText = File.ReadAllText( Path.Join( modPath, "meta.json" ) );
                var fileVersion = JObject.Parse( metaText )["FileVersion"]?.Value<int>() ?? 3;

                if( fileVersion <= 3 ) {
                    // Legacy format: meta.json + separate default_mod.json/group_*.json files
                    loaded.Meta = JsonConvert.DeserializeObject<PenumbraMeta>( metaText );
                    var modFiles = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
                    foreach( var modFile in modFiles ) {
                        try {
                            var modFileName = Path.GetFileName( modFile ).Replace( ".json", "" );
                            if( modFileName == "default_mod" ) {
                                var mod = JsonConvert.DeserializeObject<PenumbraModStruct>( File.ReadAllText( modFile ) );
                                AddSourceFiles( files, "default_mod", mod?.Files, modPath, extensions );
                            }
                            else {
                                var group = JsonConvert.DeserializeObject<PenumbraGroupStruct>( File.ReadAllText( modFile ) );
                                foreach( var option in group?.Options ?? [] ) {
                                    AddSourceFiles( files, $"{group.Name} / {option.Name}", option.Files, modPath, extensions );
                                }
                            }
                        }
                        catch( Exception e ) {
                            Dalamud.Error( e, modFile );
                        }
                    }
                }
                else {
                    // Current format (FileVersion 4+): everything embedded in a single meta.json
                    var meta = JsonConvert.DeserializeObject<PenumbraMetaV4>( metaText );
                    loaded.Meta = new PenumbraMeta {
                        FileVersion = 3,
                        Name = meta.Name,
                        Author = meta.Author,
                        Description = meta.Description,
                        Version = meta.Version
                    };

                    AddSourceFiles( files, "default_mod", meta.DefaultData?.Files, modPath, extensions );

                    foreach( var group in meta.Groups ?? [] ) {
                        switch( group.Type ) {
                            // Combining groups don't attach files to their options directly: each combination
                            // of options instead maps to one of these containers
                            case "Combining":
                                foreach( var (container, idx) in ( group.Containers ?? [] ).WithIndex() ) {
                                    var label = string.IsNullOrEmpty( container.Name ) ? $"Container {idx + 1}" : container.Name;
                                    AddSourceFiles( files, $"{group.Name} / {label}", container.Files, modPath, extensions );
                                }
                                break;
                            // Imc groups only ever carry meta manipulations, never file redirects
                            case "Imc":
                                break;
                            default: // Single / Multi
                                foreach( var option in group.Options ?? [] ) {
                                    AddSourceFiles( files, $"{group.Name} / {option.Name}", option.Files, modPath, extensions );
                                }
                                break;
                        }
                    }
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error reading Penumbra mods" );
            }

            loaded.SourceFiles = files.ToDictionary(
                x => x.Key,
                x => x.Value.Where( y => Path.Exists( y.Item2 ) ).Select( y => $"{y.Item1}|{y.Item2}" ).ToList() // actually use local path
            );

            loaded.ReplaceFiles = files.ToDictionary(
                x => x.Key,
                x => x.Value.Select( y => y.Item1 ).ToList()
            );
        }

        private static void AddSourceFiles( Dictionary<string, List<(string, string)>> files, string label, Dictionary<string, string> filesToAdd, string modPath, List<string> extensions ) {
            if( filesToAdd == null || filesToAdd.Count == 0 ) return;
            var matched = new List<(string, string)>();
            AddToFiles( filesToAdd, matched, modPath, extensions );
            if( matched.Count > 0 ) files[label] = matched;
        }

        public static void AddToFiles( Dictionary<string, string> filesToAdd, List<(string, string)> files, string modPath, List<string> extensions ) {
            if( filesToAdd == null ) return;

            foreach( var (gamePath, localFile) in filesToAdd ) {
                if( !extensions.Any( gamePath.EndsWith ) ) continue;
                files.Add( (gamePath, Path.Join( modPath, localFile )) );
            }
        }
    }
}
