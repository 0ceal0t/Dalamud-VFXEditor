using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Data.Command.ListCommands;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components.Tables {
    public class CommandTable<T> where T : class, IUiItem {
        protected readonly string Id;
        protected readonly bool AllowNewDelete;
        protected readonly bool ShowRow;
        protected readonly List<T> Items;
        protected readonly List<(string, ImGuiTableColumnFlags, int)> Columns;

        private readonly Func<T> NewAction;
        private readonly Action<T, bool> OnChangeAction;

        public CommandTable( string id, bool allowNewDelete, bool showRow, List<T> items, List<(string, ImGuiTableColumnFlags, int)> columns, Func<T> newAction, Action<T, bool> onChangeAction = null ) {
            Id = id;
            AllowNewDelete = allowNewDelete;
            ShowRow = showRow;
            Items = items;
            Columns = columns;

            NewAction = newAction;
            OnChangeAction = onChangeAction;
        }

        public void Draw() => Draw( new( -1 ) );

        public void Draw( Vector2 childSize ) {
            using var _ = ImRaii.PushId( Id );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 4 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var child = ImRaii.Child( "Child", childSize, false );
            using var table = ImRaii.Table( "Table", Columns.Count + ( AllowNewDelete ? 1 : 0 ) + ( ShowRow ? 1 : 0 ),
                ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.PadOuterX );
            if( !table ) return;

            padding.Dispose();

            ImGui.TableSetupScrollFreeze( 0, 1 );

            // ===== HEADER =====

            if( AllowNewDelete ) {
                ImGui.TableSetupColumn( "##Controls", ImGuiTableColumnFlags.None, -1 );
            }

            if( ShowRow ) {
                ImGui.TableSetupColumn( "##Row", ImGuiTableColumnFlags.None, -1 );
            }

            foreach( var (name, flags, size) in Columns ) {
                ImGui.TableSetupColumn( name, flags | ImGuiTableColumnFlags.NoResize, size );
            }

            ImGui.TableHeadersRow();

            // ===== BODY =========

            foreach( var (item, idx) in Items.WithIndex() ) {
                ImGui.TableNextRow();
                using var __ = ImRaii.PushId( idx );

                if( AllowNewDelete ) {
                    ImGui.TableNextColumn();
                    using var font = ImRaii.PushFont( UiBuilder.IconFont );
                    if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                        CommandManager.Add( new ListRemoveCommand<T>( Items, item, OnChangeAction ) );
                        break;
                    }
                }

                if( ShowRow ) {
                    using var font = ImRaii.PushFont( UiBuilder.MonoFont );
                    ImGui.TableNextColumn();
                    ImGui.TextDisabled( $"{idx}" );
                }

                item.Draw();
            }

            // ======= NEW =========

            if( AllowNewDelete ) {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                    CommandManager.Add( new ListAddCommand<T>( Items, NewAction.Invoke(), OnChangeAction ) );
                }
            }
        }
    }
}
