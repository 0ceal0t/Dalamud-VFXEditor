using Dalamud.Logging;
using ImGuiNET;
using ImGuiScene;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.Select {
    // ==== BASE =======

    public abstract class SelectTab {
        protected readonly SelectDialog Dialog;
        protected readonly string Name;

        public SelectTab( SelectDialog dialog, string name ) {
            Dialog = dialog;
            Name = name;
        }

        public abstract void Draw( string parentId );
    }

    public class SelectTabState<T> {
        public List<T> Items = new();
        public bool ItemsLoaded = false;
        public bool WaitingForItems = false;
    }

    // ===== LOAD SINGLE ========

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

        // Drawing
        protected abstract string GetName( T item );
        protected virtual bool CheckMatch( T item, string searchInput ) => SelectTabUtils.Matches( GetName( item ), searchInput );

        protected abstract void DrawSelected( string parentId );
        protected virtual void DrawExtra() { }
        protected virtual void OnSelect() { }

        public override void Draw( string parentId ) {
            var id = $"{parentId}/{Name}";

            if( !ImGui.BeginTabItem( $"{Name}{id}" ) ) return;
            Load();
            if( !ItemsLoaded ) {
                ImGui.EndTabItem();
                return;
            }

            if( Searched == null ) { Searched = new List<T>(); Searched.AddRange( Items ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var ResetScroll = false;
            DrawExtra();
            if( ImGui.InputText( "Search" + id, ref SearchInput, 255 ) ) {
                Searched = Items.Where( x => CheckMatch( x, SearchInput ) ).ToList();
                ResetScroll = true;
            }

            ImGui.Separator();

            // Navigate through items using the up and down arrow buttons
            if( KeybindConfiguration.NavigateUpDown( Searched, Selected, out var newSelected ) ) Select( newSelected );

            ImGui.Columns( 2, id + "-Columns", true );
            ImGui.BeginChild( id + "-Tree" );
            SelectTabUtils.DisplayVisible( Searched.Count, out var preItems, out var showItems, out var postItems, out var itemHeight );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
            if( ResetScroll ) { ImGui.SetScrollHereY(); };

            var idx = 0;
            foreach( var item in Searched ) {
                if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                if( ImGui.Selectable( $"{GetName( item )}{id}{idx}", Selected == item ) ) {
                    if( Selected != item ) Select( item ); // not what is currently selected
                }
                idx++;
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            ImGui.EndChild();
            ImGui.NextColumn();

            if( Selected == null ) ImGui.Text( "Select an item..." );
            else {
                DrawInner( id );
            }
            ImGui.Columns( 1 );
            ImGui.EndTabItem();
        }

        protected virtual void DrawInner( string id ) {
            ImGui.BeginChild( id + "-Selected" );
            ImGui.Text( GetName( Selected ) );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawSelected( id );
            ImGui.EndChild();
        }

        protected virtual void Select( T item ) {
            Selected = item;
            OnSelect();
        }

        protected void LoadIcon( uint iconId ) {
            Icon?.Dispose();
            Icon = null;
            if( iconId > 0 ) {
                TexFile tex;
                try { tex = Plugin.DataManager.GetIcon( iconId ); }
                catch( Exception ) { tex = Plugin.DataManager.GetIcon( 0 ); }
                Icon = Plugin.PluginInterface.UiBuilder.LoadImageRaw( SelectTabUtils.BgraToRgba( tex.ImageData ), tex.Header.Width, tex.Header.Height, 4 );
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

        protected SelectTab( SelectDialog dialog, string name, string stateId ) : base( dialog, name, stateId ) { }

        protected override void Select( T item ) {
            LoadItemAsync( item );
            base.Select( item );
        }

        protected override void DrawInner( string id ) {
            if( Loaded != null ) {
                ImGui.BeginChild( id + "-Selected" );
                ImGui.Text( GetName( Selected ) );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                DrawSelected( id );
                ImGui.EndChild();
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
