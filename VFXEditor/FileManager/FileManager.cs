using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using VFXEditor.Helper;
using VFXEditor.Textools;
using VFXEditor.Dialogs;
using VFXSelect;
namespace VFXEditor.FileManager {
    public abstract class FileManager<T, S, R> : GenericDialog where T : FileManagerDocument<R, S> where R : class { // S = workspace document
        private int DocumentId = 0;
        private readonly string Extension; // tmb
        private readonly string TempFilePrefix; // TmbTemp
        private readonly string PenumbraPath; // Tmb
        protected string LocalPath => Path.Combine( Plugin.Configuration.WriteLocation, $"{TempFilePrefix}{DocumentId++}.{Extension}" ); // temporary write location

        protected readonly string Title;
        protected readonly string Id;
        protected List<T> Documents = new();
        protected T ActiveDocument = null;
        private bool HasDefault = true;

        private T SelectedDocument = null; // for document selection dialog
        protected bool DocumentDialogVisible = false;
        private readonly bool OnlyDocumentDialog = false;

        public FileManager(string title, string id, string tempFilePrefix, string extension, string penumbaPath, bool onlyDocumentDialog = false) : base(title, menuBar:true) {
            Title = title;
            Id = id;
            TempFilePrefix = tempFilePrefix;
            Extension = extension;
            PenumbraPath = penumbaPath;
            OnlyDocumentDialog = onlyDocumentDialog;

            Size = new Vector2( 800, 1000 );
            AddDocument();
        }

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

        protected abstract T GetNewDocument();

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

        public void ImportWorkspaceFile( string localPath, S data ) {
            var newDocument = GetImportedDocument( localPath, data );
            ActiveDocument = newDocument;
            Documents.Add( newDocument );
            if( HasDefault && Documents.Count > 1 ) {
                HasDefault = false;
                RemoveDocument( Documents[0] );
            }
        }

        protected abstract T GetImportedDocument( string localPath, S data );

        public virtual void Dispose() {
            foreach( var document in Documents ) {
                document.Dispose();
            }
            Documents = null;
            ActiveDocument = null;
        }

        public override void DrawBody() {
            DrawDocumentSelect();
            if( OnlyDocumentDialog ) return;

            DialogName = Title + ( string.IsNullOrEmpty( Plugin.CurrentWorkspaceLocation ) ? "" : " - " + Plugin.CurrentWorkspaceLocation ) + "###" + Title;

            if( ImGui.BeginMenuBar() ) {
                Plugin.DrawMenu();
                if( ImGui.MenuItem( $"Documents##{Id}/Menu" ) ) DocumentDialogVisible = true;
                ImGui.EndMenuBar();
            }

            ActiveDocument?.Draw();
        }

        protected void DrawDocumentSelect() {
            if( !(OnlyDocumentDialog ? Visible : DocumentDialogVisible) ) return;
            ImGui.SetNextWindowSize( new( 600, 400 ), ImGuiCond.FirstUseEver );

            if( ImGui.Begin( $"{DialogName} Select", ref (OnlyDocumentDialog ? ref Visible : ref DocumentDialogVisible) ) ) {
                var id = $"##{Id}/Doc";
                var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

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

                foreach( var doc in Documents ) {
                    ImGui.Text( doc.ReplaceDisplay );
                }

                ImGui.Columns( 1 );
                ImGui.EndChild();

                if( ImGui.Button( "+ NEW" + id ) ) AddDocument();

                if( SelectedDocument != null ) {
                    var deleteDisabled = ( Documents.Count == 1 );

                    ImGui.SameLine( ImGui.GetWindowWidth() - 105 );
                    if( ImGui.Button( "Select" + id ) ) {
                        SelectDocument( SelectedDocument );
                    }
                    if( !deleteDisabled ) {
                        ImGui.SameLine( ImGui.GetWindowWidth() - 55 );
                        if( UiHelper.RemoveButton( "Delete" + id ) ) {
                            RemoveDocument( SelectedDocument );
                            SelectedDocument = ActiveDocument;
                        }
                    }
                }
            }
            ImGui.End();
        }

        public virtual void PenumbraExport( string modFolder, bool exportTmb ) {
            if( !exportTmb ) return;
            foreach( var document in Documents ) {
                document.PenumbraExport( modFolder );
            }
        }

        public virtual void TextoolsExport( BinaryWriter writer, bool exportTmb, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( !exportTmb ) return;
            foreach( var document in Documents ) {
                document.TextoolsExport( writer, simpleParts, ref modOffset );
            }
        }

        public virtual S[] WorkspaceExport( string saveLocation ) {
            var rootPath = Path.Combine( saveLocation, PenumbraPath);
            Directory.CreateDirectory( rootPath );

            var tmbId = 0;
            List<S> tmbMeta = new();

            foreach( var document in Documents ) {
                var newPath = $"{TempFilePrefix}{tmbId++}.{Extension}";
                document.WorkspaceExport( tmbMeta, rootPath, newPath );
            }
            return tmbMeta.ToArray();
        }
    }
}
