using Dalamud.Interface;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.IO;
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

        public static bool EnumComboBox( string label, string[] options, ref int choiceIdx ) {
            var ret = false;
            if( ImGui.BeginCombo( label, options[choiceIdx] ) ) {
                for( var idx = 0; idx < options.Length; idx++ ) {
                    var is_selected = ( choiceIdx == idx );
                    if( ImGui.Selectable( options[idx], is_selected ) ) {
                        choiceIdx = idx;
                        ret = true;
                    }

                    if( is_selected ) {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
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
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void WikiButton( string url ) {
            if (ImGui.SmallButton( $"Wiki" ) ) {
                OpenUrl( url );
            }
        }

        public static void OkNotification( string content, string? title = "VFXEditor") {
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
    }
}
