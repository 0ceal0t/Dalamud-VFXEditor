using Dalamud.Interface;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Select.Rows;
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

                Dialog.DrawFavorite( path, SelectResultType.GameAction, $"{name} {label} ({skeleton})" );
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
                Dialog.DrawFavorite( path, resultType, resultName );
                ImGui.Text( $"{label}:" );
                ImGui.SameLine();
                DisplayPath( path );
            }

            ImGui.Indent( 25f );

            if( ImGui.Button( $"SELECT{id}" ) ) Dialog.Invoke( GetSelectResult( path, resultType, resultName ) );
            ImGui.SameLine();
            Copy( path, id + "Copy" );
            if( play ) Dialog.Play( path, id + "/Spawn" );

            ImGui.Unindent( 25f );
        }

        public void DrawBgmSituation( string name, string parentId, BgmSituationStruct situation ) {
            if( situation.IsSituation ) {
                DrawPath( "Daytime Bgm", situation.DayPath, $"{parentId}/Day", SelectResultType.GameMusic, $"{name} / Day" );
                DrawPath( "Nighttime Bgm", situation.NightPath, $"{parentId}/Night", SelectResultType.GameMusic, $"{name} / Night" );
                DrawPath( "Battle Bgm", situation.BattlePath, $"{parentId}/Battle", SelectResultType.GameMusic, $"{name} / Battle" );
                DrawPath( "Daybreak Bgm", situation.DaybreakPath, $"{parentId}/Break", SelectResultType.GameMusic, $"{name} / Break" );
            }
            else DrawPath( "Bgm", situation.Path, parentId, SelectResultType.GameZone, name );
        }

        // ========================

        public static SelectResult GetSelectResult( string path, SelectResultType resultType, string resultName ) {
            var resultPrefix = resultType.ToString().ToUpper().Replace( "GAME", "" );
            return new SelectResult( resultType, $"[{resultPrefix}] {resultName}", path );
        }

        public static void DisplayNoVfx() {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
            ImGui.TextWrapped( $"This item does not have a VFX. See the link below for information on adding one" );
            ImGui.PopStyleColor();
            if( ImGui.SmallButton( "Guide##NoVfx" ) ) UiUtils.OpenUrl( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Adding-a-VFX-to-an-Item-Without-One" );
        }

        public static bool Matches( string item, string query ) => item.ToLower().Contains( query.ToLower() );

        public static void DisplayPath( string path ) {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }

        public static void DisplayPathWarning( string path, string warning ) {
            ImGui.PushStyleColor( ImGuiCol.Text, UiUtils.YELLOW_COLOR );
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
            if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( icon.ImGuiHandle, new Vector2( icon.Width, icon.Height ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }
        }

        public static void NpcThankYou() {
            ImGui.TextDisabled( "Npc file list provided by ResLogger" );
            ImGui.SameLine();
            if( ImGui.SmallButton( "Github##ResLogger" ) ) UiUtils.OpenUrl( "https://github.com/lmcintyre/ResLogger2" );
        }

        protected static void LoadIcon( uint iconId, ref ImGuiScene.TextureWrap texWrap ) {
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
                texWrap = Plugin.PluginInterface.UiBuilder.LoadImageRaw( BgraToRgba( tex.ImageData ), tex.Header.Width, tex.Header.Height, 4 );
            }
        }

        protected static byte[] BgraToRgba( byte[] data ) {
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
