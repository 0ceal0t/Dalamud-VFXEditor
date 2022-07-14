using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace VFXEditor.Helper {
    public enum VerifiedStatus {
        ISSUE,
        UNKNOWN,
        OK
    };

    public static class UIHelper {
        public static readonly Vector4 RED_COLOR = new( 0.85098039216f, 0.32549019608f, 0.30980392157f, 1.0f );
        public static readonly Vector4 GREEN_COLOR = new( 0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f );

        public static bool EnumComboBox<T>( string label, T[] options, T currentValue, out T newValue) {
            newValue = currentValue;
            if( ImGui.BeginCombo( label, $"{currentValue}" ) ) {
                foreach (var option in options ) {
                    if( ImGui.Selectable( $"{option}", currentValue.Equals( option ) ) ) {
                        newValue = option;

                        ImGui.EndCombo();
                        return true;
                    }
                }
                ImGui.EndCombo();
            }
            return false;
        }

        public static bool DisabledButton( string label, bool enabled, bool small = false ) {
            if( !enabled ) ImGui.PushStyleVar( ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f );
            if( ( small ? ImGui.SmallButton( label ) : ImGui.Button( label ) ) && enabled ) return true;
            if( !enabled ) ImGui.PopStyleVar();
            return false;
        }

        public static bool RemoveButton( string label, bool small = false ) => ColorButton( label, RED_COLOR, small );

        public static bool OkButton( string label, bool small = false ) => ColorButton( label, GREEN_COLOR, small );

        public static bool ColorButton( string label, Vector4 color, bool small ) {
            var ret = false;
            ImGui.PushStyleColor( ImGuiCol.Button, color );
            if( small ) {
                if( ImGui.SmallButton( label ) ) {
                    ret = true;
                }
            }
            else {
                if( ImGui.Button( label ) ) {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }

        public static int ColorToInt( Vector4 color ) {
            var data = new byte[] { ( byte )color.X, ( byte )color.Y, ( byte )color.Z, ( byte )color.W };
            return BitConverter.ToInt32( data );
        }

        public static Vector4 IntToColor( int color ) {
            var colors = BitConverter.GetBytes( color );
            return new Vector4( colors[0], colors[1], colors[2], colors[3] );
        }

        public static void HelpMarker( string text ) {
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - 5 );
            ImGui.TextDisabled( "(?)" );
            Tooltip( text );
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
            if( ImGui.SmallButton( $"Wiki" ) ) {
                OpenUrl( url );
            }
        }

        public static void OkNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Success );
        }

        public static void ErrorNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Error );
        }

        public static void WarningNotification( string content, string? title = "VFXEditor" ) {
            Plugin.PluginInterface.UiBuilder.AddNotification( content, title, Dalamud.Interface.Internal.Notifications.NotificationType.Warning );
        }

        public static void ShowVerifiedStatus( VerifiedStatus verified ) {
            ImGui.PushFont( UiBuilder.IconFont );

            var color = verified switch {
                VerifiedStatus.OK => GREEN_COLOR,
                VerifiedStatus.ISSUE => RED_COLOR,
                _ => new Vector4( 0.7f, 0.7f, 0.7f, 1.0f )
            };

            var icon = verified switch {
                VerifiedStatus.OK => $"{( char )FontAwesomeIcon.Check}",
                VerifiedStatus.ISSUE => $"{( char )FontAwesomeIcon.Times}",
                _ => $"{( char )FontAwesomeIcon.Question}"
            };

            var text = verified switch {
                VerifiedStatus.OK => "Verified",
                VerifiedStatus.ISSUE => "Parsing Issues",
                _ => "Unverified"
            };

            ImGui.TextColored( color, icon );
            ImGui.PopFont();
            ImGui.SameLine();
            ImGui.TextColored( color, text );

            if( verified == VerifiedStatus.ISSUE ) {
                ImGui.SameLine();
                if( ColorButton( "Report this error", RED_COLOR, false ) ) {
                    OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues/new?assignees=&labels=bug&title=%5BPARSING+ISSUE%5D&body=What%20is%20the%20name%20or%20path%20of%20the%20file%20you%20are%20trying%20to%20open%3F%0A%0AWhat%20type%20of%20file%20is%20it%20(VFX%2C%20TMB%2C%20PAP%2C%20etc.)%3F" );
                }
            }
        }

        public static void WriteBytesDialog( string filter, string data, string ext ) {
            WriteBytesDialog( filter, Encoding.ASCII.GetBytes( data ), ext );
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext ) {
            FileDialogManager.SaveFileDialog( "Select a Save Location", filter, "", ext, ( bool ok, string res ) => {
                if( ok ) File.WriteAllBytes( res, data );
            } );
        }

        public static void OpenUrl( string url ) {
            Process.Start( new ProcessStartInfo {
                FileName = url,
                UseShellExecute = true
            } );
        }

        private static readonly Random random = new();

        public static string RandomString( int length ) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string( Enumerable.Repeat( chars, length )
                .Select( s => s[random.Next( s.Length )] ).ToArray() );
        }

        public static float GetWindowContentRegionWidth() => ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;
    }
}
