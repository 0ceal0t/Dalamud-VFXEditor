using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Logging;
using System;
using VfxEditor.Utils;
using VfxEditor.TexTools;
using VfxEditor.Select;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<T, S> where T : FileManagerFile {
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

        protected readonly string Id; // Tmb
        protected readonly string IdUpperCase; // TMB
        protected readonly string Extension;
        protected readonly FileManagerWindow Manager;

        public FileManagerDocument( FileManagerWindow manager, string writeLocation, string id, string extension ) {
            Manager = manager;
            WriteLocation = writeLocation;
            Id = id;
            IdUpperCase = id.ToUpper();
            Extension = extension;
        }

        public FileManagerDocument( FileManagerWindow manager, string writeLocation, string localPath, SelectResult source, SelectResult replace, string id, string extension ) : 
            this( manager, writeLocation, id, extension ) {

            Source = source;
            Replace = replace;
            LoadLocal( localPath );
        }

        protected bool IsVerified() => CurrentFile.IsVerified();

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
            CurrentFile?.Dispose();
            CurrentFile = null;
            Source = SelectResult.None();
        }

        public void SetReplace( SelectResult result ) { Replace = result; }

        protected void RemoveReplace() { Replace = SelectResult.None(); }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = Replace.Path.Equals( path ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        protected abstract T FileFromReader( BinaryReader reader );

        protected void LoadLocal( string localPath ) {
            if( !File.Exists( localPath ) ) {
                PluginLog.Error( $"Local file: [{localPath}] does not exist" );
                return;
            }
            try {
                using var reader = new BinaryReader( File.Open( localPath, FileMode.Open ) );
                CurrentFile?.Dispose();
                CurrentFile = FileFromReader( reader );
                UiUtils.OkNotification( $"{IdUpperCase} file loaded" );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error Reading File", e );
                UiUtils.ErrorNotification( "Error reading file" );
            }
        }

        protected void LoadGame( string gamePath ) {
            if( !Plugin.DataManager.FileExists( gamePath ) ) {
                PluginLog.Error( $"Game file: [{gamePath}] does not exist" );
                return;
            }
            try {
                var file = Plugin.DataManager.GetFile( gamePath );
                using var ms = new MemoryStream( file.Data );
                using var reader = new BinaryReader( ms );
                CurrentFile?.Dispose();
                CurrentFile = FileFromReader( reader );
                UiUtils.OkNotification( $"{IdUpperCase} file loaded" );
            }
            catch( Exception e ) {
                PluginLog.Error( e, "Error Reading File" );
                UiUtils.ErrorNotification( "Error reading file" );
            }
        }

        protected void UpdateFile() {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote {1} file to {0}", WriteLocation, IdUpperCase );
            File.WriteAllBytes( WriteLocation, CurrentFile.ToBytes() );
        }

        protected void ExportRaw() => UiUtils.WriteBytesDialog( "." + Extension, CurrentFile.ToBytes(), Extension );

        protected void Reload( List<string> papIds = null ) {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( Replace.Path, WriteLocation, papIds );
        }

        public virtual void Update() {
            UpdateFile();
            Reload();
            Plugin.ResourceLoader.ReRender();
        }

        public void PenumbraExport( string modFolder ) {
            var path = Replace.Path;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;
            var data = CurrentFile.ToBytes();
            PenumbraUtils.WriteBytes( data, modFolder, path );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            var path = Replace.Path;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;
            var modData = TexToolsUtils.CreateType2Data( CurrentFile.ToBytes() );
            simpleParts.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public abstract S GetWorkspaceMeta( string newPath );

        public void WorkspaceExport( List<S> tmbMeta, string rootPath, string newPath ) {
            if( CurrentFile != null ) {
                var newFullPath = Path.Combine( rootPath, newPath );
                File.WriteAllBytes( newFullPath, CurrentFile.ToBytes() );
                tmbMeta.Add( GetWorkspaceMeta( newPath ) );
            }
        }

        public virtual void CheckKeybinds() {
            if( Plugin.Configuration.CopyKeybind.KeyPressed() ) Manager.GetCopyManager()?.Copy();
            if( Plugin.Configuration.PasteKeybind.KeyPressed() ) Manager.GetCopyManager()?.Paste();
            if( Plugin.Configuration.UndoKeybind.KeyPressed() ) Manager.GetCommandManager()?.Undo();
            if( Plugin.Configuration.RedoKeybind.KeyPressed() ) Manager.GetCommandManager()?.Redo();
        }

        protected abstract bool ExtraInputColumn();

        // ====== DRAWING ==========

        public void Draw() {
            if( Plugin.Configuration.WriteLocationError ) {
                ImGui.TextWrapped( $"The plugin does not have access to your designated temp file location ({Plugin.Configuration.WriteLocation}). Please go to File > Settings and change it, then restart your game (for example, C:\\Users\\[YOUR USERNAME HERE]\\Documents\\VFXEdit)." );
                return;
            }

            var threeColumns = ExtraInputColumn();

            ImGui.Columns( threeColumns ? 3 : 2, $"{Id}-Columns", false );

            DrawInputTextColumn();
            ImGui.NextColumn();

            DrawSearchBarsColumn();
            if( threeColumns ) {
                ImGui.NextColumn();
                DrawExtraColumn();
            }

            ImGui.Columns( 1 );

            DrawBody();
        }

        protected virtual void DrawInputTextColumn() {
            ImGui.SetColumnWidth( 0, 140 );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Text( $"Loaded {IdUpperCase}" );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( $"{IdUpperCase} Being Replaced" );
        }

        protected virtual void DrawSearchBarsColumn() {
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 140 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );
            DisplaySearchBars();
            ImGui.PopItemWidth();
        }

        protected virtual void DrawExtraColumn() { }

        protected void DisplaySearchBars() {
            var sourceString = SourceDisplay ?? "";
            var previewString = ReplaceDisplay ?? "";

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            if( UiUtils.TransparentButton( $"{( char )FontAwesomeIcon.Times}##{Id}-SourceRemove", UiUtils.RED_COLOR ) ) RemoveSource();

            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( $"##{Id}-Source", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##{Id}-SourceSelect" ) ) Manager.ShowSource();
            ImGui.PopFont();

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            if( UiUtils.TransparentButton( $"{( char )FontAwesomeIcon.Times}##{Id}-ReplaceRemove", UiUtils.RED_COLOR ) ) RemoveReplace();

            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputText( $"##{Id}-Preview", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
            // Search
            ImGui.SameLine();
            ImGui.PushFont( UiBuilder.IconFont );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Search}##{Id}-PreviewSelect" ) ) Manager.ShowReplace();
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

        public virtual void Dispose() {
            CurrentFile?.Dispose();
            CurrentFile = null;
            File.Delete( WriteLocation );
        }

        // ========================

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
