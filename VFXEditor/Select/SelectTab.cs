using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor.Select.Tabs.BgmQuest;
using static Dalamud.Plugin.Services.ITextureProvider;

namespace VfxEditor.Select {
    public abstract partial class SelectTab {
        protected readonly SelectDialog Dialog;
        protected readonly string Name;
        protected readonly SelectResultType ResultType;

        public SelectTab( SelectDialog dialog, string name, SelectResultType resultType ) {
            Dialog = dialog;
            Name = name;
            ResultType = resultType;
        }

        public abstract void Draw();

        // ====== DRAWING UTILITY FUNCTIONS ===========

        protected bool DrawFavorite( string path, string resultName ) => Dialog.DrawFavorite( SelectUiUtils.GetSelectResult( path, ResultType, resultName ) );

        protected void DrawBgmSituation( string name, BgmSituationStruct situation ) { // TODO
            if( situation.IsSituation ) {
                DrawPath( "Daytime Bgm", situation.DayPath, $"{name} / Day" );
                DrawPath( "Nighttime Bgm", situation.NightPath, $"{name} / Night" );
                DrawPath( "Battle Bgm", situation.BattlePath, $"{name} / Battle" );
                DrawPath( "Daybreak Bgm", situation.DaybreakPath, $"{name} / Break" );
            }
            else DrawPath( "Bgm", situation.Path, name );
        }

        protected static void DrawIcon( uint iconId ) {
            if( iconId <= 0 ) return;

            var icon = Dalamud.TextureProvider.GetIcon( iconId, IconFlags.None );
            if( icon != null && icon.ImGuiHandle != IntPtr.Zero ) {
                ImGui.Image( icon.ImGuiHandle, new Vector2( icon.Width, icon.Height ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 3 );
            }
        }
    }

    // ===== LOAD SINGLE ========

    public class SelectTabState<T> {
        public List<T> Items = new();
        public bool ItemsLoaded = false;
        public bool WaitingForItems = false;
    }

    public abstract class SelectTab<T> : SelectTab where T : class {
        // Using this so that we don't have to query for tab entries multiple times
        private static readonly Dictionary<string, object> States = new();

        private readonly string StateId;

        protected readonly SelectTabState<T> State;
        protected bool ItemsLoaded => State.ItemsLoaded;
        protected bool WaitingForItems => State.WaitingForItems;
        protected List<T> Items => State.Items;

        protected T Selected = default;
        protected string SearchInput = "";
        protected List<T> Searched;

        protected SelectTab( SelectDialog dialog, string name, string stateId, SelectResultType resultType ) : base( dialog, name, resultType ) {
            StateId = stateId;
            if( States.TryGetValue( StateId, out var existingState ) ) {
                State = ( SelectTabState<T> )existingState;
            }
            else {
                State = new SelectTabState<T>();
                States.Add( StateId, State );
            }
        }

        protected abstract string GetName( T item );

        protected virtual bool CheckMatch( T item, string searchInput ) => SelectUiUtils.Matches( GetName( item ), searchInput );

        protected abstract void DrawSelected();

        protected virtual void DrawExtra() { }

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );

            using var tabItem = ImRaii.TabItem( Name );
            if( !tabItem ) return;

            Load();

            if( !ItemsLoaded ) return;

            Searched ??= new List<T>( Items );

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
                SelectUiUtils.DisplayVisible( Searched.Count, out var preItems, out var showItems, out var postItems, out var itemHeight );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );

                if( resetScroll ) ImGui.SetScrollHereY();

                var idx = 0;
                foreach( var item in Searched ) {
                    if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }

                    using var __ = ImRaii.PushId( idx );
                    if( ImGui.Selectable( GetName( item ), Selected == item ) ) {
                        if( Selected != item ) Select( item ); // not what is currently selected
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

            ImGui.Text( GetName( Selected ) );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
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
    }

    // ======= LOAD DOUBLE ========

    public abstract class SelectTab<T, S> : SelectTab<T> where T : class where S : class {
        protected S Loaded;

        protected SelectTab( SelectDialog dialog, string name, string stateId, SelectResultType resultType ) : base( dialog, name, stateId, resultType ) { }

        protected override void Select( T item ) {
            LoadItemAsync( item );
            base.Select( item );
        }

        protected override void DrawInner() {
            if( Loaded != null ) {
                using var child = ImRaii.Child( "Child" );
                ImGui.Text( GetName( Selected ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
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
