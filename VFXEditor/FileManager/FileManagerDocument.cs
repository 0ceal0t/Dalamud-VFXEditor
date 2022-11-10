using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Utils;
using VfxEditor.TexTools;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<T, S> where T : class {
        public T CurrentFile { get; protected set; }

        protected SelectResult Source = SelectResult.None();
        public string SourceDisplay => Source.DisplayString;
        public string SourcePath => Source.Path;

        protected SelectResult Replace = SelectResult.None();
        public string ReplaceDisplay => Replace.DisplayString;
        public string ReplacePath => Replace.Path;

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

        protected abstract bool IsVerified();

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
                Verified = IsVerified() ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                UpdateFile();
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

        protected abstract void UpdateFile();

        protected abstract void ExportRaw();

        protected void Reload( List<string> papIds = null ) {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( Replace.Path, WriteLocation, papIds );
        }

        public virtual void Dispose() {
            CurrentFile = null;
            File.Delete( WriteLocation );
        }

        public abstract void Update();

        public abstract void PenumbraExport( string modFolder );

        public abstract void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset );

        public abstract void WorkspaceExport( List<S> meta, string rootPath, string newPath );

        public abstract void CheckKeybinds();

        public virtual void Draw() {
            if ( Plugin.Configuration.WriteLocationError ) {
                ImGui.TextWrapped( $"The plugin does not have access to your designated temp file location ({Plugin.Configuration.WriteLocation}). Please go to File > Settings and change it, then restart your game (for example, C:\\Users\\[YOUR USERNAME HERE]\\Documents\\VFXEdit)." );
                return;
            }

            ImGui.Columns( 2, $"{Id}-Columns", false );

            // ======== INPUT TEXT =========
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Text( $"Loaded {FileType}" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $"{FileType} Being Replaced" );
            ImGui.NextColumn();

            // ======= SEARCH BARS =========
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 140 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );

            DisplaySearchBars();

            ImGui.PopItemWidth();
            ImGui.Columns( 1 );

            DrawBody();
        }

        protected void DisplaySearchBars() {
            var sourceString = SourceDisplay;
            var previewString = ReplaceDisplay;

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.PushStyleColor( ImGuiCol.Button, UiUtils.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##{Id}-SourceRemove", new Vector2( 30, 23 ) ) ) {
                RemoveSource();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( $"##{Id}-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##{Id}-SourceSelect", new Vector2( 30, 23 ) ) ) {
                SourceShow();
            }
            ImGui.PopFont();

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.PushStyleColor( ImGuiCol.Button, UiUtils.RED_COLOR );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Times}##{Id}-PreviewRemove", new Vector2( 30, 23 ) ) ) {
                RemoveReplace();
            }
            ImGui.PopStyleColor();
            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( $"##{Id}-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##{Id}-PreviewSelect", new Vector2( 30, 23 ) ) ) {
                ReplaceShow();
            }
            ImGui.PopFont();
        }

        protected void DisplayFileControls() {
            if( UiUtils.OkButton( "UPDATE" ) ) Update();

            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.FileDownload}" ) ) ExportRaw();
            ImGui.PopFont();
            UiUtils.Tooltip( "Export as a raw file.\nTo export as a Textools/Penumbra mod, use the \"mod export\" menu item" );

            ImGui.SameLine();
            UiUtils.ShowVerifiedStatus( Verified );
        }

        protected abstract void DrawBody();

        protected abstract void SourceShow();

        protected abstract void ReplaceShow();

        protected static void DisplayBeginHelpText() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 15 );

            var availWidth = ImGui.GetContentRegionMax().X;
            var width = availWidth > 500 ? 500 : availWidth; // cap out at 300
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + ( availWidth - width ) / 2 );
            ImGui.BeginChild( "##HelpText-1", new Vector2( width, -1 ) );
            ImGui.BeginChild( "##HelpText-1", new Vector2( width, -1 ) );

            UiUtils.CenteredText( "Welcome to VFXEditor" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );
            ImGui.TextWrapped( "To begin, select a file to load and one to replace using the magnifying glass icons above, then click \"Update\". For example, to edit the skill \"Fell Cleave,\" select it as both the loaded and replaced effect. For more information, please see any of the resources below." );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 10 );

            var buttonWidth = ImGui.GetContentRegionMax().X - ImGui.GetStyle().FramePadding.X * 2;

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 ) );
            if( ImGui.Button( "Github", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 ) );
            if( ImGui.Button( "Report an Issue", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 ) );
            if( ImGui.Button( "Wiki", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.33725490196f, 0.38431372549f, 0.96470588235f, 1 ) );
            if( ImGui.Button( "XIVLauncher Discord", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://discord.gg/3NMcUV5" );
            }
            ImGui.PopStyleColor();

            ImGui.EndChild();
        }

        private static readonly string Text = "DO NOT modify movement abilities (dashes, backflips). Please read a guide before attempting to modify a .tmb or .pap file";

        protected static void DisplayAnimationWarning() {
            ImGui.PushStyleColor( ImGuiCol.Border, new Vector4( 1, 0, 0, 0.3f ) );
            ImGui.PushStyleColor( ImGuiCol.ChildBg, new Vector4( 1, 0, 0, 0.1f ) );

            var textSize = ImGui.CalcTextSize( Text, ImGui.GetContentRegionMax().X - 40 );

            ImGui.BeginChild( "##AnimationWarning", new Vector2( -1,
                ImGui.GetFrameHeightWithSpacing() +
                textSize.Y +
                ImGui.GetStyle().ItemSpacing.Y * 2 +
                ImGui.GetStyle().FramePadding.Y
            ), true );

            ImGui.TextWrapped( Text );
            if( ImGui.SmallButton( "Guides##Pap" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.EndChild();
            ImGui.PopStyleColor( 2 );
        }
    }
}
