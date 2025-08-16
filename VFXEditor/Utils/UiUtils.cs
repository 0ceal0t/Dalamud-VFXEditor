using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Style;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.FileBrowser;

namespace VfxEditor.Utils {
    public enum VerifiedStatus {
        VERIFIED,
        ERROR,
        WORKSPACE,
        UNKNOWN,
        UNSUPPORTED,
        PARTIAL
    };

    public enum DraggingState {
        NotDragging,
        Dragging,
        Waiting
    };

    public static class UiUtils {
        public static readonly Vector4 RED_COLOR = new( 0.89098039216f, 0.30549019608f, 0.28980392157f, 1.0f );

        public static readonly Vector4 GREEN_COLOR = new( 0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f );

        public static readonly Vector4 DARK_GRAY = new( 0.21764705882f, 0.21764705882f, 0.21764705882f, 1 );

        public static Vector4 DALAMUD_RED => StyleModel.GetFromCurrent().BuiltInColors.DalamudRed.Value;

        public static Vector4 DALAMUD_YELLOW => StyleModel.GetFromCurrent().BuiltInColors.DalamudYellow.Value;

        public static Vector4 DALAMUD_ORANGE => StyleModel.GetFromCurrent().BuiltInColors.DalamudOrange.Value;

        public static Vector4 PARSED_GREEN => StyleModel.GetFromCurrent().BuiltInColors.ParsedGreen.Value;

        public static readonly string HIGH_PRECISION_FORMAT = "%.5f";

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

        public static bool TransparentButton( string label, Vector4 color, Vector2 size ) {
            using var style = ImRaii.PushColor( ImGuiCol.Text, color );
            style.Push( ImGuiCol.ButtonHovered, new Vector4( 0.710f, 0.710f, 0.710f, 0.2f ) );
            style.Push( ImGuiCol.ButtonActive, new Vector4( 0 ) );
            using var col = ImRaii.PushColor( ImGuiCol.Button, new Vector4( 0 ) );
            return ImGui.Button( label, size );
        }

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

        public static void DrawIntText( string label, int value ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            ImGui.TextDisabled( label );
            ImGui.SameLine();
            var dalamudColors = StyleModel.GetFromCurrent().BuiltInColors;
            ImGui.Text( $"{value}" );
        }

        public static void DrawBoolText( string label, bool value ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            ImGui.TextDisabled( label );
            ImGui.SameLine();
            ImGui.TextColored( value ? PARSED_GREEN : DALAMUD_RED, $"{value}" );
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

        public static void Tooltip( string text, bool hovered = false ) {
            if( hovered || ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( text );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void WikiButton( string url ) {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( TransparentButton( FontAwesomeIcon.InfoCircle.ToIconString(), DALAMUD_ORANGE ) ) OpenUrl( url );
            }
            Tooltip( "Click to view more information on the VFXEditor wiki" );
        }

        public static void ShowVerifiedStatus( VerifiedStatus verified ) {
            var color = verified switch {
                VerifiedStatus.VERIFIED or VerifiedStatus.PARTIAL => GREEN_COLOR,
                VerifiedStatus.ERROR => RED_COLOR,
                VerifiedStatus.WORKSPACE => new Vector4( 0.7f, 0.7f, 0.7f, 1.0f ),
                VerifiedStatus.UNKNOWN or VerifiedStatus.UNSUPPORTED => DALAMUD_ORANGE,
                _ => new Vector4( 1 )
            };

            var icon = verified switch {
                VerifiedStatus.VERIFIED or VerifiedStatus.PARTIAL => FontAwesomeIcon.Check.ToIconString(),
                VerifiedStatus.ERROR => FontAwesomeIcon.Times.ToIconString(),
                VerifiedStatus.WORKSPACE => FontAwesomeIcon.Circle.ToIconString(),
                VerifiedStatus.UNKNOWN => FontAwesomeIcon.QuestionCircle.ToIconString(),
                VerifiedStatus.UNSUPPORTED => FontAwesomeIcon.ExclamationCircle.ToIconString(),
                _ => FontAwesomeIcon.QuestionCircle.ToIconString()
            };

            var text = verified switch {
                VerifiedStatus.VERIFIED => "Verified",
                VerifiedStatus.PARTIAL => "Partially Verified",
                VerifiedStatus.ERROR => "Parsing Issues",
                VerifiedStatus.WORKSPACE => "Workspace",
                VerifiedStatus.UNKNOWN => "Unknown",
                VerifiedStatus.UNSUPPORTED => "Unsupported",
                _ => "[OTHER]"
            };

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.TextColored( color, icon );
            }

            ImGui.SameLine();
            ImGui.TextColored( color, text );

            if( verified == VerifiedStatus.UNSUPPORTED ) Tooltip( "Verification is not supported for this file or file type" );

            if( verified == VerifiedStatus.ERROR ) {
                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    if( RemoveButton( FontAwesomeIcon.Bug.ToIconString() ) ) {
                        OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/issues/new?assignees=&labels=bug&title=%5BPARSING+ISSUE%5D&body=What%20is%20the%20name%20or%20path%20of%20the%20file%20you%20are%20trying%20to%20open%3F%0A%0AWhat%20type%20of%20file%20is%20it%20(VFX%2C%20TMB%2C%20PAP%2C%20etc.)%3F" );
                    }
                }
                Tooltip( "Report issue on Github" );
            }
        }

        public static void WriteBytesDialog( string filter, byte[] data, string ext, string fileName ) {
            FileBrowserManager.SaveFileDialog( "Select a Save Location", filter, fileName, ext, ( ok, res ) => {
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

        public static bool IconButton( FontAwesomeIcon icon, string text ) {
            var buttonClicked = false;

            var iconSize = GetIconSize( icon );
            var textSize = ImGui.CalcTextSize( text );
            var padding = ImGui.GetStyle().FramePadding;
            var spacing = ImGui.GetStyle().ItemSpacing;

            var buttonSizeX = iconSize.X + textSize.X + padding.X * 2 + spacing.X;
            var buttonSizeY = ( iconSize.Y > textSize.Y ? iconSize.Y : textSize.Y ) + padding.Y * 2;
            var buttonSize = new Vector2( buttonSizeX, buttonSizeY );

            if( ImGui.Button( "##" + icon.ToIconString() + text, buttonSize ) ) buttonClicked = true;

            ImGui.SameLine();
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() - buttonSize.X - spacing.X + padding.X );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.Text( icon.ToIconString() );
            }

            ImGui.SameLine();
            ImGui.Text( text );

            return buttonClicked;
        }

        private delegate bool DrawRadiansInputDelegate<T>( string label, ref T value );

        public static bool DrawRadians( string name, float oldValue, out float newValue ) => DrawRadians( name, oldValue, out newValue, ToDegrees, ToRadians, ( string label, ref float value ) => ImGui.InputFloat( label, ref value ) );

        public static bool DrawRadians3( string name, Vector3 oldValue, out Vector3 newValue ) => DrawRadians( name, oldValue, out newValue, ToDegrees, ToRadians, ( string label, ref Vector3 value ) => ImGui.InputFloat3( label, ref value ) );

        public static bool DrawRadians4( string name, Vector4 oldValue, out Vector4 newValue ) => DrawRadians( name, oldValue, out newValue, ToDegrees, ToRadians, ( string label, ref Vector4 value ) => ImGui.InputFloat4( label, ref value ) );

        private static bool DrawRadians<T>( string name, T oldValue, out T newValue, Func<T, T> toDegrees, Func<T, T> toRadians, DrawRadiansInputDelegate<T> input ) {
            newValue = oldValue;
            var needToConvert = Plugin.Configuration.UseDegreesForAngles;
            var value = needToConvert ? toDegrees( oldValue ) : oldValue;

            ImGui.SetNextItemWidth( GetOffsetInputSize( 15 + ImGui.GetStyle().ItemInnerSpacing.X ) );
            if( input( "##Input", ref value ) ) {
                newValue = needToConvert ? toRadians( value ) : value;
                return true;
            }

            DrawAngleToggle( name );
            return false;
        }

        public static bool DrawDegrees3( string name, Vector3 oldValue, out Vector3 newValue ) {
            newValue = oldValue;
            var needToConvert = !Plugin.Configuration.UseDegreesForAngles;
            var value = needToConvert ? ToRadians( oldValue ) : oldValue;

            ImGui.SetNextItemWidth( GetOffsetInputSize( 15 + ImGui.GetStyle().ItemInnerSpacing.X ) );
            if( ImGui.InputFloat3( "##Input", ref value ) ) {
                newValue = needToConvert ? ToDegrees( value ) : value;
                return true;
            }

            DrawAngleToggle( name );
            return false;
        }

        public static void DrawAngleToggle( string name ) {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            ImGui.SameLine();
            if( ImGui.Button( Plugin.Configuration.UseDegreesForAngles ? "°" : "r", new Vector2( 15, ImGui.GetFrameHeight() ) ) ) {
                Plugin.Configuration.UseDegreesForAngles = !Plugin.Configuration.UseDegreesForAngles;
                Plugin.Configuration.Save();
            }
            Tooltip( "Switch between degrees and radians" );

            ImGui.SameLine();
            ImGui.Text( name );
        }

        public static float ToRadians( float value ) => ( float )( ( Math.PI / 180 ) * value );

        public static float ToDegrees( float value ) => ( float )( ( 180 / Math.PI ) * value );

        public static Vector3 ToRadians( Vector3 value ) => new( ToRadians( value.X ), ToRadians( value.Y ), ToRadians( value.Z ) );

        public static Vector3 ToDegrees( Vector3 value ) => new( ToDegrees( value.X ), ToDegrees( value.Y ), ToDegrees( value.Z ) );

        public static Vector4 ToRadians( Vector4 value ) => new( ToRadians( value.X ), ToRadians( value.Y ), ToRadians( value.Z ), ToRadians( value.W ) );

        public static Vector4 ToDegrees( Vector4 value ) => new( ToDegrees( value.X ), ToDegrees( value.Y ), ToDegrees( value.Z ), ToDegrees( value.W ) );

        public static bool MouseOver( Vector2 start, Vector2 end ) => Contains( start, end, ImGui.GetIO().MousePos );

        public static bool Contains( Vector2 min, Vector2 max, Vector2 point ) => point.X >= min.X && point.Y >= min.Y && point.X <= max.X && point.Y <= max.Y;

        public static bool DrawDragDrop<T>( List<T> items, T item, string text, ref T draggingItem, string id, bool record ) where T : class {
            var listModified = false;

            if( ImGui.BeginDragDropSource( ImGuiDragDropFlags.None ) ) {
                ImGui.SetDragDropPayload( id, null, 0 );
                draggingItem = item;
                ImGui.Text( text );
                ImGui.EndDragDropSource();
            }
            if( ImGui.BeginDragDropTarget() ) {
                if( StopDragging( items, item, ref draggingItem, id, record ) ) listModified = true;
                ImGui.EndDragDropTarget();
            }

            return listModified;
        }

        private static unsafe bool StopDragging<T>( List<T> items, T destination, ref T draggingItem, string id, bool record ) where T : class {
            if( draggingItem == null ) return false;

            var payload = ImGui.AcceptDragDropPayload( id );
            if( payload.Handle == null ) return false;

            if( draggingItem != destination && items.Contains( draggingItem ) && items.Contains( destination ) ) {
                var command = new ListMoveCommand<T>( items, draggingItem, destination );
                if( record ) CommandManager.Add( command );
            }
            draggingItem = null;
            return true;
        }

        public static unsafe bool DrawAngleUpDown( ref bool open ) {
            using var _ = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 2, 2 ) );
            ImGui.Separator();
            var offset = ImGui.GetContentRegionAvail().X / 2 - 15;
            ImGui.SetCursorPos( ImGui.GetCursorPos() + new Vector2( offset, 0 ) );
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, new Vector2( 5, 1 ) );
            if( TransparentButton( open ? FontAwesomeIcon.AngleDown.ToIconString() : FontAwesomeIcon.AngleUp.ToIconString(), *ImGui.GetStyleColorVec4( ImGuiCol.TextDisabled ), new( 30, 15 ) ) ) {
                open = !open;
                return true;
            }

            if( open ) ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );

            return false;
        }

        public static float AngleUpDownSize => 17 + ImGui.GetStyle().ItemSpacing.Y;

        public static readonly HashSet<Type> ForceOpenTabs = [];

        public static unsafe bool BeginTabItem<T>( string label ) {
            var labelBytes = Encoding.UTF8.GetBytes( label );
            var labelRef = stackalloc byte[labelBytes.Length + 1];
            Marshal.Copy( labelBytes, 0, new IntPtr( labelRef ), labelBytes.Length );

            var type = typeof( T );
            var forceOpen = ForceOpenTabs.Contains( type );
            if( forceOpen ) ForceOpenTabs.Remove( type );
            return ImGuiNative.BeginTabItem( labelRef, null, forceOpen ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None ) == 1;
        }
    }
}
