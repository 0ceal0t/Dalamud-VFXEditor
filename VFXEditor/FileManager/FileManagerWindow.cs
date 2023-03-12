using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Ui;
using VfxEditor.TexTools;

namespace VfxEditor.FileManager {
    public abstract class FileManagerWindow<T, S, R> : GenericDialog where T : FileManagerDocument<R, S> where R : FileManagerFile { // S = workspace document
        public T ActiveDocument { get; protected set; } = null;
        public R CurrentFile => ActiveDocument?.CurrentFile;

        private int DocumentId = 0;
        private readonly string Extension; // tmb
        private readonly string TempFilePrefix; // TmbTemp
        private readonly string PenumbraPath; // Tmb
        private bool HasDefault = true;

        protected string LocalPath => Path.Combine( Plugin.Configuration.WriteLocation, $"{TempFilePrefix}{DocumentId++}.{Extension}" ).Replace( '\\', '/' ); // temporary write location
        protected readonly string Title;
        protected readonly string Id;
        public readonly List<T> Documents = new();

        private readonly FileManagerDocumentWindow<T, S, R> DocumentWindow;

        public FileManagerWindow( string title, string id, string tempFilePrefix, string extension, string penumbaPath ) : base( title, true, 800, 1000 ) {
            Title = title;
            Id = id;
            TempFilePrefix = tempFilePrefix;
            Extension = extension;
            PenumbraPath = penumbaPath;
            AddDocument();
            DocumentWindow = new( title, this );
        }

        protected abstract T GetImportedDocument( string localPath, S data );

        protected abstract void DrawMenu();

        protected abstract T GetNewDocument();

        public void SetSource( SelectResult result ) => ActiveDocument?.SetSource( result );

        public void SetReplace( SelectResult result ) => ActiveDocument?.SetReplace( result );

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            foreach( var document in Documents ) {
                if( document.GetReplacePath( path, out var _replacePath ) ) {
                    replacePath = _replacePath;
                    return true;
                }
            }
            return false;
        }

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

        public override void DrawBody() {
            Name = Title + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceLocation ) ? "" : " - " + Plugin.CurrentWorkspaceLocation ) + "###" + Title;
            CheckKeybinds();
            DocumentWindow.Draw();

            if( ImGui.BeginMenuBar() ) {
                Plugin.DrawFileMenu();
                DrawMenu();
                if( ImGui.MenuItem( $"Documents##{Title}/Menu" ) ) DocumentWindow.Show();
                ImGui.Separator();
                Plugin.DrawManagersMenu();

                ImGui.EndMenuBar();
            }

            ActiveDocument?.Draw();
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused( ImGuiFocusedFlags.RootAndChildWindows ) ) return;
            if( Plugin.Configuration.DocumentsKeybind.KeyPressed() ) DocumentWindow.Show();
            if( Plugin.Configuration.UpdateKeybind.KeyPressed() ) ActiveDocument?.Update();
            ActiveDocument?.CheckKeybinds();
        }

        public void ImportWorkspaceFile( string localPath, S data ) {
            var newDocument = GetImportedDocument( localPath, data );
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
            if( HasDefault && Documents.Count > 1 ) {
                HasDefault = false;
                RemoveDocument( Documents[0] );
            }
        }

        public virtual void PenumbraExport( string modFolder, bool doExport ) {
            if( !doExport ) return;
            foreach( var document in Documents ) {
                document.PenumbraExport( modFolder );
            }
        }

        public virtual void TextoolsExport( BinaryWriter writer, bool doExport, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( !doExport ) return;
            foreach( var document in Documents ) {
                document.TextoolsExport( writer, simpleParts, ref modOffset );
            }
        }

        public virtual S[] WorkspaceExport( string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, PenumbraPath );
            Directory.CreateDirectory( rootPath );

            var documentIdx = 0;
            List<S> documentMeta = new();

            foreach( var document in Documents ) {
                var newPath = $"{TempFilePrefix}{documentIdx++}.{Extension}";
                document.WorkspaceExport( documentMeta, rootPath, newPath );
            }
            return documentMeta.ToArray();
        }

        public virtual void Dispose() {
            foreach( var document in Documents ) document.Dispose();
            Documents.Clear();
            ActiveDocument = null;
        }
    }
}
