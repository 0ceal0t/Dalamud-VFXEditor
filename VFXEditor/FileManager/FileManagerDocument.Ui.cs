using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Interop.Penumbra;
using VfxEditor.Select;
using VfxEditor.Ui.Export;
using VfxEditor.Utils;

namespace VfxEditor.FileManager {
    public abstract partial class FileManagerDocument<R, S> {
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

            if( DrawBar( inputSize, SourceDisplay, ref SourceTextInput, Source, Manager.SourceSelect.Focused, out var result ) ) SetSource( result );

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

            if( DrawBar( inputSize, ReplaceDisplay, ref ReplaceTextInput, Replace, Manager.ReplaceSelect.Focused, out var result ) ) SetReplace( result );

            // Search
            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Search.ToIconString() ) ) Manager.ShowReplace();
            }
        }

        private bool DrawBar( float inputSize, string display, ref string inputText, SelectResult selectResult, bool highlight, out SelectResult result ) {
            using var border = ImRaii.PushStyle( ImGuiStyleVar.FrameBorderSize, 2f, highlight );
            using var borderColor = ImRaii.PushColor( ImGuiCol.Border, UiUtils.DALAMUD_YELLOW, highlight );

            var edited = false;
            result = null;

            ImGui.SameLine();
            ImGui.SetNextItemWidth( inputSize );
            using( var color = ImRaii.PushColor( ImGuiCol.TextDisabled, UiUtils.DALAMUD_YELLOW, selectResult != null ) ) {
                if( ImGui.InputTextWithHint( "", display, ref inputText, 255, ImGuiInputTextFlags.EnterReturnsTrue ) ) {
                    var cleanedPath = inputText.Trim().Replace( "\\", "/" );
                    result = new SelectResult( SelectResultType.GamePath, cleanedPath, $"[GAME] {cleanedPath}", cleanedPath );
                    edited = true;
                    Plugin.Configuration.AddRecent( Manager.Configuration.RecentItems, result );
                }
            }
            DrawCopy( selectResult );
            return edited;
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

        // Reasons a change will not show up in-game through Penumbra. Shown under the file controls
        protected virtual List<string> GetPreviewWarnings() {
            return [];
        }

        private void DrawPreviewWarnings() {
            if( PenumbraPreview.IsUiPath( ReplacePath ) && !PenumbraPreview.UiPreviewAvailable ) PenumbraPreview.DrawUiCollectionWarning();

            // Several documents (possibly in different windows) replacing the same file: only the first one is applied
            if( File != null && !string.IsNullOrEmpty( ReplacePath ) ) {
                var winner = PenumbraPreview.GetWinningDocument( ReplacePath );
                if( winner != null && winner != this ) {
                    using var winnerColor = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_YELLOW );
                    ImGui.TextWrapped( "Another open document also replaces this file and is being used instead. Disable or close the other document to see these changes." );
                }
            }

            var warnings = GetPreviewWarnings();
            if( warnings.Count == 0 ) return;

            using var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_YELLOW );
            foreach( var warning in warnings ) ImGui.TextWrapped( warning );
        }

        protected virtual void DrawBody() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            ImGui.Separator();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( File == null ) {
                DisplayBeginHelpText();
                return;
            }

            DisplayFileControls();
            DrawPreviewWarnings();
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            using var _ = ImRaii.PushId( "Body" );
            File.Draw();
        }

        public void DrawRename() {
            Name ??= "";
            using var _ = ImRaii.PushId( "Rename" );
            ImGui.InputTextWithHint( "", ReplaceDisplay, ref Name, 64, ImGuiInputTextFlags.AutoSelectAll );
        }


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
            var textSize = ImGui.CalcTextSize( WarningText, wrapWidth: textWidth );

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