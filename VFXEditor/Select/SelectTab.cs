using Dalamud.Logging;
using ImGuiNET;
using ImGuiScene;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VfxEditor.Select.Scd.BgmQuest;
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

        protected void DrawPapsWithHeader( Dictionary<string, Dictionary<string, string>> items, string name ) {
            foreach( var (subName, subItems) in items ) {
                if( subItems.Count == 0 ) continue;

                using var _ = ImRaii.PushId( subName );

                if( ImGui.CollapsingHeader( subName, ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var indent = ImRaii.PushIndent( 10f );
                    DrawPaps( subItems, $"{name} {subName}" );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                }
            }
        }

        protected void DrawPaps( Dictionary<string, string> items, string name ) {
            using var _ = ImRaii.PushId( name );

            foreach( var (suffix, path) in items ) {
                using var __ = ImRaii.PushId( suffix );

                DrawFavorite( path, $"{name} ({suffix})" );

                ImGui.Text( $"{suffix}:" );

                ImGui.SameLine();
                if( path.Contains( "action.pap" ) || path.Contains( "face.pap" ) ) {
                    SelectUiUtils.DisplayPathWarning( path, "Be careful about modifying this file, as it contains dozens of animations for every job" );
                }
                else SelectUiUtils.DisplayPath( path );

                DrawPath( "", path, $"{name} ({suffix})" );
            }
        }

        protected void DrawPaths( string label, IEnumerable<string> paths, string resultName, bool play = false ) {
            var idx = 0;
            foreach( var path in paths ) {
                using var _ = ImRaii.PushId( idx );
                DrawPath( $"{label} #{idx}", path, $"{resultName} #{idx}", play );
                idx++;
            }
        }

        protected void DrawPaths( Dictionary<string, string> paths, string resultName, bool play = false ) {
            foreach( var item in paths ) {
                DrawPath( item.Key, item.Value, $"{resultName} ({item.Key})", play );
            }
        }

        protected void DrawPath( string label, string path, string resultName, bool play = false ) =>
            DrawPath( label, path, path, resultName, play );

        protected void DrawPath( string label, string path, string displayPath, string resultName, bool play = false ) {
            if( string.IsNullOrEmpty( path ) ) return;
            if( path.Contains( "BGM_Null" ) ) return;

            using var _ = ImRaii.PushId( label );

            if( !string.IsNullOrEmpty( label ) ) { // if this is blank, assume there is some custom logic to draw the path
                DrawFavorite( path, resultName );
                ImGui.Text( $"{label}:" );
                ImGui.SameLine();
                SelectUiUtils.DisplayPath( displayPath );
            }

            using var indent = ImRaii.PushIndent( 25f );

            if( ImGui.Button( "SELECT" ) ) Dialog.Invoke( SelectUiUtils.GetSelectResult( path, ResultType, resultName ) );
            ImGui.SameLine();
            SelectUiUtils.Copy( path );
            if( play ) Dialog.Play( path );
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
        protected TextureWrap Icon; // Not used by every tab

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

        // Drawing
        protected abstract string GetName( T item );

        protected virtual bool CheckMatch( T item, string searchInput ) => SelectUiUtils.Matches( GetName( item ), searchInput );

        protected abstract void DrawSelected();

        protected virtual void DrawExtra() { }

        protected virtual void OnSelect() { }

        public override void Draw() {
            using var _ = ImRaii.PushId( Name );

            using var tabItem = ImRaii.TabItem( Name );
            if( !tabItem ) return;

            Load();

            if( !ItemsLoaded ) return;

            if( Searched == null ) { Searched = new List<T>(); Searched.AddRange( Items ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            var resetScroll = false;
            DrawExtra();
            if( ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 ) ) {
                Searched = Items.Where( x => CheckMatch( x, SearchInput ) ).ToList();
                resetScroll = true;
            }

            ImGui.Separator();

            // Navigate through items using the up and down arrow buttons
            if( KeybindConfiguration.NavigateUpDown( Searched, Selected, out var newSelected ) ) Select( newSelected );

            ImGui.Columns( 2, "Columns", true );

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

            ImGui.NextColumn();

            if( Selected == null ) ImGui.Text( "Select an item..." );
            else DrawInner();

            ImGui.Columns( 1 );
        }

        protected virtual void DrawInner() {
            using var child = ImRaii.Child( "Child" );

            ImGui.Text( GetName( Selected ) );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawSelected();
        }

        protected virtual void Select( T item ) {
            Selected = item;
            OnSelect();
        }

        protected void LoadIcon( uint iconId ) {
            Icon?.Dispose();
            Icon = null;
            if( iconId <= 0 ) return;
            try {
                Icon = Plugin.TextureProvider.GetIcon( iconId, IconFlags.None );
            }
            catch( Exception ) {
                Icon = Plugin.TextureProvider.GetIcon( 0, IconFlags.None );
            }
        }

        // Loading

        public virtual async void Load() {
            if( WaitingForItems || ItemsLoaded ) return;
            State.WaitingForItems = true;
            PluginLog.Log( "Loading " + StateId );

            await Task.Run( () => {
                try {
                    LoadData();
                }
                catch( Exception e ) {
                    PluginLog.Error( e, "Error Loading: " + StateId );
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
