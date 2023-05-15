using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<T, S> where T : FileManagerFile {
        public T CurrentFile { get; protected set; }

        public string DisplayName => string.IsNullOrEmpty( Name ) ? ReplaceDisplay : Name;
        protected string Name = "";

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

        public bool Unsaved = false;
        protected DateTime LastUpdate = DateTime.Now;

        public FileManagerDocument(
            FileManagerWindow manager,
            string writeLocation,
            string id,
            string extension
         ) {
            Manager = manager;
            WriteLocation = writeLocation;
            Id = id;
            IdUpperCase = id.ToUpper();
            Extension = extension;
        }

        public FileManagerDocument(
            FileManagerWindow manager,
            string writeLocation,
            string localPath,
            string name,
            SelectResult source,
            SelectResult replace,
            string id,
            string extension
        ) : this( manager, writeLocation, id, extension ) {
            Name = name ?? "";
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
            Unsaved = false;
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
            Unsaved = false;

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

        public void PenumbraExport( string modFolder, Dictionary<string, string> files ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;
            var data = CurrentFile.ToBytes();

            PenumbraUtils.WriteBytes( data, modFolder, path, files );
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
            ImGui.Columns( threeColumns ? 3 : 2, "Columns", false );

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
            ImGui.SetColumnWidth( 0, 150 );

            var pos = ImGui.GetCursorScreenPos() + new Vector2( 5, 0 );
            var color = ImGui.GetColorU32( ImGuiCol.TextDisabled );
            var height = ImGui.GetFrameHeight();
            var spacing = ImGui.GetStyle().ItemSpacing.Y;

            var radius = 5f;
            var width = 15f;
            var segmentResolution = 10;
            var thickness = 2;

            var arrowHeight = 8;
            var arrowWidth = 8;

            var drawList = ImGui.GetWindowDrawList();
            var topLeft = pos + new Vector2( 0, height * 0.5f );
            var topRight = topLeft + new Vector2( width, 0 );
            var bottomRight = pos + new Vector2( width, height * 1.5f + spacing - 1 );
            var bottomLeft = new Vector2( topLeft.X, bottomRight.Y );

            var topLeftCurveCenter = new Vector2( topLeft.X + radius, topLeft.Y + radius );
            var bottomLeftCurveCenter = new Vector2( bottomLeft.X + radius, bottomLeft.Y - radius );

            drawList.PathArcTo( topLeftCurveCenter, radius, DegreesToRadians( 180 ), DegreesToRadians( 270 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.PathArcTo( bottomLeftCurveCenter, radius, DegreesToRadians( 90 ), DegreesToRadians( 180 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.AddLine( topLeft + new Vector2( -0.5f, radius - 0.5f ), bottomLeft + new Vector2( -0.5f, -radius + 0.5f ), color, thickness );
            drawList.AddLine( topLeft + new Vector2( radius - 0.5f, -0.5f ), topRight + new Vector2( 0, -0.5f ), color, thickness );
            drawList.AddLine( bottomLeft + new Vector2( radius - 0.5f, -0.5f ), bottomRight + new Vector2( -4, -0.5f ), color, thickness );

            drawList.AddTriangleFilled( bottomRight, bottomRight + new Vector2( -arrowWidth, arrowHeight / 2 ), bottomRight + new Vector2( -arrowWidth, -arrowHeight / 2 ), color );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"Loaded {IdUpperCase}" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"{IdUpperCase} Being Replaced" );
        }

        private static float DegreesToRadians( float degrees ) => MathF.PI / 180 * degrees;

        protected virtual void DrawSearchBarsColumn() {
            ImGui.SetColumnWidth( 1, ImGui.GetWindowWidth() - 150 );
            ImGui.PushItemWidth( ImGui.GetColumnWidth() - 100 );
            DisplaySourceBar();
            DisplayReplaceBar();
            ImGui.PopItemWidth();
        }

        protected virtual bool ExtraInputColumn() => false;

        protected virtual void DrawExtraColumn() { }

        protected void DisplaySourceBar() {
            using var _ = ImRaii.PushId( "Source" );
            var sourceString = Source == null ? "" : Source.DisplayString;

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveSource();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputTextWithHint( "", "[NONE]", ref sourceString, 255, ImGuiInputTextFlags.ReadOnly );

            // Search
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowSource();
            }
        }

        protected void DisplayReplaceBar() {
            using var _ = ImRaii.PushId( "Replace" );
            var previewString = Replace == null ? "" : Replace.DisplayString;

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveReplace();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.InputTextWithHint( "", "[NONE]", ref previewString, 255, ImGuiInputTextFlags.ReadOnly );

            // Search
            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowReplace();
            }
        }

        protected void DisplayFileControls() {
            if( UiUtils.OkButton( "UPDATE" ) ) Update();

            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.FileDownload.ToIconString() ) ) ExportRaw();
            }
            UiUtils.Tooltip( "Export as a raw file.\nTo export as a Textools/Penumbra mod, use the \"mod export\" menu item" );

            ImGui.SameLine();
            UiUtils.ShowVerifiedStatus( Verified );
        }

        protected virtual void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( CurrentFile == null ) DisplayBeginHelpText();
            else {
                DisplayFileControls();
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                using var _ = ImRaii.PushId( "Body" );
                CurrentFile.Draw();
            }
        }

        public void DrawRename() {
            Name ??= "";
            using var _ = ImRaii.PushId( "Rename" );
            ImGui.InputTextWithHint( "", ReplaceDisplay, ref Name, 64, ImGuiInputTextFlags.AutoSelectAll );
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
            using var child = ImRaii.Child( "HelpTextChild", new Vector2( width, -1 ) );

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
        }

        private static readonly string Text = "DO NOT modify movement abilities (dashes, backflips). Please read a guide before attempting to modify a .tmb or .pap file";

        protected static void DisplayAnimationWarning() {
            using var color = ImRaii.PushColor( ImGuiCol.Border, new Vector4( 1, 0, 0, 0.3f ) );
            color.Push( ImGuiCol.ChildBg, new Vector4( 1, 0, 0, 0.1f ) );

            var style = ImGui.GetStyle();
            var textSize = ImGui.CalcTextSize( Text, ImGui.GetContentRegionMax().X - style.WindowPadding.X * 2 - 8 );

            using var child = ImRaii.Child( "AnimationWarningChild", new Vector2( -1,
                textSize.Y +
                style.WindowPadding.Y * 2 +
                style.ItemSpacing.Y +
                ImGui.GetTextLineHeightWithSpacing()
            ), true, ImGuiWindowFlags.NoScrollbar );

            ImGui.TextWrapped( Text );
            if( ImGui.SmallButton( "Guides##Pap" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );
        }
    }
}
