using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Data;
using VfxEditor.Select.Base;
using VfxEditor.Ui.DragDrop;
using VfxEditor.Utils;

namespace VfxEditor.Select.Favorite {
    public class SelectFavoriteTab : SelectTabBase {
        protected readonly List<SelectFavoriteItem> Items;
        protected readonly SelectFavoriteItem Root;

        public readonly DragDropManager<SelectFavoriteItem> DragDropManager;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectFavoriteItem> items ) : base( dialog, name ) {
            Items = items;
            Root = new SelectFavoriteItem() {
                IsFolder = true,
                Items = Items
            };

            DragDropManager = new( $"{Name}_FAVORITE",
                x => x.IsFolder,
                x => x.IsFolder ? x.Items : null,
                Plugin.Configuration.Save,
                x => x.DragDropName
            );
        }

        protected override int NumColumns() => 2;

        protected override void DrawTable() {
            ImGui.TableSetupColumn( "##Tree", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Type", ImGuiTableColumnFlags.WidthFixed, 100 );

            if( Items == null ) return;

            foreach( var item in Items ) {
                if( DrawRow( item, Root ) ) break;
            }

            // Move into root
            ImGui.TableNextRow();
            ImGui.TableSetBgColor( ImGuiTableBgTarget.RowBg0, 0 );
            ImGui.TableNextColumn();
            const float inset = 4f;
            ImGui.SetCursorPosX( ImGui.GetCursorPosX() + inset );
            var height = ImGui.GetContentRegionAvail().Y - ImGui.GetStyle().CellPadding.Y * 2;
            ImGui.Dummy( new Vector2( ImGui.GetContentRegionAvail().X - inset, Math.Max( height, ImGui.GetFrameHeight() ) ) );
            DragDropManager.DropTarget( Root, null, false );
        }

        protected bool DrawRow( SelectFavoriteItem row, SelectFavoriteItem parent ) {
            if( !row.IsFolder && row.Result.Type == SelectResultType.Local && !Dialog.ShowLocal ) return false;
            if( !row.Matches( SearchInput ) ) return false;

            using var _ = ImRaii.PushId( row.Id );
            ImGui.TableNextRow();

            if( !row.IsFolder ) {
                var result = row.Result;
                ImGui.TableNextColumn();
                ImGui.TreeNodeEx( $"##Folder", ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.AllowItemOverlap );
                if( DragDropManager.DragDrop( row, parent ) ) return true;
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "EditPopup" );
                if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() ) {
                    Dialog.Invoke( result );
                    return true;
                }

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
                using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.YELLOW_ACCENT ) ) {
                    ImGui.Text( FontAwesomeIcon.Star.ToIconString() );

                    if( ImGui.IsItemClicked() ) {
                        parent.Items.Remove( row );
                        Plugin.Configuration.Save();
                        return true;
                    }
                }

                using( ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 15, 5 ) ) ) {
                    ImGui.SameLine();
                }
                ImGui.Text( string.IsNullOrEmpty( result.Name ) ? result.DisplayString.Split( "]" )[^1] : result.Name );

                ImGui.TableNextColumn();
                ImGui.TextDisabled( "[" + result.Type.ToString().ToUpperInvariant().Replace( "GAME", "" ) + "]" );
            }
            else {
                ImGui.TableNextColumn();

                var listModified = false;

                if( DragDropManager.DrawDropTarget( row, parent, true, "BeforeFolder" ) ) listModified = true;

                var open = ImGui.TreeNodeEx( $"##Folder", ImGuiTreeNodeFlags.SpanFullWidth );
                if( DragDropManager.DragDrop( row, parent ) ) listModified = true;
                var rightClicked = ImGui.IsItemClicked( ImGuiMouseButton.Right );

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.Folder.ToIconString() );
                }

                using( ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, new Vector2( 15, 5 ) ) ) {
                    ImGui.SameLine();
                }
                ImGui.Text( row.FolderName );

                ImGui.TableNextColumn();
                // Type, skip

                if( open ) {
                    foreach( var item in row.Items ) {
                        if( DrawRow( item, row ) ) {
                            listModified = true;
                            break;
                        }
                    }

                    ImGui.TreePop();
                }

                if( rightClicked ) ImGui.OpenPopup( "EditPopup" );

                if( listModified ) return true;
            }

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, DefaultWindowPadding );
            using var popup = ImRaii.Popup( "EditPopup" );
            if( popup ) {
                ImGui.SetNextItemWidth( 400f );
                if( row.IsFolder ) {
                    ImGui.InputText( "##Rename", ref row.FolderName, 256 );
                    if( ImGui.IsItemDeactivatedAfterEdit() ) Plugin.Configuration.Save();
                }
                else {
                    var name = string.IsNullOrEmpty( row.Result.Name ) ? row.Result.DisplayString.Split( "]" )[^1] : row.Result.Name;
                    if( ImGui.InputText( "##Rename", ref name, 256 ) ) row.Result.Name = name;
                    if( ImGui.IsItemDeactivatedAfterEdit() ) Plugin.Configuration.Save();
                }

                if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                    parent.Items.Remove( row );
                    Plugin.Configuration.Save();
                    return true;
                }
                if( !row.IsFolder && row.Result?.Type != SelectResultType.Local ) Dialog.PlayPopupItems( row.Result?.Path );
            }

            return false;
        }

        protected override void DrawControls() {
            ImGui.SameLine();
            using var font = ImRaii.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( FontAwesomeIcon.FolderPlus.ToIconString() ) ) {
                Items.Add( new SelectFavoriteItem() {
                    IsFolder = true
                } );
                Plugin.Configuration.Save();
            }
        }
    }
}