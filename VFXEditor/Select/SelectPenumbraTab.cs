using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Ui.Export;

namespace VfxEditor.Select {
    public class SelectPenumbraTab : SelectTab<string, Dictionary<string, List<(string, string)>>> {
        public SelectPenumbraTab( SelectDialog dialog ) : base( dialog, "Penumbra", "Penumbra-Shared", SelectResultType.Local ) { }

        public override void Draw() {
            if( !Plugin.PenumbraIpc.PenumbraEnabled ) return;
            base.Draw();
        }

        // Don't need to worry about doing this async
        // Also fine to keep getting the mods every frame, since it could change
        public override void Load() {
            LoadData();
            State.WaitingForItems = false;
            State.ItemsLoaded = true;
        }

        public override void LoadData() {
            Items.Clear();
            Items.AddRange( Plugin.PenumbraIpc.GetMods() );
        }

        // $"{group} {option}" -> (gamePath, localPath)
        public override void LoadSelection( string item, out Dictionary<string, List<(string, string)>> loaded ) {
            loaded = new();
            var baseModPath = Plugin.PenumbraIpc.GetModDirectory();
            if( string.IsNullOrEmpty( baseModPath ) ) return;

            try {
                var modPath = Path.Join( baseModPath, Selected );
                var files = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
                foreach( var file in files ) {
                    try {
                        var fileName = Path.GetFileName( file ).Replace( ".json", "" );
                        if( fileName == "default_mod" ) {
                            var mod = JsonConvert.DeserializeObject<PenumbraModStruct>( File.ReadAllText( file ) );
                            if( mod.Files != null ) {
                                var defaultFiles = new List<(string, string)>();
                                AddToFiles( mod?.Files, defaultFiles, modPath );
                                loaded["default_mod"] = defaultFiles;
                            }
                        }
                        else {
                            var group = JsonConvert.DeserializeObject<PenumbraGroupStruct>( File.ReadAllText( file ) );
                            if( group.Options != null ) {
                                foreach( var option in group.Options.Where( x => x.Files != null ) ) {
                                    var optionFiles = new List<(string, string)>();
                                    AddToFiles( option?.Files, optionFiles, modPath );
                                    loaded[$"{group.Name} / {option.Name}"] = optionFiles;
                                }
                            }
                        }
                    }
                    catch( Exception e ) {
                        Dalamud.Error( e, file );
                    }
                }
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error reading Penumbra mods" );
            }
        }

        private void AddToFiles( Dictionary<string, string> filesToAdd, List<(string, string)> files, string modPath ) {
            if( filesToAdd == null ) return;

            foreach( var (gamePath, localFile) in filesToAdd ) {
                if( !gamePath.EndsWith( Dialog.Extension ) ) continue;
                files.Add( (gamePath, Path.Join( modPath, localFile )) );
            }
        }

        protected override void DrawSelected() {
            var filtered = Loaded.ToDictionary(
                x => (x.Key, 0u),
                x => x.Value.Where( y => Path.Exists( y.Item2 ) ).Select( y => $"{y.Item1}|{y.Item2}" ).ToList()
            ); // Filter out local paths that don't exist
            DrawPaths( filtered, Selected );
        }

        protected override string GetName( string item ) => item;
    }
}
