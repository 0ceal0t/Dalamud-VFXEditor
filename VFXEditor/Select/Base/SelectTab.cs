using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor.Select.Base;

namespace VfxEditor.Select {
    public abstract class SelectTab {
        protected readonly SelectDialog Dialog;
        protected readonly string Name;

        public SelectTab( SelectDialog dialog, string name ) {
            Dialog = dialog;
            Name = name;
        }

        public abstract void Draw();
    }

    // ===== LOAD SINGLE ========

    public class SelectTabState<T> {
        public List<T> Items = [];
        public bool ItemsLoaded = false;
        public bool WaitingForItems = false;
    }

    public abstract class SelectTab<T> : SelectTab where T : class, ISelectItem {
        // Using this so that we don't have to query for tab entries multiple times
        private static readonly Dictionary<string, object> States = [];

        private readonly string StateId;

        protected readonly SelectTabState<T> State;
        protected bool ItemsLoaded => State.ItemsLoaded;
        protected bool WaitingForItems => State.WaitingForItems;
        protected List<T> Items => State.Items;

        protected T Selected = default;
        protected string SearchInput = "";
        protected List<T> Searched;

        protected SelectTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name ) {
            StateId = stateId;
            if( States.TryGetValue( StateId, out var existingState ) ) {
                State = ( SelectTabState<T> )existingState;
            }
            else {
                State = new SelectTabState<T>();
                States.Add( StateId, State );
            }
        }

        protected virtual bool CheckMatch( T item, string searchInput ) => SelectUiUtils.Matches( item.GetName(), searchInput );

        protected abstract void DrawSelected();

        protected virtual void DrawExtra() { }

        protected virtual bool IsDisabled() => false;

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );

            using var disabled = ImRaii.Disabled( IsDisabled() );
            using var tabItem = ImRaii.TabItem( Name );
            if( !tabItem || IsDisabled() ) return;

            Load();

            if( !ItemsLoaded ) return;

            Searched ??= [.. Items];

            var resetScroll = false;
            DrawExtra();
            if( ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 ) ) {
                Searched = Items.Where( x => CheckMatch( x, SearchInput ) ).ToList();
                resetScroll = true;
            }

            ImGui.Separator();

            // Navigate through items using the up and down arrow buttons
            if( KeybindConfiguration.NavigateUpDown( Searched, Selected, out var newSelected ) ) Select( newSelected );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
            using var table = ImRaii.Table( "Table", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendY, new( -1, ImGui.GetContentRegionAvail().Y ) );
            if( !table ) return;
            style.Dispose();

            ImGui.TableSetupColumn( "##Left", ImGuiTableColumnFlags.WidthFixed, 200 );
            ImGui.TableSetupColumn( "##Right", ImGuiTableColumnFlags.WidthStretch );

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            using( var tree = ImRaii.Child( "Tree" ) ) {
                var iconOnLeft = Plugin.Configuration.SelectDialogIconsOnLeft && typeof( T ).IsAssignableTo( typeof( ISelectItemWithIcon ) );
                var itemHeight = iconOnLeft ? 25 : ImGui.GetTextLineHeight() + ImGui.GetStyle().ItemSpacing.Y;
                SelectUiUtils.DisplayVisible( Searched.Count, itemHeight, out var preItems, out var showItems, out var postItems );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );

                if( resetScroll ) ImGui.SetScrollHereY();

                var idx = 0;
                foreach( var item in Searched ) {
                    if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                    using var __ = ImRaii.PushId( idx );

                    if( iconOnLeft ) {
                        // based on https://github.com/Etheirys/Brio/blob/93950203668f672f8ff67a4fcf9d8758418b783f/Brio/UI/Controls/Selectors/GearSelector.cs#L12
                        var startPos = ImGui.GetCursorPos();
                        if( ImGui.Selectable( "###Entry", Selected == item, ImGuiSelectableFlags.AllowDoubleClick, new Vector2( 0, itemHeight ) ) && Selected != item ) Select( item );
                        var endPos = ImGui.GetCursorPos();

                        ImGui.SetCursorPos( startPos );
                        using( var group = ImRaii.Group() ) {
                            if( group ) {
                                var iconId = ( item as ISelectItemWithIcon ).GetIconId();
                                if( iconId <= 0 ) {
                                    ImGui.Dummy( new( itemHeight, itemHeight ) );
                                }
                                else {
                                    var icon = Dalamud.TextureProvider.GetFromGameIcon( iconId ).GetWrapOrDefault();
                                    if( icon != null && icon.Handle != IntPtr.Zero ) {
                                        ImGui.Image( icon.Handle, new Vector2( ( itemHeight / icon.Height ) * icon.Width, itemHeight ) );
                                    }
                                }

                                ImGui.SameLine();
                                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + ( itemHeight - ImGui.GetTextLineHeight() ) / 2f );
                                ImGui.Text( item.GetName() );
                            }


                        }
                        ImGui.SetCursorPos( endPos );
                    }
                    else {
                        if( ImGui.Selectable( item.GetName(), Selected == item ) && Selected != item ) Select( item );
                    }

                    idx++;
                }

                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            }

            ImGui.TableNextColumn();

            if( Selected == null ) ImGui.Text( "Select an item..." );
            else DrawInner();
        }

        protected virtual void DrawInner() {
            using var child = ImRaii.Child( "Child" );

            ImGui.Text( Selected.GetName() );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( ISelectItemWithIcon.HasIcon( Selected, out var iconId ) ) DrawIcon( iconId );
            DrawSelected();
        }

        protected virtual void Select( T item ) {
            Selected = item;
        }

        public virtual async void Load() {
            if( WaitingForItems || ItemsLoaded ) return;
            State.WaitingForItems = true;
            Dalamud.Log( "Loading " + StateId );

            await Task.Run( () => {
                try {
                    LoadData();
                }
                catch( Exception e ) {
                    Dalamud.Error( e, "Error Loading: " + StateId );
                }

                State.ItemsLoaded = true;
            } );
        }

        public abstract void LoadData();

        protected static void DrawIcon( uint iconId ) {
            if( iconId <= 0 ) return;

            var icon = Dalamud.TextureProvider.GetFromGameIcon( iconId ).GetWrapOrDefault();
            if( icon != null && icon.Handle != IntPtr.Zero ) {
                ImGui.Image( icon.Handle, new Vector2( icon.Width, icon.Height ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }
        }
    }

    // ======= LOAD DOUBLE ========

    public abstract class SelectTab<T, S> : SelectTab<T> where T : class, ISelectItem where S : class {
        protected S Loaded;

        protected SelectTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        protected override void Select( T item ) {
            LoadItemAsync( item );
            base.Select( item );
        }

        protected override void DrawInner() {
            if( Loaded != null ) {
                using var child = ImRaii.Child( "Child" );
                ImGui.Text( Selected.GetName() );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                if( ISelectItemWithIcon.HasIcon( Selected, out var iconId ) ) DrawIcon( iconId );
                DrawSelected();
            }
            else ImGui.Text( "No data found" );
        }

        private async void LoadItemAsync( T item ) {
            Loaded = null;
            await Task.Run( () => {
                LoadSelection( item, out Loaded );
            } );
        }

        public abstract void LoadSelection( T item, out S loaded );
    }
}
