using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.Helper;
using VFXEditor.Textools;
using VFXSelect;

namespace VFXEditor.FileManager {
    public abstract class FileManagerDocument<T, S> where T : class {
        protected SelectResult Source = SelectResult.None();
        public string SourceDisplay => Source.DisplayString;
        public string SourcePath => Source.Path;

        protected SelectResult Replace = SelectResult.None();
        public string ReplaceDisplay => Replace.DisplayString;
        public string ReplacePath => Replace.Path;

        protected T CurrentFile;

        protected VerifiedStatus Verified = VerifiedStatus.UNKNOWN;
        protected string WriteLocation;
        public string WritePath => WriteLocation;

        private readonly string Id; // Tmb
        private readonly string FileType; // TMB

        public FileManagerDocument( string writeLocation, string id, string fileType ) {
            Id = id;
            FileType = fileType;
            WriteLocation = writeLocation;
        }

        public FileManagerDocument( string writeLocation, string localPath, SelectResult source, SelectResult replace, string id, string fileType ) {
            Id = id;
            FileType = fileType;
            WriteLocation = writeLocation;
            Source = source;
            Replace = replace;
            LoadLocal( localPath );
        }

        protected abstract bool GetVerified();

        public void SetSource( SelectResult result ) {
            switch( result.Type ) {
                case SelectResultType.Local: // LOCAL
                    LoadLocal( result.Path );
                    break;
                default: // EVERYTHING ELSE: GAME FILES
                    LoadGame( result.Path );
                    break;
            }
            Source = result;
            if( CurrentFile != null ) {
                Verified = GetVerified() ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                Update();
            }
        }

        protected void RemoveSource() {
            CurrentFile = null;
            Source = SelectResult.None();
        }

        public void SetReplace( SelectResult result ) {
            Replace = result;
        }

        protected void RemoveReplace() {
            Replace = SelectResult.None();
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = Replace.Path.Equals( path ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        protected abstract void LoadLocal( string localPath );

        protected abstract void LoadGame( string localPath );

        protected abstract void Update();

        protected void Reload( List<string> papIds = null ) {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( Replace.Path, WriteLocation, papIds );
        }

        public virtual void Dispose() {
            CurrentFile = null;
            File.Delete( WriteLocation );
        }

        public abstract void PenumbraExport( string modFolder );

        public abstract void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset );

        public abstract void WorkspaceExport( List<S> meta, string rootPath, string newPath );

        public virtual void Draw() {
            ImGui.Columns( 2, $"{Id}-Columns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.Text( $"Loaded {FileType}" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $"{FileType} Being Replaced" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            var sourceString = Source.DisplayString;
            var previewString = Replace.DisplayString;
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 140 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            ImGui.InputText( $"##{Id}-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) ) ) {
                SourceShow();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UIHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##{Id}-SourceRemove", new Vector2( 30, 23 ) ) ) {
                RemoveSource();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.InputText( $"##{Id}-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##{Id}-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                ReplaceShow();
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushStyleColor( ImGuiCol.Button, UIHelper.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##{Id}-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                RemoveReplace();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();

            ImGui.PopItemWidth();
            ImGui.Columns( 1 );

            // ================================

            DrawBody();
        }

        protected abstract void DrawBody();

        protected abstract void SourceShow();

        protected abstract void ReplaceShow();

        protected static void DisplayBeginHelpText() {
            ImGui.Text( "To begin, select a file using the magnifying glass icon: " );
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.Button( $"{( char )FontAwesomeIcon.Search}", new Vector2( 30, 23 ) );
            ImGui.PopFont();
        }
    }
}
