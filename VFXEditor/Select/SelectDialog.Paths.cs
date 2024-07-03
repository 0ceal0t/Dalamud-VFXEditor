using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Select.Tabs.BgmQuest;

namespace VfxEditor.Select {
    public partial class SelectDialog {
        // ========= UTILS =========

        public void DrawBgmSituation( string name, BgmSituationStruct situation, SelectResultType resultType ) {
            if( situation.IsSituation ) {
                DrawPaths( new Dictionary<string, string>() {
                    { "Day", situation.DayPath },
                    { "Night", situation.NightPath },
                    { "Battle", situation.BattlePath },
                    { "Daybreak", situation.DaybreakPath }
                }, name, resultType );
            }
            else DrawPaths( situation.Path, name, resultType );
        }

        public static bool DrawIcon( uint iconId, Vector2 size ) {
            if( iconId <= 0 ) return false;
            var icon = Dalamud.TextureProvider.GetFromGameIcon( iconId ).GetWrapOrDefault();
            if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( icon.ImGuiHandle, size );
                if( ImGui.IsItemHovered() ) {
                    ImGui.BeginTooltip();
                    ImGui.Image( icon.ImGuiHandle, icon.Size );
                    ImGui.EndTooltip();
                }

                return true;
            }
            return false;
        }

        // ========= HEADERS ========

        public void DrawPaths( Dictionary<string, List<string>> items, string resultName, SelectResultType resultType )
            => DrawPaths( items.ToDictionary( x => (x.Key, 0u), x => x.Value ), resultName, resultType );

        public void DrawPaths( Dictionary<(string, uint), List<string>> items, string resultName, SelectResultType resultType )
            => DrawPaths( items.ToDictionary( x => x.Key, x => x.Value.WithIndex().Select( y => ($"#{y.Index}", 0u, y.Value) ) ), resultName, resultType );

        public void DrawPaths( Dictionary<string, Dictionary<string, string>> items, string resultName, SelectResultType resultType )
            => DrawPaths( items.ToDictionary( x => (x.Key, 0u), x => x.Value ), resultName, resultType );

        public void DrawPaths( Dictionary<(string, uint), Dictionary<string, string>> items, string resultName, SelectResultType resultType )
            => DrawPaths( items.ToDictionary( x => x.Key, x => x.Value.Select( y => (y.Key, 0u, y.Value) ) ), resultName, resultType );

        public void DrawPaths( Dictionary<string, List<(string, uint, string)>> items, string resultName, SelectResultType resultType )
            => DrawPaths( items.ToDictionary( x => (x.Key, 0u), x => x.Value.Select( x => x ) ), resultName, resultType );

        public void DrawPaths( Dictionary<(string, uint), IEnumerable<(string, uint, string)>> items, string resultName, SelectResultType resultType ) {
            if( items == null || items.Count == 0 ) return;

            foreach( var ((name, iconId), paths) in items ) {
                if( !paths.Any() ) continue;

                using var _ = ImRaii.PushId( name );

                var showIcon = DrawIcon( iconId, new( 40, 40 ) );
                if( showIcon ) {
                    using var spacing = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                    ImGui.SameLine();
                }

                using var style = ImRaii.PushStyle( ImGuiStyleVar.FramePadding, ImGui.GetStyle().FramePadding with { Y = 12f }, showIcon );
                if( ImGui.CollapsingHeader( name, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    style.Dispose();
                    DrawPaths( paths, resultName, resultType );
                }
            }
        }

        // ========= INDEXED TABLE ===========

        public void DrawPaths( Dictionary<string, string> named, IEnumerable<string> indexed, string resultName, SelectResultType resultType )
            => DrawPaths( named.Select( x => (x.Key, x.Value) ), indexed, resultName, resultType );

        public void DrawPaths( IEnumerable<(string, string)> named, IEnumerable<string> indexed, string resultName, SelectResultType resultType ) {
            var paths = named.Select( x => (x.Item1, 0u, x.Item2) ).ToList();
            paths.AddRange( indexed.WithIndex().Select( x => ($"#{x.Index}", 0u, x.Value) ) );
            DrawPaths( paths, resultName, resultType );
        }

        public void DrawPaths( IEnumerable<string> paths, string resultName, SelectResultType resultType )
            => DrawPaths( paths.WithIndex().Select( x => ($"#{x.Index}", 0u, x.Value) ), resultName, resultType );

        // ========= SINGLE ===============

        public void DrawPaths( string path, string resultName, SelectResultType resultType )
            => DrawPaths( new List<(string, uint, string)> { (null, 0, path) }, resultName, resultType );

        // ========= TABLE ================

        public void DrawPaths( Dictionary<string, string> paths, string resultName, SelectResultType resultType )
            => DrawPaths( paths.Select( x => (x.Key, 0u, x.Value) ), resultName, resultType );

        public void DrawPaths( IEnumerable<(string, string)> paths, string resultName, SelectResultType resultType )
            => DrawPaths( paths.Select( x => (x.Item1, 0u, x.Item2) ), resultName, resultType );

        public void DrawPaths( IEnumerable<(string, uint, string)> paths, string resultName, SelectResultType resultType ) {
            if( paths == null || !paths.Any() ) return;

            using var _ = ImRaii.PushId( resultName );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 4 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var table = ImRaii.Table( "Table", 5,
                ImGuiTableFlags.RowBg | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.PadOuterX | ImGuiTableFlags.Hideable );
            if( !table ) return;
            padding.Dispose();

            ImGui.TableSetupColumn( "##Icon", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##FavoriteName", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##Path", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Controls", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##Play", ImGuiTableColumnFlags.None );

            ImGui.TableSetColumnEnabled( 0, paths.Where( x => x.Item2 > 0 ).Any() ); // show icon?
            ImGui.TableSetColumnEnabled( 4, CanPlay && resultType != SelectResultType.Local ); // show play button?

            foreach( var ((name, iconId, path), idx) in paths.WithIndex() ) {
                using var __ = ImRaii.PushId( idx );
                DrawPathRow(
                    iconId,
                    name,
                    path,
                    string.IsNullOrEmpty( name ) ? resultName : ( name.StartsWith( '#' ) ? $"{resultName} {name}" : $"{resultName} ({name})" ),
                    resultType
                );
            }
        }

        public void DrawPathRow( uint iconId, string label, string path, string resultName, SelectResultType resultType ) {
            if( string.IsNullOrEmpty( path ) || path.Contains( "BGM_Null" ) ) return;

            var displayPath = path;
            if( path.Contains( '|' ) ) {
                var split = path.Split( "|" );
                displayPath = split[0];
                path = split[1];
            }

            ImGui.TableNextRow();

            ImGui.TableNextColumn(); // Icon
            DrawIcon( iconId, new( 40, 40 ) );

            ImGui.TableNextColumn(); // Favorite + name
            DrawFavorite( path, resultName, resultType );
            if( !string.IsNullOrEmpty( label ) ) {
                ImGui.SameLine();
                ImGui.Text( label );
            }

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            ImGui.TableNextColumn(); // Path
            if( displayPath.Contains( "/action.pap" ) ) {
                SelectUiUtils.DisplayPathWarning( displayPath, "Be careful about modifying this file, as it contains dozens of animations for every job" );
            }
            else SelectUiUtils.DisplayPath( displayPath );

            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            ImGui.TableNextColumn(); // Controls
            if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) Invoke( SelectUiUtils.GetSelectResult( path, resultType, resultName ) );
            ImGui.SameLine();
            SelectUiUtils.Copy( path );

            ImGui.TableNextColumn(); // Play
            if( CanPlay && resultType != SelectResultType.Local ) PlayButton( path );
        }
    }
}
