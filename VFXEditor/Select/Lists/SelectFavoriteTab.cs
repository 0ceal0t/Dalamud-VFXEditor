using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Select.Base;
using VfxEditor.Select.Favorite;
using VfxEditor.Utils;

namespace VfxEditor.Select.Lists {
    public class SelectFavoriteTab : SelectTabBase {
        protected readonly List<SelectFavoriteItem> Items;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectFavoriteItem> items ) : base( dialog, name ) {
            Items = items;
        }

        protected override int NumColumns() => 2;

        protected override void DrawTable() {
            ImGui.TableSetupColumn( "##Tree", ImGuiTableColumnFlags.WidthStretch );
            ImGui.TableSetupColumn( "##Type", ImGuiTableColumnFlags.WidthFixed, 100 );

            if( Items == null ) return;

            foreach( var item in Items ) {
                if( DrawRow( item, Items ) ) break;
            }
        }

        protected bool DrawRow( SelectFavoriteItem row, List<SelectFavoriteItem> parent ) {
            // TODO: search input
            // TODO: drag and drop

            if( !row.IsFolder && row.Result.Type == SelectResultType.Local && !Dialog.ShowLocal ) return false;

            using var _ = ImRaii.PushId( row.Id );
            ImGui.TableNextRow();

            if( !row.IsFolder ) {
                var result = row.Result;
                ImGui.TableNextColumn();
                ImGui.TreeNodeEx( $"##Folder", ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.AllowItemOverlap );
                if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "EditPopup" );
                if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() ) {
                    Dialog.Invoke( result );
                    return true;
                }

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
                using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.DALAMUD_YELLOW ) ) {
                    ImGui.Text( FontAwesomeIcon.Star.ToIconString() );

                    if( ImGui.IsItemClicked() ) {
                        parent.Remove( row );
                        Plugin.Configuration.Save();
                        return true;
                    }
                }

                ImGui.SameLine();
                ImGui.Text( string.IsNullOrEmpty( result.Name ) ? result.DisplayString.Split( "]" )[^1] : result.Name );

                ImGui.TableNextColumn();
                ImGui.TextDisabled( "[" + result.Type.ToString().ToUpper().Replace( "GAME", "" ) + "]" );
            }
            else {
                ImGui.TableNextColumn();
                var open = ImGui.TreeNodeEx( $"##Folder", ImGuiTreeNodeFlags.SpanFullWidth );
                var rightClicked = ImGui.IsItemClicked( ImGuiMouseButton.Right );

                ImGui.SameLine();
                using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                    ImGui.Text( FontAwesomeIcon.Folder.ToIconString() );
                }

                ImGui.SameLine();
                ImGui.Text( row.FolderName );

                ImGui.TableNextColumn();
                // Type, skip

                if( open ) {
                    foreach( var item in row.Items ) {
                        // TODO
                        if( DrawRow( item, row.Items ) ) {
                            ImGui.TreePop();
                            return true;
                        }
                    }

                    ImGui.TreePop();
                }

                if( rightClicked ) ImGui.OpenPopup( "EditPopup" );
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
                    parent.Remove( row );
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

        /*
        private SelectResult DraggingItem;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool PostRow( SelectResult item, int idx ) {
            if( UiUtils.DrawDragDrop( Items, item, item?.DisplayString ?? string.Empty, ref DraggingItem, $"{Name}-FAVORITE", false ) ) {
                Plugin.Configuration.Save();
                return true;
            }

            if( base.PostRow( item, idx ) ) return true;

            // ======== POPUP ============

            using var _ = ImRaii.PushId( idx );

            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "FavoritePopup" );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, DefaultWindowPadding );
            if( ImGui.BeginPopup( "FavoritePopup" ) ) {
                var name = string.IsNullOrEmpty( item.Name ) ? item.DisplayString.Split( "]" )[^1] : item.Name;
                if( ImGui.InputText( "##Rename", ref name, 128, ImGuiInputTextFlags.AutoSelectAll ) ) {
                    item.Name = name;
                    Plugin.Configuration.Save();
                }

                if( item.Type != SelectResultType.Local ) Dialog.PlayPopupItems( item.Path );
                ImGui.EndPopup();
            }

            return false;
        }*/
    }
}