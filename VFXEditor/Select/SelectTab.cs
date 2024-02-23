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
    public abstract class SelectTab {
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

        protected void DrawPaths( Dictionary<string, Dictionary<string, string>> items, string resultName ) => DrawPaths( items.ToDictionary( x => (x.Key, 0u), x => x.Value ), resultName );

        protected void DrawPaths( Dictionary<(string, uint), Dictionary<string, string>> items, string resultName ) { // With headers and icons
            if( items == null ) return;

            foreach( var ((name, icon), paths) in items ) {
                if( paths.Count == 0 ) continue;

                using var _ = ImRaii.PushId( name );
                if( ImGui.CollapsingHeader( name, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var indent = ImRaii.PushIndent( 10f );
                    DrawIcon( icon );
                    DrawPaths( paths, $"{resultName} {name}" );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                }
            }
        }

        protected void DrawPaths( string label, IEnumerable<string> paths, string resultName ) => DrawPaths( label, paths.Select( x => (x, 0u) ), resultName );

        protected void DrawPaths( string label, IEnumerable<(string, uint)> paths, string resultName ) {
            if( paths == null ) return;

            foreach( var ((path, icon), idx) in paths.WithIndex() ) {
                using var _ = ImRaii.PushId( idx );
                DrawIcon( icon );
                DrawPath( $"{label} #{idx}", path, $"{resultName} #{idx}" );
            }
        }

        protected void DrawPaths( Dictionary<string, string> paths, string resultName ) => DrawPaths( paths.ToDictionary( x => (x.Key, 0u), x => x.Value ), resultName );

        protected void DrawPaths( Dictionary<(string, uint), string> paths, string resultName ) {
            if( paths == null ) return;

            using var _ = ImRaii.PushId( resultName );
            foreach( var ((name, icon), path) in paths ) {
                DrawIcon( icon );
                DrawPath( name, path, $"{resultName} ({name})" );
            }
        }

        protected void DrawPath( string label, string path, string resultName ) => DrawPath( label, path, path, resultName );

        protected void DrawPath( string label, string path, string displayPath, string resultName ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            using var _ = ImRaii.PushId( label );

            DrawFavorite( path, resultName );
            if( string.IsNullOrEmpty( displayPath ) ) {
                ImGui.Text( label );
            }
            else {
                ImGui.Text( $"{label}:" );
                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) {
                    SelectUiUtils.DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                }
                else SelectUiUtils.DisplayPath( path );
            }

            using var indent = ImRaii.PushIndent( 25f );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
            if( ImGui.Button( "SELECT" ) ) Dialog.Invoke( SelectUiUtils.GetSelectResult( path, ResultType, resultName ) );
            ImGui.SameLine();
            SelectUiUtils.Copy( path );

            if( Dialog.CanPlay && ResultType != SelectResultType.Local ) Dialog.PlayButton( path );
        }

        protected void DrawBgmSituation( string name, BgmSituationStruct situation ) {
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
        public List<T> Items = [];
        public bool ItemsLoaded = false;
        public bool WaitingForItems = false;
    }

    public abstract class SelectTab<T> : SelectTab where T : class {
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
