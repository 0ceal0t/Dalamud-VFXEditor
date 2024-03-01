using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select {
    public partial class SelectTab {
        // ========= HEADERS ========

        protected void DrawPaths( Dictionary<string, Dictionary<string, string>> items, string resultName ) => DrawPaths( items.ToDictionary( x => (x.Key, 0u), x => x.Value ), resultName );

        protected void DrawPaths( Dictionary<(string, uint), Dictionary<string, string>> items, string resultName ) {
            if( items == null || items.Count == 0 ) return;

            foreach( var (((name, iconId), paths), idx) in items.WithIndex() ) {
                if( !paths.Any() ) continue;

                using var _ = ImRaii.PushId( name );
                if( ImGui.CollapsingHeader( name, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    if( iconId > 0 ) {
                        var icon = Dalamud.TextureProvider.GetIcon( iconId, IconFlags.None );
                        if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) ImGui.Image( icon.ImGuiHandle, new Vector2( 40, 40 ) );
                    }

                    DrawPaths( paths.Select( x => (x.Key, 0u, x.Value) ), resultName, idx != items.Count - 1 );
                }
            }
        }

        // ========= INDEXED TABLE ===========

        protected void DrawPaths( Dictionary<string, string> named, IEnumerable<string> indexed, string resultName, bool limit = false ) => DrawPaths( named.Select( x => (x.Key, x.Value) ), indexed, resultName, limit );

        protected void DrawPaths( IEnumerable<(string, string)> named, IEnumerable<string> indexed, string resultName, bool limit = false ) {
            var paths = named.Select( x => (x.Item1, 0u, x.Item2) ).ToList();
            paths.AddRange( indexed.WithIndex().Select( x => ($"#{x.Index}", 0u, x.Value) ) );
            DrawPaths( paths, resultName, limit );
        }

        protected void DrawPaths( IEnumerable<string> paths, string resultName, bool limit = false ) => DrawPaths( paths.WithIndex().Select( x => ($"#{x.Index}", 0u, x.Value) ), resultName, limit );

        // ========= TABLE ================

        protected void DrawPaths( Dictionary<string, string> paths, string resultName, bool limit = false ) => DrawPaths( paths.Select( x => (x.Key, 0u, x.Value) ), resultName, limit );

        protected void DrawPaths( IEnumerable<(string, string)> paths, string resultName, bool limit = false ) => DrawPaths( paths.Select( x => (x.Item1, 0u, x.Item2) ), resultName, limit );

        protected void DrawPaths( IEnumerable<(string, uint, string)> paths, string resultName, bool limit = false ) {
            if( paths == null || !paths.Any() ) return;

            var hasIcons = paths.Where( x => x.Item2 > 0 ).Any();
            var childSize = limit ? new Vector2( -1, ( ( hasIcons ? 25f : ImGui.GetFrameHeight() ) + 8f ) * paths.Count() ) : new Vector2( -1 ); // TODO

            using var _ = ImRaii.PushId( resultName );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 4 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", childSize, false );
            using var table = ImRaii.Table( "Table", 5,
                ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.PadOuterX | ImGuiTableFlags.Hideable );
            if( !table ) return;
            padding.Dispose();

            ImGui.TableSetupColumn( "##Icon", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##FavoriteName", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##Path", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Controls", ImGuiTableColumnFlags.None );
            ImGui.TableSetupColumn( "##Play", ImGuiTableColumnFlags.None );

            ImGui.TableSetColumnEnabled( 0, hasIcons ); // show icon?
            ImGui.TableSetColumnEnabled( 4, Dialog.CanPlay && ResultType != SelectResultType.Local ); // show play button?

            foreach( var ((name, iconId, path), idx) in paths.WithIndex() ) {
                using var __ = ImRaii.PushId( idx );
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                if( iconId > 0 ) {
                    var icon = Dalamud.TextureProvider.GetIcon( iconId, IconFlags.None );
                    if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) ImGui.Image( icon.ImGuiHandle, new Vector2( 40, 40 ) );
                }

                DrawPathRow( name, path, name.StartsWith( '#' ) ? $"{resultName} {name}" : $"{resultName} ({name})" );
            }
        }

        protected void DrawPathRow( string label, string path, string resultName ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            ImGui.TableNextColumn(); // Favorite + name
            DrawFavorite( path, resultName );
            if( !string.IsNullOrEmpty( label ) ) {
                ImGui.SameLine();
                ImGui.Text( label );
            }

            ImGui.TableNextColumn(); // Path
            if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) {
                SelectUiUtils.DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
            }
            else SelectUiUtils.DisplayPath( path );

            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            ImGui.TableNextColumn(); // Controls
            if( ImGui.Button( FontAwesomeIcon.Check.ToIconString() ) ) Dialog.Invoke( SelectUiUtils.GetSelectResult( path, ResultType, resultName ) );
            ImGui.SameLine();
            SelectUiUtils.Copy( path );

            ImGui.TableNextColumn(); // Play
            if( Dialog.CanPlay && ResultType != SelectResultType.Local ) Dialog.PlayButton( path );
        }
    }
}
