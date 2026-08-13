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
    // ==== FOR READING/WRITING PENUMBRA JSONS =====
    // Shared by mods, groups, and options across every format version
    [Serializable]
    public class PenumbraItemStruct {
        public string Name = "";
        public string Description = "";
        public int Priority = 0;
    }

    // A standalone file redirect/manipulation container, used wherever Penumbra
    // nests one as its own JSON object (mod-level DefaultData, combining-group containers)
    [Serializable]
    public class PenumbraContainerStruct {
        public Dictionary<string, string> Files = [];
        public Dictionary<string, string> FileSwaps = [];
        public List<object> Manipulations = [];
    }

    // FileVersion 3 (legacy): meta.json + separate default_mod.json/group_*.json files.
    // Still written here for backwards-compatible workspace saves; Penumbra itself
    // migrates any mod on disk in this format up to V4 as soon as it loads it.
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
        public List<string> Layout = []; // e.g. [ "Hide" ] to hide this group in Penumbra's UI
        public List<PenumbraOptionStruct> Options = [];
    }

    [Serializable]
    public class PenumbraOptionStruct : PenumbraItemStruct {
        public List<string> Layout = []; // e.g. [ "Hide" ] to hide this option in Penumbra's UI
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

    // FileVersion 4+ (current): a single meta.json with everything embedded, no
    // separate group files. This is what Penumbra writes for every mod today
    // (ModSerialization.CurrentFileVersion), so it's also what VFXEditor exports.
    [Serializable]
    public class PenumbraMetaV4 {
        public int FileVersion = 4;
        public string Identifier = "";
        public string LastWrite = "";
        public string Name = "";
        public string Author = "";
        public string Description = "";
        public string Version = "";
        public string Website = "";
        public List<string> ModTags = [];
        public PenumbraContainerStruct DefaultData = new();
        public List<PenumbraGroupStructV4> Groups = [];
    }

    [Serializable]
    public class PenumbraGroupStructV4 : PenumbraItemStruct {
        public string Id = "";
        public string Type = "Single"; // Single / Multi / Imc / Combining
        public uint DefaultSettings = 0; // Bitmask of 32 defaults
        public List<string> Layout = []; // e.g. [ "Hide" ] to hide this group in Penumbra's UI
        public List<PenumbraOptionStructV4> Options = [];

        // Only present on "Combining" groups: each combination of Options maps to one of
        // these containers, rather than files being attached directly to an option
        public List<PenumbraCombiningContainerStruct> Containers = [];
    }

    [Serializable]
    public class PenumbraOptionStructV4 : PenumbraItemStruct {
        public string Id = "";
        public List<string> Layout = []; // e.g. [ "Hide" ] to hide this option in Penumbra's UI

        // Only present on Single/Multi groups. "Imc"/"Combining" options carry no files of their own
        public Dictionary<string, string> Files = [];
        public Dictionary<string, string> FileSwaps = [];
        public List<object> Manipulations = [];
    }

    [Serializable]
    public class PenumbraCombiningContainerStruct : PenumbraContainerStruct {
        public string Name = "";
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

        private string Description = "Exported from VFXEditor";
        private string Website = "";
        private string ModTags = ""; // comma-separated

        public PenumbraDialog() : base( "Penumbra" ) { }

        protected override void OnExport() {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", ".pmp,.*", ModName, "pmp", ( ok, res ) => {
                if( !ok ) return;
                Export( res );
                Hide();
            } );
        }

        protected override void OnDraw() {
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SetNextItemWidth( ImGui.GetContentRegionAvail().X * 0.5f );
                ImGui.InputTextWithHint( "##Website", "Website", ref Website, 255 );

                ImGui.SameLine();
                ImGui.SetNextItemWidth( -1 );
                ImGui.InputTextWithHint( "##ModTags", "Tags (comma-separated)", ref ModTags, 255 );

                ImGui.InputTextMultiline( "##Description", ref Description, 1024, new( -1, ImGui.GetFrameHeightWithSpacing() * 2 ) );
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

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
            Description = "Exported from VFXEditor";
            Website = "";
            ModTags = "";
            Groups.ForEach( x => x.Reset() );
            Groups.Clear();
            Selected = null;
            DefaultMod.Reset();
        }

        // Used for both the workspace save (round-tripping this dialog's state) and as the
        // base for the actual exported meta.json, so it only carries the shared, version-agnostic fields
        private PenumbraMeta GetMeta() => new() {
            Name = ModName,
            Author = Author,
            Description = Description,
            Version = Version,
            Website = Website,
            ModTags = [.. ModTags.Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries )]
        };

        // Penumbra's current format (FileVersion 4) embeds everything into a single meta.json
        private void Export( string saveFile ) {
            try {
                var saveDir = Path.GetDirectoryName( saveFile );
                var tempDir = Path.Combine( saveDir, "VFXEDITOR_PENUMBRA_TEMP" );
                Directory.CreateDirectory( tempDir );

                var meta = GetMeta();
                var modMeta = new PenumbraMetaV4 {
                    Identifier = Guid.NewGuid().ToString(),
                    LastWrite = DateTime.UtcNow.ToString( "o" ),
                    Name = meta.Name,
                    Author = meta.Author,
                    Description = meta.Description,
                    Version = meta.Version,
                    Website = meta.Website,
                    ModTags = meta.ModTags,
                    DefaultData = new() { Files = DefaultMod.Export( tempDir, "" ) },
                    Groups = [.. Groups.Select( x => x.Export( tempDir ) )]
                };

                File.WriteAllText( Path.Combine( tempDir, "meta.json" ), JsonConvert.SerializeObject( modMeta ) );

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
                Groups = [.. Groups.Select( x => x.WorkspaceExport() )]
            };
            meta["penumbra"] = JsonConvert.SerializeObject( data );
        }

        public void WorkspaceImport( JObject meta, Dictionary<IFileManagerGroup, int> offsets ) {
            if( !meta.ContainsKey( "penumbra" ) ) return;
            var data = JsonConvert.DeserializeObject<PenumbraWorkspace>( meta["penumbra"].ToString() );

            ModName = data.Meta.Name;
            Author = data.Meta.Author;
            Version = data.Meta.Version;
            Description = data.Meta.Description;
            Website = data.Meta.Website;
            ModTags = string.Join( ", ", data.Meta.ModTags ?? [] );

            DefaultMod.WorkspaceImport( data.DefaultMod.Files, offsets );
            Groups.AddRange( data.Groups.Select( x => new PenumbraGroup( x, offsets ) ) );
        }
    }
}
