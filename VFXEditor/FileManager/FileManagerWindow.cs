using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui;
using VfxEditor.TexTools;
using VfxEditor.Utils;
using Newtonsoft.Json.Linq;
using VfxEditor.Data;
using VfxEditor.Select;

namespace VfxEditor.FileManager {
    public abstract class FileManagerWindow : GenericDialog {
        protected FileManagerWindow( string name, bool menuBar, int startingWidth, int startingHeight ) : 
            base( name, menuBar, startingWidth, startingHeight ) {
        }

        public abstract ManagerConfiguration GetConfig();
        public abstract CopyManager GetCopyManager();
        public abstract CommandManager GetCommandManager();

        public abstract void SetSource( SelectResult result );
        public abstract void ShowSource();

        public abstract void SetReplace( SelectResult result );
        public abstract void ShowReplace();
    }

    public abstract class FileManagerWindow<T, R, S> : FileManagerWindow, IFileManager where T : FileManagerDocument<R, S> where R : FileManagerFile {
        public T ActiveDocument { get; protected set; } = null;
        public R CurrentFile => ActiveDocument?.CurrentFile;

        private int DOC_ID = 0;
        public readonly string Id;
        protected readonly string WindowTitle;
        private readonly string Extension;
        private readonly string TempFilePrefix;
        private readonly string WorkspaceKey;
        private readonly string WorkspacePath;

        public readonly ManagerConfiguration Configuration;
        public readonly CopyManager Copy = new();

        protected string LocalPath => Path.Combine( Plugin.Configuration.WriteLocation, $"{TempFilePrefix}{DOC_ID++}.{Extension}" ).Replace( '\\', '/' ); // temporary write location

        public readonly List<T> Documents = new();
        private readonly FileManagerDocumentWindow<T, R, S> DocumentWindow;

        public SelectDialog SourceSelect { get; protected set; }
        public SelectDialog ReplaceSelect { get; protected set; }

        public FileManagerWindow( string windowTitle, string id, string extension, string workspaceKey, string workspacePath ) : base( windowTitle, true, 800, 1000 ) {
            WindowTitle = windowTitle;
            Id = id;
            TempFilePrefix = $"{id}Temp";
            Extension = extension;
            WorkspaceKey = workspaceKey;
            WorkspacePath = workspacePath;

            if( !Plugin.Configuration.ManagerConfigs.TryGetValue( Id, out Configuration ) ) {
                Configuration = new ManagerConfiguration();
                Plugin.Configuration.ManagerConfigs.Add( Id, Configuration );
            }

            AddDocument();
            DocumentWindow = new( windowTitle, this );
        }

        public override CopyManager GetCopyManager() => Copy;
        public override CommandManager GetCommandManager() => CurrentFile?.Command;
        public override ManagerConfiguration GetConfig() => Configuration;
        public string GetExportName() => Id;

        public override void SetSource( SelectResult result ) {
            ActiveDocument?.SetSource( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void ShowSource() => SourceSelect?.Show();

        public override void SetReplace( SelectResult result ) {
            ActiveDocument?.SetReplace( result );
            Plugin.Configuration.AddRecent( Configuration.RecentItems, result );
        }

        public override void ShowReplace() => ReplaceSelect?.Show();

        protected abstract T GetNewDocument();

        public void AddDocument() {
            var newDocument = GetNewDocument();
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
        }

        public void SelectDocument( T document ) {
            ActiveDocument = document;
        }

        public bool RemoveDocument( T document ) {
            var switchDoc = ( document == ActiveDocument );
            Documents.Remove( document );

            if( switchDoc ) {
                ActiveDocument = Documents[0];
                document.Dispose();
                return true;
            }

            document.Dispose();
            return false;
        }

        protected virtual void DrawMenu() {
            if( CurrentFile == null ) return;

            if( ImGui.BeginMenu( "Edit##Menu" ) ) {
                GetCopyManager().Draw();
                GetCommandManager().Draw();
                ImGui.EndMenu();
            }
        }

        public override void DrawBody() {
            SourceSelect?.Draw();
            ReplaceSelect?.Draw();

            Name = WindowTitle + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceLocation ) ? "" : " - " + Plugin.CurrentWorkspaceLocation ) + "###" + WindowTitle;
            CheckKeybinds();
            DocumentWindow.Draw();

            if( ImGui.BeginMenuBar() ) {
                Plugin.DrawFileMenu();
                DrawMenu();
                if( ImGui.MenuItem( $"Documents##{WindowTitle}/Menu" ) ) DocumentWindow.Show();
                ImGui.Separator();
                Plugin.DrawManagersMenu();

                ImGui.EndMenuBar();
            }

            ActiveDocument?.Draw();
        }

        protected override void PreDraw() {
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PushStyleColor( ImGuiCol.TitleBg, Configuration.TitleBg );
            ImGui.PushStyleColor( ImGuiCol.TitleBgActive, Configuration.TitleBgActive );
            ImGui.PushStyleColor( ImGuiCol.TitleBgCollapsed, Configuration.TitleBgCollapsed );
        }

        protected override void PostDraw() {
            if( !Configuration.UseCustomWindowColor ) return;
            ImGui.PopStyleColor( 3 );
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.DocumentsKeybind.KeyPressed() ) DocumentWindow.Show();
            if( Plugin.Configuration.UpdateKeybind.KeyPressed() ) ActiveDocument?.Update();
            ActiveDocument?.CheckKeybinds();
        }

        public void WorkspaceImport( JObject meta, string loadLocation ) {
            var items = WorkspaceUtils.ReadFromMeta<S>( meta, WorkspaceKey );
            if( items == null ) return;
            foreach( var item in items ) {
                var newDocument = GetWorkspaceDocument( item, Path.Combine( loadLocation, WorkspacePath ) );
                ActiveDocument = newDocument;
                Documents.Add( newDocument );
            }
            if( Documents.Count == 0 ) AddDocument();
        }

        protected abstract T GetWorkspaceDocument( S data, string localPath );

        public void WorkspaceExport( Dictionary<string, string> meta, string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, WorkspacePath );
            Directory.CreateDirectory( rootPath );

            var documentIdx = 0;
            List<S> documentMeta = new();

            foreach( var document in Documents ) {
                document.WorkspaceExport( documentMeta, rootPath, $"{TempFilePrefix}{documentIdx++}.{Extension}" );
            }

            WorkspaceUtils.WriteToMeta( meta, documentMeta.ToArray(), WorkspaceKey );
        }

        public virtual void PenumbraExport( string modFolder ) {
            foreach( var document in Documents ) {
                document.PenumbraExport( modFolder );
            }
        }

        public virtual void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            foreach( var document in Documents ) {
                document.TextoolsExport( writer, simpleParts, ref modOffset );
            }
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            foreach( var document in Documents ) {
                if( document.GetReplacePath( path, out var documentReplacePath ) ) {
                    replacePath = documentReplacePath;
                    return true;
                }
            }
            return false;
        }

        public bool DoDebug( string path ) => path.Contains( $".{Extension}" );

        public void ToDefault() {
            Dispose();
            AddDocument(); // Default Document
        }

        public virtual void Dispose() {
            foreach( var document in Documents ) document.Dispose();
            Documents.Clear();
            ActiveDocument = null;
            DOC_ID = 0;

            SourceSelect?.Hide();
            ReplaceSelect?.Hide();
        }
    }
}
