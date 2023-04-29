using Dalamud.Logging;
using ImGuiNET;
using ImGuizmoNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VfxEditor.Penumbra;

namespace VfxEditor.Select {
    public class SelectPenumbraTab : SelectTab<string, Dictionary<string, List<(string, string)>>> {
        public SelectPenumbraTab( SelectDialog dialog ) : base( dialog, "Penumbra", "Penumbra-Shared" ) { }

        public override void Draw( string parentId ) {
            if( !Plugin.PenumbraIpc.PenumbraEnabled ) return;
            base.Draw( parentId );
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
                        var mod = JsonConvert.DeserializeObject<PenumbraMod>( File.ReadAllText( file ) );

                        if( fileName == "default_mod" && mod.Files != null ) { // Default mod
                            var defaultFiles = new List<(string, string)>();
                            AddToFiles( mod, defaultFiles, modPath );
                            loaded[fileName] = defaultFiles;
                        }
                        else if( mod.Options != null ) { // Option group
                            var groupName = mod.Name;

                            foreach( var option in mod.Options.Where( x => x.Files != null ) ) {
                                var optionFiles = new List<(string, string)>();
                                AddToFiles( option, optionFiles, modPath );
                                loaded[$"{groupName} / {option.Name}"] = optionFiles;
                            }
                        }
                    }
                    catch( Exception ex ) {
                        PluginLog.Error( file, ex );
                    }
                }
            }
            catch( Exception e ) {
                PluginLog.Error( "Error reading Penumbra mods", e );
            }
        }

        private void AddToFiles( PenumbraMod mod, List<(string, string)> files, string modPath ) {
            if( mod == null || mod.Files == null ) return;
            foreach( var modFile in mod.Files ) {
                var (gamePath, localFile) = modFile;
                if( !gamePath.EndsWith( Dialog.Extension ) ) continue;
                files.Add( (gamePath, Path.Join( modPath, localFile )) );
            }
        }

        protected override void DrawSelected( string parentId ) {
            var groupIdx = 0;
            foreach( var group in Loaded ) {
                if( ImGui.CollapsingHeader( $"{group.Key}{parentId}{groupIdx}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    ImGui.Indent();

                    var fileIdx = 0;
                    foreach( var file in group.Value ) {
                        var (gamePath, localPath) = file;
                        Dialog.DrawPath( $"File {fileIdx}", localPath, gamePath, $"{parentId}{groupIdx}", SelectResultType.Local, $"{Selected} {group.Key} {fileIdx}", false );
                        fileIdx++;
                    }

                    ImGui.Unindent();
                }

                groupIdx++;
            }
        }

        protected override string GetName( string item ) => item;
    }
}
