using Dalamud.Logging;
using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Utils;

namespace VfxEditor {
    public abstract class SelectTab {
        protected readonly SelectDialog Dialog;
        protected readonly string Name;

        public SelectTab( SelectDialog dialog, string name ) {
            Dialog = dialog;
            Name = name;
        }

        public abstract void Draw( string parentId );

        public void DrawPapDict( Dictionary<string, string> items, string label, string name, string id ) {
            foreach( var item in items ) {
                var skeleton = item.Key;
                var path = item.Value;

                ImGui.Text( $"{label} ({skeleton}): " );
                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                else DisplayPath( path );

                DrawPath( "", path, $"{id}{skeleton}", SelectResultType.GameAction, $"{name} {label} ({skeleton})" );
            }
        }

        public void DrawPath( string label, IEnumerable<string> paths, string id, SelectResultType resultType, string resultName, bool play = false ) {
            var idx = 0;
            foreach( var path in paths ) {
                DrawPath( $"{label} #{idx}", path, $"{id}-{idx}", resultType, $"{resultName} #{idx}", play );
                idx++;
            }
        }

        public void DrawPath( string label, string path, string id, SelectResultType resultType, string resultName, bool play = false ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            if( !string.IsNullOrEmpty( label ) ) { // if this is blank, assume there is some custom logic to draw the path
                ImGui.Text( $"{label}: " );
                ImGui.SameLine();
                DisplayPath( path );
            }

            if( ImGui.Button( $"SELECT{id}" ) ) {
                var resultPrefix = resultType.ToString().ToUpper().Replace( "GAME", "" );
                PluginLog.Log( id );
                Dialog.Invoke( new SelectResult( resultType, $"[{resultPrefix}] {resultName}", path ) );
            }
            ImGui.SameLine();
            Copy( path, id + "Copy" );
            if( play ) Dialog.Play( path, id: id + "Spawn" );
        }

        public static bool Matches( string item, string query ) => item.ToLower().Contains( query.ToLower() );

        public static void DisplayPath( string path ) {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }

        public static void DisplayPathWarning( string path, string warning ) {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.984375f, 0.7265625f, 0.01176470f, 1 ) );
            ImGui.TextWrapped( $"{path} (!)" );
            ImGui.PopStyleColor();
            if( ImGui.IsItemHovered() ) {
                ImGui.BeginTooltip();
                ImGui.PushTextWrapPos( ImGui.GetFontSize() * 35.0f );
                ImGui.TextUnformatted( warning );
                ImGui.PopTextWrapPos();
                ImGui.EndTooltip();
            }
        }

        public static void Copy( string copyPath, string id ) {
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.15f, 0.15f, 1 ) );
            if( ImGui.Button( "Copy" + id ) ) ImGui.SetClipboardText( copyPath );
            ImGui.PopStyleColor();
        }

        public static void DisplayVisible( int count, out int preItems, out int showItems, out int postItems, out float itemHeight ) {
            var childHeight = ImGui.GetWindowSize().Y - ImGui.GetCursorPosY();
            var scrollY = ImGui.GetScrollY();
            var style = ImGui.GetStyle();
            itemHeight = ImGui.GetTextLineHeight() + style.ItemSpacing.Y;
            preItems = ( int )Math.Floor( scrollY / itemHeight );
            showItems = ( int )Math.Ceiling( childHeight / itemHeight );
            postItems = count - showItems - preItems;

        }

        public static void DrawIcon( ImGuiScene.TextureWrap icon ) {
            if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) ImGui.Image( icon.ImGuiHandle, new Vector2( icon.Width, icon.Height ) );
        }

        public static void DrawThankYou() {
            ImGui.Text( "Thank you to Anamnesis/CMTools for compiling the list of NPC names" );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##Anamnesis" ) ) UiUtils.OpenUrl( "https://github.com/imchillin/Anamnesis" );
        }

        protected static void LoadIcon( ushort iconId, ref ImGuiScene.TextureWrap texWrap ) {
            texWrap?.Dispose();
            texWrap = null;
            if( iconId > 0 ) {
                TexFile tex;
                try {
                    tex = Plugin.DataManager.GetIcon( iconId );
                }
                catch( Exception ) {
                    tex = Plugin.DataManager.GetIcon( 0 );
                }
                texWrap = Plugin.PluginInterface.UiBuilder.LoadImageRaw( BGRA_to_RGBA( tex.ImageData ), tex.Header.Width, tex.Header.Height, 4 );
            }
        }

        protected static byte[] BGRA_to_RGBA( byte[] data ) {
            var ret = new byte[data.Length];
            for( var i = 0; i < data.Length / 4; i++ ) {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }
    }
}
