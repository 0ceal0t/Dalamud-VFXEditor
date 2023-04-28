using Dalamud.Logging;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Penumbra;

namespace VfxEditor.Select {
    public class SelectPenumbraTab : SelectTab<string, Dictionary<string, List<(string, string)>>> {
        public SelectPenumbraTab( SelectDialog dialog ) : base( dialog, "Penumbra", "Penumbra-Shared" ) { }

        public override void Draw( string parentId ) {
            if( Plugin.PenumbraIpc.PenumbraEnabled ) {
                base.Draw( parentId );
                return;
            }

            var id = $"{parentId}/{Name}";

            ImGui.BeginDisabled();
            var closed = !ImGui.BeginTabItem( $"{Name}{id}" );
            ImGui.EndDisabled();
            if( closed ) return;

            ImGui.TextDisabled( "Penumbra is not currently running..." );
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

            var modPath = Path.Join( baseModPath, Selected );
            var files = Directory.GetFiles( modPath ).Where( x => x.EndsWith( ".json" ) && !x.EndsWith( "meta.json" ) );
            foreach( var file in files ) {
                try {
                    var loadedFiles = new List<(string, string)>();
                    loaded[Path.GetFileName( file ).Replace( ".json", "" )] = loadedFiles;

                    var jsonFile = JsonConvert.DeserializeObject<PenumbraMod>( File.ReadAllText( file ) );
                    foreach( var modFile in jsonFile.Files ) {
                        var (gamePath, localFile) = modFile;
                        if( !gamePath.EndsWith( Dialog.Extension ) ) continue;
                        loadedFiles.Add( (gamePath, Path.Join( modPath, localFile )) );
                    }
                }
                catch( Exception ex ) {
                    PluginLog.Error( file, ex );
                }
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
