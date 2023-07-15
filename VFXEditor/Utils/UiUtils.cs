using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace VfxEditor.Utils {
    public enum VerifiedStatus {
        ISSUE,
        UNKNOWN,
        OK
    };

    public enum DraggingState {
        NotDragging,
        Dragging,
        Waiting
    };

    public static class UiUtils {
        public static readonly Vector4 RED_COLOR = new( 0.89098039216f, 0.30549019608f, 0.28980392157f, 1.0f );

        public static readonly Vector4 GREEN_COLOR = new( 0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f );

        public static readonly Vector4 YELLOW_COLOR = new( 0.984375f, 0.7265625f, 0.01176470f, 1.0f );

        public static string GetArticle( string input ) => "aeiouAEIOU".Contains( input[0] ) ? "an" : "a";

        public static bool EnumComboBox<T>( string label, T[] options, T currentValue, out T newValue ) =>
            EnumComboBox( label, $"{currentValue}", options, currentValue, out newValue );

        public static bool EnumComboBox<T>( string label, string text, T[] options, T currentValue, out T newValue ) {
            newValue = currentValue;
            using var combo = ImRaii.Combo( label, text );
            if( !combo ) return false;

            foreach( var option in options ) {
                var selected = currentValue.Equals( option );
                if( ImGui.Selectable( $"{option}", selected ) ) {
                    newValue = option;
                    return true;
                }
                if( selected ) ImGui.SetItemDefaultFocus();
            }
            return false;
        }

        public static bool DisabledButton( string label, bool enabled, bool small = false ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, !enabled );
            return ( small ? ImGui.SmallButton( label ) : ImGui.Button( label ) ) && enabled;
        }

        public static bool DisabledTransparentButton( string label, Vector4 color, bool enabled ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f, !enabled );
            return TransparentButton( label, color ) && enabled;
        }

        public static bool RemoveButton( string label, bool small = false ) => ColorButton( label, RED_COLOR, small );

        public static bool OkButton( string label, bool small = false ) => ColorButton( label, GREEN_COLOR, small );

        public static bool TransparentButton( string label, Vector4 color ) {
            using var style = ImRaii.PushColor( ImGuiCol.Text, color );
            style.Push( ImGuiCol.ButtonHovered, new Vector4( 0.710f, 0.710f, 0.710f, 0.2f ) );
            style.Push( ImGuiCol.ButtonActive, new Vector4( 0 ) );
            return ColorButton( label, new Vector4( 0 ), false );
        }

        public static bool ColorButton( string label, Vector4 color, bool small ) {
            using var style = ImRaii.PushColor( ImGuiCol.Button, color );
            return small ? ImGui.SmallButton( label ) : ImGui.Button( label );
        }

        public static void HelpMarker( string text ) {
            IconText( FontAwesomeIcon.InfoCircle, true );
            Tooltip( text );
        }

        public static void IconText( FontAwesomeIcon icon, bool disabled = false ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( disabled ) ImGui.TextDisabled( icon.ToIconString() );
            else ImGui.Text( icon.ToIconString() );
        }

        public static void Tooltip( string text ) {
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void WikiButton( string url ) {
            if( ImGui.SmallButton( $"Wiki" ) ) OpenUrl( url );
        }

#nullable enable
        public static void OkNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Success );
        }

        public static void ErrorNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Error );
        }

        public static void WarningNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Warning );
        }
#nullable disable

        public static void ShowVerifiedStatus( VerifiedStatus verified ) {
            var color = verified switch {
                VerifiedStatus.OK => GREEN_COLOR,
                VerifiedStatus.ISSUE => RED_COLOR,
                _ => new Vector4( 0.7f, 0.7f, 0.7f, 1.0f )
            };

            var icon = verified switch {
                VerifiedStatus.OK => FontAwesomeIcon.Check.ToIconString(),
                VerifiedStatus.ISSUE => FontAwesomeIcon.Times.ToIconString(),
                _ => FontAwesomeIcon.Circle.ToIconString()
            };

            var text = verified switch {
                VerifiedStatus.OK => "Verified",
                VerifiedStatus.ISSUE => "Parsing Issues",
                _ => "Workspace"
            };

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.TextColored( color, icon );
            }

            ImGui.SameLine();
            ImGui.TextColored( color, text );

            if( verified == VerifiedStatus.ISSUE ) {
                ImGui.SameLine();
                if( ColorButton( "Report this error", RED_COLOR, false ) ) {
                    OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues/new?assignees=&labels=bug&title=%5BPARSING+ISSUE%5D&body=What%20is%20the%20name%20or%20path%20of%20the%20file%20you%20are%20trying%20to%20open%3F%0A%0AWhat%20type%20of%20file%20is%20it%20(VFX%2C%20TMB%2C%20PAP%2C%20etc.)%3F" );
                }
            }
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", filter, "", ext, ( bool ok, string res ) => {
                if( ok ) File.WriteAllBytes( res, data );
            } );
        }

        public static void OpenUrl( string url ) => Process.Start( new ProcessStartInfo {
            FileName = url,
            UseShellExecute = true
        } );

        private static readonly Random random = new();

        public static string RandomString( int length ) =>
            new( Enumerable.Repeat( "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length ).Select( x => x[random.Next( x.Length )] ).ToArray() );

        public static float GetWindowContentRegionWidth() => ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;

        public static bool MouseOver( Vector2 start, Vector2 end ) => Contains( start, end, ImGui.GetIO().MousePos );

        public static bool MouseClicked() => ImGui.IsMouseClicked( ImGuiMouseButton.Left );

        public static bool DoubleClicked() => ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left );

        public static bool Contains( Vector2 min, Vector2 max, Vector2 point ) => point.X >= min.X && point.Y >= min.Y && point.X <= max.X && point.Y <= max.Y;

        public static float Lerp( float firstFloat, float secondFloat, float by ) => firstFloat * ( 1 - by ) + secondFloat * by;

        public static float GetOffsetInputSize( FontAwesomeIcon icon ) => GetOffsetInputSize( GetPaddedIconSize( icon ) );

        public static float GetOffsetInputSize( float size ) => ( ImGui.GetWindowSize().X ) * 0.65f - size;

        public static float GetPaddedIconSize( FontAwesomeIcon icon ) {
            var style = ImGui.GetStyle();
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            return ImGui.CalcTextSize( icon.ToIconString() ).X + style.FramePadding.X * 2 + style.ItemInnerSpacing.X;
        }

        public static Vector2 GetIconSize( FontAwesomeIcon icon ) {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            return ImGui.CalcTextSize( icon.ToIconString() );
        }

        public static bool IconSelectable( FontAwesomeIcon icon, string text ) {
            var ret = false;

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ret = ImGui.Selectable( icon.ToIconString() );
            }

            ImGui.SameLine();
            ImGui.Text( text );

            return ret;
        }

        public static bool IconButton( FontAwesomeIcon icon, string text, string tooltip = "" ) {
            var buttonClicked = false;

            var iconSize = GetIconSize( icon );
            var textSize = ImGui.CalcTextSize( text );
            var padding = ImGui.GetStyle().FramePadding;
            var spacing = ImGui.GetStyle().ItemSpacing;

            var buttonSizeX = iconSize.X + textSize.X + padding.X * 2 + spacing.X;
            var buttonSizeY = ( iconSize.Y > textSize.Y ? iconSize.Y : textSize.Y ) + padding.Y * 2;
            var buttonSize = new Vector2( buttonSizeX, buttonSizeY );

            if( ImGui.Button( "##" + icon.ToIconString() + text, buttonSize ) ) buttonClicked = true;
            var hovered = ImGui.IsItemHovered();

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - buttonSize.X - spacing.X + padding.X );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.Text( icon.ToIconString() );
            }

            ImGui.SameLine();
            ImGui.Text( text );

            if( !string.IsNullOrEmpty( tooltip ) && hovered ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( tooltip );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }

            return buttonClicked;
        }
    }
}
