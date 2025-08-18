using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Ui.Export;

namespace VfxEditor.Utils {
    public class PenumbraMod
    {
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
        public static void LoadFromName( string itemName, List<string> extensions, out PenumbraMod loaded )
        {
            loaded = new();
            var files = new Dictionary<string, List<(string, string)>>();

            var baseModPath = Plugin.PenumbraIpc.GetModDirectory();
            if( string.IsNullOrEmpty( baseModPath ) ) return;

            try
            {
                var modPath = Path.Join( baseModPath, itemName );
                loaded.Meta = JsonConvert.DeserializeObject<PenumbraMeta>( File.ReadAllText( Path.Join( modPath, "meta.json" ) ) );

                var modFiles = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
                foreach( var modFile in modFiles )
                {
                    try
                    {
                        var modFileName = Path.GetFileName( modFile ).Replace( ".json", "" );
                        if( modFileName == "default_mod" )
                        {
                            var mod = JsonConvert.DeserializeObject<PenumbraModStruct>( File.ReadAllText( modFile ) );
                            if( mod.Files != null )
                            {
                                var defaultFiles = new List<(string, string)>();
                                AddToFiles( mod?.Files, defaultFiles, modPath, extensions );
                                files["default_mod"] = defaultFiles;
                            }
                        }
                        else
                        {
                            var group = JsonConvert.DeserializeObject<PenumbraGroupStruct>( File.ReadAllText( modFile ) );
                            if( group.Options != null )
                            {
                                foreach( var option in group.Options.Where( x => x.Files != null ) )
                                {
                                    var optionFiles = new List<(string, string)>();
                                    AddToFiles( option?.Files, optionFiles, modPath, extensions );
                                    files[$"{group.Name} / {option.Name}"] = optionFiles;
                                }
                            }
                        }
                    }
                    catch( Exception e )
                    {
                        Dalamud.Error( e, modFile );
                    }
                }
            }
            catch( Exception e )
            {
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

        public static void AddToFiles( Dictionary<string, string> filesToAdd, List<(string, string)> files, string modPath, List<string> extensions )
        {
            if( filesToAdd == null ) return;

            foreach( var (gamePath, localFile) in filesToAdd )
            {
                if( !extensions.Any( gamePath.EndsWith ) ) continue;
                files.Add( (gamePath, Path.Join( modPath, localFile )) );
            }
        }
    }
}
