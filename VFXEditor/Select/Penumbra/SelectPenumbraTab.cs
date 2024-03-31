using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Ui.Export;

namespace VfxEditor.Select.Penumbra {
    public class SelectedPenumbraMod {
        public Dictionary<string, List<string>> SourceFiles;
        public Dictionary<string, List<string>> ReplaceFiles;
        public PenumbraMeta Meta;
    }

    public class SelectPenumbraTab : SelectTab<SelectPenumbraTabItem, SelectedPenumbraMod> {
        public SelectPenumbraTab( SelectDialog dialog ) : base( dialog, "Penumbra", "Penumbra-Shared" ) { }

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
            Items.AddRange( Plugin.PenumbraIpc.GetMods().Select( x => new SelectPenumbraTabItem( x ) ) );
        }

        // $"{group} {option}" -> (gamePath, localPath)
        public override void LoadSelection( SelectPenumbraTabItem item, out SelectedPenumbraMod loaded ) {
            loaded = new();
            var files = new Dictionary<string, List<(string, string)>>();

            var baseModPath = Plugin.PenumbraIpc.GetModDirectory();
            if( string.IsNullOrEmpty( baseModPath ) ) return;

            try {
                var modPath = Path.Join( baseModPath, Selected.Name );
                loaded.Meta = JsonConvert.DeserializeObject<PenumbraMeta>( File.ReadAllText( Path.Join( modPath, "meta.json" ) ) );

                var modFiles = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
                foreach( var modFile in modFiles ) {
                    try {
                        var modFileName = Path.GetFileName( modFile ).Replace( ".json", "" );
                        if( modFileName == "default_mod" ) {
                            var mod = JsonConvert.DeserializeObject<PenumbraModStruct>( File.ReadAllText( modFile ) );
                            if( mod.Files != null ) {
                                var defaultFiles = new List<(string, string)>();
                                AddToFiles( mod?.Files, defaultFiles, modPath );
                                files["default_mod"] = defaultFiles;
                            }
                        }
                        else {
                            var group = JsonConvert.DeserializeObject<PenumbraGroupStruct>( File.ReadAllText( modFile ) );
                            if( group.Options != null ) {
                                foreach( var option in group.Options.Where( x => x.Files != null ) ) {
                                    var optionFiles = new List<(string, string)>();
                                    AddToFiles( option?.Files, optionFiles, modPath );
                                    files[$"{group.Name} / {option.Name}"] = optionFiles;
                                }
                            }
                        }
                    }
                    catch( Exception e ) {
                        Dalamud.Error( e, modFile );
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

        private void AddToFiles( Dictionary<string, string> filesToAdd, List<(string, string)> files, string modPath ) {
            if( filesToAdd == null ) return;

            foreach( var (gamePath, localFile) in filesToAdd ) {
                if( !Dialog.Extensions.Any( gamePath.EndsWith ) ) continue;
                files.Add( (gamePath, Path.Join( modPath, localFile )) );
            }
        }

        protected override void DrawSelected() {
            if( Loaded.Meta != null ) {
                ImGui.TextDisabled( $"by {Loaded.Meta.Author}" );
            }

            var files = Dialog.ShowLocal ? Loaded.SourceFiles : Loaded.ReplaceFiles;
            if( files != null ) {
                Dialog.DrawPaths( files, Selected.Name, SelectResultType.Local );
            }
        }
    }
}
