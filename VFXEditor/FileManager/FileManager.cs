using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.Dialogs;
using VFXEditor.Utils;
using VFXEditor.TexTools;

namespace VFXEditor.FileManager {
    public abstract class FileManager<T, S, R> : GenericDialog where T : FileManagerDocument<R, S> where R : class { // S = workspace document
        private int DocumentId = 0;
        private readonly string Extension; // tmb
        private readonly string TempFilePrefix; // TmbTemp
        private readonly string PenumbraPath; // Tmb
        protected string LocalPath => Path.Combine( VfxEditor.Configuration.WriteLocation, $"{TempFilePrefix}{DocumentId++}.{Extension}" ).Replace( '\\', '/' ); // temporary write location

        protected readonly string Title;
        protected readonly string Id;
        protected List<T> Documents = new();
        protected T ActiveDocument = null;
        public bool HasCurrentFile => ActiveDocument != null && ActiveDocument.HasCurrentFile;

        private bool HasDefault = true;

        private T SelectedDocument = null; // for document selection dialog
        protected bool DocumentDialogVisible = false;

        public FileManager( string title, string id, string tempFilePrefix, string extension, string penumbaPath ) : base( title, menuBar: true ) {
            Title = title;
            Id = id;
            TempFilePrefix = tempFilePrefix;
            Extension = extension;
            PenumbraPath = penumbaPath;

            Size = new Vector2( 800, 1000 );
            AddDocument();
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

        private void AddDocument() {
            var newDocument = GetNewDocument();
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
        }

        private void SelectDocument( T document ) {
            ActiveDocument = document;
        }

        private bool RemoveDocument( T document ) {
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
            Name = Title + ( string.IsNullOrEmpty( VfxEditor.CurrentWorkspaceLocation ) ? "" : " - " + VfxEditor.CurrentWorkspaceLocation ) + "###" + Title;
            CheckKeybinds();
            DrawDocumentSelect();

            if( ImGui.BeginMenuBar() ) {
                VfxEditor.DrawFileMenu();
                DrawMenu();
                if( ImGui.MenuItem( $"Documents##{Id}/Menu" ) ) DocumentDialogVisible = true;
                ImGui.Separator();
                VfxEditor.DrawManagersMenu();

                ImGui.EndMenuBar();
            }

            ActiveDocument?.Draw();
        }

        private void CheckKeybinds() {
            if( !ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows) ) return;
            if( VfxEditor.Configuration.DocumentsKeybind.KeyPressed() ) DocumentDialogVisible = true;
            if( VfxEditor.Configuration.UpdateKeybind.KeyPressed() ) ActiveDocument?.Update();
            ActiveDocument?.CheckKeybinds();
        }

        protected void DrawDocumentSelect() {
            if( !Visible || !DocumentDialogVisible ) return;
            ImGui.SetNextWindowSize( new( 600, 400 ), ImGuiCond.FirstUseEver );

            if( ImGui.Begin( $"{Title} Select", ref DocumentDialogVisible, ImGuiWindowFlags.NoDocking ) ) {
                var id = $"##{Id}/Doc";
                var footerHeight = ImGui.GetFrameHeightWithSpacing();

                if( ImGui.Button( "+ NEW" + id ) ) AddDocument();
                ImGui.SameLine();
                ImGui.Text( "Create documents in order to replace multiple files simultaneously" );

                ImGui.BeginChild( id + "/Child", new Vector2( 0, -footerHeight ), true );
                ImGui.Columns( 2, id + "/Columns", false );

                var idx = 0;
                foreach( var document in Documents ) {
                    if( ImGui.Selectable( document.SourceDisplay + id + idx, document == SelectedDocument, ImGuiSelectableFlags.SpanAllColumns ) ) {
                        SelectedDocument = document;
                    }
                    if( ImGui.IsItemHovered() ) {
                        ImGui.BeginTooltip();
                        ImGui.Text( "Replace path: " + document.ReplaceDisplay );
                        ImGui.Text( "Write path: " + document.WritePath );
                        ImGui.EndTooltip();
                    }
                    idx++;
                }
                ImGui.NextColumn();

                foreach( var document in Documents ) {
                    ImGui.Text( document.ReplaceDisplay );  
                }

                ImGui.Columns( 1 );
                ImGui.EndChild();

                if ( UiUtils.DisabledButton($"Open{id}", SelectedDocument != null) ) {
                    SelectDocument( SelectedDocument );
                }
                ImGui.SameLine();
                if ( UiUtils.DisabledRemoveButton($"Delete{id}", SelectedDocument != null && Documents.Count > 1 ) ) {
                    RemoveDocument( SelectedDocument );
                    SelectedDocument = ActiveDocument;
                }
            }
            ImGui.End();
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
            foreach( var document in Documents ) {
                document.Dispose();
            }
            Documents = null;
            ActiveDocument = null;
        }
    }
}
