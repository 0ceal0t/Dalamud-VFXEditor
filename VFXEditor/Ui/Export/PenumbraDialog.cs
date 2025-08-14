using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using VfxEditor.FileBrowser;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Ui.Export.Categories;
using VfxEditor.Ui.Export.Penumbra;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Export {
    // ==== FOR READING PENUMBRA JSONS =====

    [Serializable]
    public class PenumbraItemStruct {
        public string Name = "";
        public string Description = "";
        public int Priority = 0;
    }

    [Serializable]
    public class PenumbraModStruct : PenumbraItemStruct {
        public Dictionary<string, string> Files = [];
        public Dictionary<string, string> FileSwaps = [];
        public List<object> Manipulations = [];
    }

    [Serializable]
    public class PenumbraGroupStruct : PenumbraItemStruct {
        public string Type = "Single"; // Single / Multi
        public uint DefaultSettings = 0; // Bitmask of 32 defaults
        public List<PenumbraOptionStruct> Options = [];
    }

    [Serializable]
    public class PenumbraOptionStruct : PenumbraItemStruct {
        public Dictionary<string, string> Files = [];
        public Dictionary<string, string> FileSwaps = [];
        public List<object> Manipulations = [];
    }

    [Serializable]
    public class PenumbraMeta {
        public int FileVersion = 3;
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Version = "";
        public string Website = "";
        public List<string> ModTags = [];
    }

    // ======= FOR WORKSPACE =======

    [Serializable]
    public class PenumbraWorkspace {
        // Exactly the same, except the `Files` dictionaries just store the indexes of which documents to use
        // Like "vfx": "Array[int]{0,1,2}"
        public PenumbraMeta Meta = new();
        public PenumbraModStruct DefaultMod = new();
        public List<PenumbraGroupStruct> Groups = [];
    }

    public class PenumbraDialog : ExportDialog {
        private readonly ExportDialogCategorySet DefaultMod = new();
        private readonly List<PenumbraGroup> Groups = [];
        private PenumbraGroup Selected;

        public PenumbraDialog() : base( "Penumbra" ) { }

        protected override void OnExport() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".pmp,.*", ModName, "pmp", ( bool ok, string res ) => {
                if( !ok ) return;
                Export( res );
                Hide();
            } );
        }

        protected override void OnDraw() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            if( ImGui.BeginCombo( "##Group", Selected == null ? "Default Mod" : Selected.GetName() ) ) {
                using( var color = ImRaii.PushColor( ImGuiCol.Text, ImGui.GetColorU32( ImGuiCol.TextDisabled ) ) ) {
                    if( ImGui.Selectable( "Default Mod" ) ) Selected = null;
                }

                foreach( var (group, idx) in Groups.WithIndex() ) {
                    if( ImGui.Selectable( $"{group.GetName()}###{idx}" ) ) Selected = group;
                }
                ImGui.EndCombo();
            }

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
                if( ImGui.Button( $"{FontAwesomeIcon.Plus.ToIconString()}" ) ) {
                    var newGroup = new PenumbraGroup();
                    Selected = newGroup;
                    Groups.Add( newGroup );
                }

                if( Selected != null ) {
                    ImGui.SameLine();
                    if( UiUtils.RemoveButton( $"{FontAwesomeIcon.Trash.ToIconString()}" ) ) {
                        Selected.Reset();
                        Groups.Remove( Selected );
                        Selected = null;
                    }
                }
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( Selected == null ) DefaultMod.Draw();
            else Selected.Draw();
        }

        protected override void OnRemoveDocument( IFileDocument document ) {
            Groups.ForEach( x => x.RemoveDocument( document ) );
            DefaultMod.RemoveDocument( document );
        }

        protected override void OnReset() {
            ModName = "";
            Author = "";
            Version = "1.0.0";
            Groups.ForEach( x => x.Reset() );
            Groups.Clear();
            Selected = null;
            DefaultMod.Reset();
        }

        private PenumbraMeta GetMeta() => new() {
            Name = ModName,
            Author = Author,
            Description = "Exported from VFXEditor",
            Version = Version
        };

        private void Export( string saveFile ) {
            try {
                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_PENUMBRA_TEMP" );
                Directory.CreateDirectory( tempDir );

                // Meta
                File.WriteAllText( Path.Combine( tempDir, "meta.json" ), JsonConvert.SerializeObject( GetMeta() ) );

                // Default Mod
                var mod = new PenumbraModStruct {
                    Files = DefaultMod.Export( tempDir, "" )
                };
                File.WriteAllText( Path.Combine( tempDir, "default_mod.json" ), JsonConvert.SerializeObject( mod ) );

                // Groups
                foreach( var (group, idx) in Groups.WithIndex() ) {
                    var groupData = group.Export( tempDir );
                    File.WriteAllText( Path.Combine( tempDir, $"group_{idx + 1:D3}_{group.GetName().ToLower()}.json" ), JsonConvert.SerializeObject( groupData ) );
                }

                if( File.Exists( saveFile ) ) File.Delete( saveFile );
                ZipFile.CreateFromDirectory( tempDir, saveFile );
                Directory.Delete( tempDir, true );
                Dalamud.Log( $"Exported To: {saveFile}" );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Could not export to Penumbra" );
            }
        }

        // =====================

        public void WorkspaceExport( Dictionary<string, string> meta ) {
            var data = new PenumbraWorkspace {
                Meta = GetMeta(),
                DefaultMod = new() {
                    Files = DefaultMod.WorkspaceExport()
                },
                Groups = Groups.Select( x => x.WorkspaceExport() ).ToList()
            };
            meta["penumbra"] = JsonConvert.SerializeObject( data );
        }

        public void WorkspaceImport( JObject meta, Dictionary<IFileManager, int> offsets ) {
            if( !meta.ContainsKey( "penumbra" ) ) return;
            var data = JsonConvert.DeserializeObject<PenumbraWorkspace>( meta["penumbra"].ToString() );

            ModName = data.Meta.Name;
            Author = data.Meta.Author;
            Version = data.Meta.Version;

            DefaultMod.WorkspaceImport( data.DefaultMod.Files, offsets );
            Groups.AddRange( data.Groups.Select( x => new PenumbraGroup( x, offsets ) ) );
        }
    }
}
