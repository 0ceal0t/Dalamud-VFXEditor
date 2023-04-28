using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Select;
using VfxEditor.TexTools;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<T, S> where T : FileManagerFile {
        public T CurrentFile { get; protected set; }

        protected SelectResult Source;
        public string SourceDisplay => Source == null ? "[NONE]" : Source.DisplayString;
        public string SourcePath => Source == null ? "" : Source.Path;

        protected SelectResult Replace;
        public string ReplaceDisplay => Replace == null ? "[NONE]" : Replace.DisplayString;
        public string ReplacePath => Replace == null ? "" : Replace.Path;

        protected VerifiedStatus Verified = VerifiedStatus.UNKNOWN;
        protected string WriteLocation;
        public string WritePath => WriteLocation;

        protected readonly string Id;
        protected readonly string IdUpperCase;
        protected readonly string Extension;
        protected readonly FileManagerWindow Manager;
        protected DateTime LastUpdate = DateTime.Now;

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
            if( result == null ) return;
            Source = result;

            if( result.Type == SelectResultType.Local ) LoadLocal( result.Path );
            else LoadGame( result.Path );

            if( CurrentFile != null ) {
                Verified = IsVerified() ? VerifiedStatus.OK : VerifiedStatus.ISSUE;
                WriteFile( WriteLocation );
            }
        }

        protected void RemoveSource() {
            CurrentFile?.Dispose();
            CurrentFile = null;
            Source = null;
        }

        public void SetReplace( SelectResult result ) { Replace = result; }

        protected void RemoveReplace() { Replace = null; }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = ReplacePath.Equals( path ) ? WriteLocation : null;
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

        protected void WriteFile( string path ) {
            if( CurrentFile == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) PluginLog.Log( "Wrote {1} file to {0}", path, IdUpperCase );
            File.WriteAllBytes( path, CurrentFile.ToBytes() );
        }

        protected void ExportRaw() => UiUtils.WriteBytesDialog( "." + Extension, CurrentFile.ToBytes(), Extension );

        protected void Reload( List<string> papIds = null ) {
            if( CurrentFile == null ) return;
            Plugin.ResourceLoader.ReloadPath( ReplacePath, WriteLocation, papIds );
        }

        public void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds <= 0.2 ) return;
            LastUpdate = DateTime.Now;

            if( Plugin.Configuration.UpdateWriteLocation ) {
                var oldWriteLocation = WriteLocation;
                var newWriteLocation = Manager.GetWriteLocation();

                WriteFile( newWriteLocation );
                WriteLocation = newWriteLocation;
                Reload( GetPapIds() );
                Plugin.ResourceLoader.ReRender();
                File.Delete( oldWriteLocation );
            }
            else {
                WriteFile( WriteLocation );
                Reload( GetPapIds() );
                Plugin.ResourceLoader.ReRender();
            }
        }

        protected virtual List<string> GetPapIds() => null;

        public void PenumbraExport( string modFolder ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;
            var data = CurrentFile.ToBytes();
            PenumbraUtils.WriteBytes( data, modFolder, path );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            var path = ReplacePath;
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

        protected virtual bool ExtraInputColumn() => false;

        protected virtual void DrawExtraColumn() { }

        protected void DisplaySearchBars() {
            var sourceString = Source == null ? "" : Source.DisplayString;
            var previewString = Replace == null ? "" : Replace.DisplayString;

            // Remove
            ImGui.PushFont( UiBuilder.IconFont );
            if( UiUtils.TransparentButton( $"{( char )FontAwesomeIcon.Times}##{Id}-SourceRemove", UiUtils.RED_COLOR ) ) RemoveSource();

            ImGui.PopFont();
            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputTextWithHint( $"##{Id}-Source", "[NONE]", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );
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
            ImGui.InputTextWithHint( $"##{Id}-Preview", "[NONE]", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );
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

        protected virtual void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                CurrentFile.Draw( $"##{Id}" );
            }
        }

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

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 30 );

            var buttonWidth = ImGui.GetContentRegionMax().X - ImGui.GetStyle().FramePadding.X * 2;

            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.41764705882f, 0.41764705882f, 0.41764705882f, 1 ) );
            if( ImGui.Button( "Wiki + Guides", new Vector2( buttonWidth, 0 ) ) ) {
                UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
            }
            ImGui.PopStyleColor();

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

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
