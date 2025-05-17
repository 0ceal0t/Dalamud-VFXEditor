using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract class FileManagerDocument<R, S> : IFileDocument where R : FileManagerFile {
        public R File { get; protected set; }
        protected VerifiedStatus Verified => File == null ? VerifiedStatus.UNKNOWN : File.Verified;
        public bool Unsaved => File != null && File.Unsaved;
        public bool LocalSource => Source != null && Source.Type == SelectResultType.Local;

        public string DisplayName => string.IsNullOrEmpty( Name ) ? ReplaceDisplay : Name;
        protected string Name = "";

        protected SelectResult Source;
        protected SelectResult Replace;
        public string SourceDisplay => Source == null ? "[NONE]" : Source.DisplayString;
        public string ReplaceDisplay => Replace == null ? "[NONE]" : Replace.DisplayString;
        public string ReplacePath => ( Disabled || Replace == null ) ? "" : Replace.Path;
        protected bool Disabled = false;

        private string SourceTextInput = "";
        private string ReplaceTextInput = "";

        public string WriteLocation { get; protected set; }

        public abstract string Id { get; }
        public abstract string Extension { get; }

        protected readonly FileManagerBase Manager;

        protected DateTime LastUpdate = DateTime.Now;

        public FileManagerDocument( FileManagerBase manager, string writeLocation ) {
            Manager = manager;
            WriteLocation = writeLocation;
        }

        public bool GetReplacePath( string path, out string replacePath ) {
            replacePath = null;
            if( File == null ) return false;

            replacePath = ReplacePath.ToLower().Equals( path.ToLower() ) ? WriteLocation : null;
            return !string.IsNullOrEmpty( replacePath );
        }

        protected abstract R FileFromReader( BinaryReader reader, bool verify );

        protected void LoadLocal( string path, bool verify ) {
            if( !System.IO.File.Exists( path ) ) {
                Dalamud.Error( $"Local file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                using var reader = new BinaryReader( System.IO.File.Open( path, FileMode.Open ) );
                File?.Dispose();
                File = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error Reading File" );
                Dalamud.ErrorNotification( "Error reading file" );
            }
        }

        protected void LoadGame( string path, bool verify ) {
            if( !Dalamud.DataManager.FileExists( path ) ) {
                Dalamud.Error( $"Game file: [{path}] does not exist" );
                return;
            }

            if( !path.EndsWith( $".{Extension}" ) ) {
                Dalamud.Error( $"{path} is the wrong file type" );
                return;
            }

            try {
                var file = Dalamud.DataManager.GetFile( path );
                using var ms = new MemoryStream( file.Data );
                using var reader = new BinaryReader( ms );
                File?.Dispose();
                File = FileFromReader( reader, verify );
            }
            catch( Exception e ) {
                Dalamud.Error( e, "Error Reading File: " + path );
                Dalamud.ErrorNotification( "Error reading file: " + path );
            }
        }

        // =================

        public void SetSource( SelectResult result ) {
            if( result == null ) return;
            Source = result;
            SourceTextInput = "";

            if( result.Type == SelectResultType.Local ) LoadLocal( result.Path, true );
            else LoadGame( result.Path, true );

            if( File != null ) {
                WriteFile( WriteLocation );
            }
        }

        protected void RemoveSource() {
            File?.Dispose();
            File = null;
            Source = null;
            SourceTextInput = "";
        }

        public void SetReplace( SelectResult result ) {
            Replace = result;
            ReplaceTextInput = "";
            Plugin.AddCustomBackupLocation( Replace, WriteLocation );
        }

        protected void RemoveReplace() {
            Replace = null;
            ReplaceTextInput = "";
        }

        // =====================

        protected void WriteFile( string path ) {
            if( File == null ) return;
            if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"Wrote {Id} file to {path}" );
            System.IO.File.WriteAllBytes( path, File.ToBytes() );
        }

        protected void ExportRawDialog() => UiUtils.WriteBytesDialog( $".{Extension}", File.ToBytes(), Extension, "ExportedFile" );

        public void Update() {
            if( ( DateTime.Now - LastUpdate ).TotalSeconds <= 0.2 ) return;
            LastUpdate = DateTime.Now;

            File?.Update();

            var newWriteLocation = Manager.NewWriteLocation;
            WriteFile( newWriteLocation );
            WriteLocation = newWriteLocation;
            Plugin.AddCustomBackupLocation( Replace, WriteLocation );

            if( File != null && !ReplacePath.Contains( ".sklb" ) ) {
                Plugin.ResourceLoader.ReloadPath( ReplacePath, WriteLocation, File.GetPapIds(), File.GetPapTypes() );
            }

            Plugin.ResourceLoader.ReRender();
        }

        // =======================

        protected void LoadWorkspace( string localPath, string relativeLocation, string name, SelectResult source, SelectResult replace, bool disabled ) {
            Name = name ?? "";
            Source = source;
            Replace = replace;
            Disabled = disabled;

            SourceTextInput = "";
            ReplaceTextInput = "";

            LoadLocal( WorkspaceUtils.ResolveWorkspacePath( relativeLocation, localPath ), false );
            if( File != null ) File.Verified = VerifiedStatus.WORKSPACE;
            WriteFile( WriteLocation );
        }

        public string GetExportSource() => SourceDisplay;

        public string GetExportReplace() => DisplayName;

        public bool CanExport() => File != null && !string.IsNullOrEmpty( ReplacePath );

        public void PenumbraExport( string modFolder, string groupOption, Dictionary<string, string> filesOut ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || File == null ) return;
            var data = File.ToBytes();

            PenumbraUtils.WriteBytes( data, modFolder, groupOption, path, filesOut );
        }

        public void TextoolsExport( BinaryWriter writer, List<TTMPL_Simple> simplePartsOut, ref int modOffset ) {
            var path = ReplacePath;
            if( string.IsNullOrEmpty( path ) || File == null ) return;

            var modData = TexToolsUtils.CreateType2Data( File.ToBytes() );
            simplePartsOut.Add( TexToolsUtils.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }

        public abstract S GetWorkspaceMeta( string newPath );

        public void WorkspaceExport( List<S> meta, string rootPath, string newPath ) {
            if( File == null ) return;

            var newFullPath = Path.Combine( rootPath, newPath );
            System.IO.File.WriteAllBytes( newFullPath, File.ToBytes() );
            meta.Add( GetWorkspaceMeta( newPath ) );
        }

        // ====== DRAWING ==========

        public virtual void CheckKeybinds() {
            if( Plugin.Configuration.UpdateKeybind.KeyPressed() ) Update();
        }

        public void Draw() {
            if( Plugin.Configuration.WriteLocationError ) {
                ImGui.TextWrapped( $"VFXEditor does not have access to {Plugin.Configuration.WriteLocation}. Please go to [File > Settings] and change it, then restart your game" );
                return;
            }

            var searchWidth = ImGui.GetContentRegionAvail().X - 160 - 125;

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0 ) ) )
            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.Columns( 3, "Columns", false );
                ImGui.SetColumnWidth( 0, 160 );
            }
            DrawInputTextColumn();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.NextColumn();
                ImGui.SetColumnWidth( 1, searchWidth );
            }
            DrawSearchBarsColumn();

            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.NextColumn();
                ImGui.SetColumnWidth( 2, 126 );
            }
            DrawExtraColumn();

            ImGui.Columns( 1 );

            DrawBody();
        }

        protected virtual void DrawInputTextColumn() {
            var pos = ImGui.GetCursorScreenPos() + new Vector2( 5, 0 );
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

            var hovered = ImGui.IsWindowFocused( ImGuiFocusedFlags.RootWindow ) && ImGui.IsMouseHoveringRect( topLeft - new Vector2( 5, 5 ), bottomRight + new Vector2( 5, 5 ) );

            var color = hovered ?
                ImGui.ColorConvertFloat4ToU32( UiUtils.DALAMUD_ORANGE ) :
                ( Disabled ?
                    ImGui.ColorConvertFloat4ToU32( UiUtils.RED_COLOR ) :
                    ImGui.GetColorU32( ImGuiCol.TextDisabled )
               );

            if( hovered && ImGui.IsMouseClicked( ImGuiMouseButton.Left ) ) Disabled = !Disabled;

            var topLeftCurveCenter = new Vector2( topLeft.X + radius, topLeft.Y + radius );
            var bottomLeftCurveCenter = new Vector2( bottomLeft.X + radius, bottomLeft.Y - radius );

            drawList.PathArcTo( topLeftCurveCenter, radius, DegreesToRadians( 180 ), DegreesToRadians( 270 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.PathArcTo( bottomLeftCurveCenter, radius, DegreesToRadians( 90 ), DegreesToRadians( 180 ), segmentResolution );
            drawList.PathStroke( color, ImDrawFlags.None, thickness );

            drawList.AddLine( topLeft + new Vector2( -0.5f, radius - 0.5f ), bottomLeft + new Vector2( -0.5f, -radius + 0.5f ), color, thickness );
            drawList.AddLine( topLeft + new Vector2( radius - 0.5f, -0.5f ), topRight + new Vector2( 0, -0.5f ), color, thickness );
            drawList.AddLine( bottomLeft + new Vector2( radius - 0.5f, -0.5f ), bottomRight + new Vector2( -4, -0.5f ), color, thickness );

            if( Disabled ) {
                var crossCenter = bottomRight + new Vector2( -4, 0 );
                var crossHeight = arrowHeight / 2;

                drawList.AddLine( crossCenter + new Vector2( crossHeight, crossHeight ), crossCenter + new Vector2( -crossHeight, -crossHeight ), color, thickness );
                drawList.AddLine( crossCenter + new Vector2( -crossHeight, crossHeight ), crossCenter + new Vector2( crossHeight, -crossHeight ), color, thickness );
            }
            else {
                drawList.AddTriangleFilled( bottomRight, bottomRight + new Vector2( -arrowWidth, arrowHeight / 2 ), bottomRight + new Vector2( -arrowWidth, -arrowHeight / 2 ), color );
            }

            if( hovered ) UiUtils.Tooltip( "Toggle replacement", true );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"Loaded {Id}" );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 25 );
            ImGui.Text( $"{Id} Being Replaced" );
        }

        private static float DegreesToRadians( float degrees ) => MathF.PI / 180 * degrees;

        protected void DrawSearchBarsColumn() {
            var timesWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Times );
            var searchWidth = UiUtils.GetPaddedIconSize( FontAwesomeIcon.Search );
            // 3 * 2 for spacing, 25 for some more padding
            var inputWidth = ImGui.GetColumnWidth() - timesWidth - searchWidth - ( 3 * 2 ) - 20;

            DisplaySourceBar( inputWidth );
            DisplayReplaceBar( inputWidth );
        }

        protected virtual void DrawExtraColumn() { }

        // ====== TEXT INPUTS ============

        protected void DisplaySourceBar( float inputSize ) {
            using var _ = ImRaii.PushId( "Source" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );

            var reloadWidth = UiUtils.GetIconSize( FontAwesomeIcon.Sync ).X;
            var replaceWidth = UiUtils.GetIconSize( FontAwesomeIcon.Upload ).Y;

            inputSize -= ( reloadWidth + replaceWidth + ( ImGui.GetStyle().FramePadding.X * 4 ) + 6 );

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveSource();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            using( var color = ImRaii.PushColor( ImGuiCol.TextDisabled, UiUtils.DALAMUD_YELLOW, Source != null ) ) {
                if( ImGui.InputTextWithHint( "", SourceDisplay, ref SourceTextInput, 255, ImGuiInputTextFlags.EnterReturnsTrue ) ) {
                    var cleanedPath = SourceTextInput.Trim().Replace( "\\", "/" );
                    var result = new SelectResult( SelectResultType.GamePath, cleanedPath, $"[GAME] {cleanedPath}", cleanedPath );
                    SetSource( result );
                    Plugin.Configuration.AddRecent( Manager.Configuration.RecentItems, result );
                }
            }
            DrawCopy( Source );

            // Local files
            using( var disabled = ImRaii.Disabled( !LocalSource ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Sync.ToIconString() ) ) {
                    Manager.SetSource( Source );
                    Dalamud.OkNotification( "Reloaded " + Source.Path );
                }
                ImGui.SameLine();
                if( ImGui.Button( FontAwesomeIcon.Upload.ToIconString() ) ) {
                    System.IO.File.WriteAllBytes( Source.Path, File.ToBytes() );
                    Dalamud.OkNotification( "Exported to " + Source.Path );
                }
            }

            // Search
            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowSource();
            }
        }

        protected void DisplayReplaceBar( float inputSize ) {
            using var _ = ImRaii.PushId( "Replace" );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 3, 4 ) );

            // Remove
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( UiUtils.TransparentButton( FontAwesomeIcon.Times.ToIconString(), UiUtils.RED_COLOR ) ) RemoveReplace();
            }

            // Input
            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            using( var color = ImRaii.PushColor( ImGuiCol.TextDisabled, UiUtils.DALAMUD_YELLOW, Replace != null ) ) {
                if( ImGui.InputTextWithHint( "", ReplaceDisplay, ref ReplaceTextInput, 255, ImGuiInputTextFlags.EnterReturnsTrue ) ) {
                    var cleanedPath = ReplaceTextInput.Trim().Replace( "\\", "/" );
                    var result = new SelectResult( SelectResultType.GamePath, cleanedPath, $"[GAME] {cleanedPath}", cleanedPath );
                    SetReplace( result );
                    Plugin.Configuration.AddRecent( Manager.Configuration.RecentItems, result );
                }
            }
            DrawCopy( Replace );

            // Search
            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowReplace();
            }
        }

        private static void DrawCopy( SelectResult result ) {
            if( result == null ) return;
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "CopyPopup" );

            using var popup = ImRaii.Popup( "CopyPopup" );
            if( !popup ) return;
            ImGui.TextDisabled( result.Path );
            ImGui.SameLine();
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( 4, -2 ) );
            SelectUiUtils.Copy( result.Path );
        }

        // ==========================

        protected virtual void DisplayFileControls() {
            if( UiUtils.OkButton( "UPDATE" ) ) Update();

            using( var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, ImGui.GetStyle().FramePadding + new Vector2( 0, 1 ) ) )
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Download.ToIconString() ) ) ExportRawDialog();
            }
            UiUtils.Tooltip( "Export as a raw file" );

            ImGui.SameLine();
            UiUtils.ShowVerifiedStatus( Verified );

            var warnings = GetWarningText();
            if( !string.IsNullOrEmpty( warnings ) ) {
                using var _ = ImRaii.PushColor( ImGuiCol.Text, UiUtils.RED_COLOR );
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.ExclamationCircle.ToIconString() );
                }
                UiUtils.Tooltip( warnings );
            }
        }

        protected virtual string GetWarningText() => "";

        protected virtual void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( File == null ) {
                DisplayBeginHelpText();
                return;
            }

            DisplayFileControls();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Body" );
            File.Draw();
        }

        public void DrawRename() {
            Name ??= "";
            using var _ = ImRaii.PushId( "Rename" );
            ImGui.InputTextWithHint( "", ReplaceDisplay, ref Name, 64, ImGuiInputTextFlags.AutoSelectAll );
        }

        public virtual void Dispose() {
            File?.Dispose();
            File = null;
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

        private static readonly string WarningText = "DO NOT modify movement abilities (dashes, backflips). Please read a guide before attempting to modify a .tmb or .pap file";

        protected static void DrawAnimationWarning() {
            using var color = ImRaii.PushColor( ImGuiCol.Border, new Vector4( 1, 0, 0, 0.3f ) );
            color.Push( ImGuiCol.ChildBg, new Vector4( 1, 0, 0, 0.1f ) );

            var style = ImGui.GetStyle();
            var iconSize = UiUtils.GetIconSize( FontAwesomeIcon.Globe ) + 2 * style.FramePadding;
            var textWidth = ImGui.GetContentRegionAvail().X - ( 2 * style.WindowPadding.X ) - ( 2 * style.ItemSpacing.X ) - iconSize.X;
            var textSize = ImGui.CalcTextSize( WarningText, textWidth );

            using var child = ImRaii.Child( "Warning", new Vector2( -1, Math.Max( textSize.Y, iconSize.Y ) + ( 2 * style.WindowPadding.Y ) ), true, ImGuiWindowFlags.NoScrollbar );
            using( var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 0 ) ) ) {
                ImGui.Columns( 2, "##WarningColumns", false );
                ImGui.SetColumnWidth( 0, textWidth );
            }

            using( var textColor = ImRaii.PushColor( ImGuiCol.Text, 0xFF4A67FF ) ) {
                ImGui.TextWrapped( WarningText );
            }

            ImGui.NextColumn();
            ImGui.SetColumnWidth( 1, iconSize.X + ( 2 * style.ItemSpacing.X ) );

            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.Globe.ToIconString() ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki" );

            ImGui.Columns( 1 );
        }
    }
}
