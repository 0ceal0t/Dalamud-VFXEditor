using ImGuiNET;
using Lumina.Data.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using VfxEditor.Utils;
using VfxEditor.Select.Sheets;

namespace VfxEditor {
    public abstract class SelectTab<T> : SelectTab where T : class {
        protected abstract void DrawSelected( string parentId );

        protected virtual void DrawExtra() { }
        protected virtual void OnSelect() { }
        private readonly SheetLoader<T> Loader;

        protected string SearchInput = "";
        protected T Selected = default;

        protected List<T> Searched;

        public SelectTab( string name, SheetLoader<T> loader, SelectDialog dialog ) : base( dialog, name ) {
            Loader = loader;
        }

        protected abstract string GetName( T item );
        protected virtual bool CheckMatch( T item, string searchInput ) => Matches( GetName( item ), searchInput );

        public override void Draw( string parentId ) {
            var id = $"{parentId}/{Name}";

            var ret = ImGui.BeginTabItem( $"{Name}{id}" );
            if( !ret ) return;
            Loader.Load();
            if( !Loader.Loaded ) {
                ImGui.EndTabItem();
                return;
            }

            if( Searched == null ) { Searched = new List<T>(); Searched.AddRange( Loader.Items ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            var ResetScroll = false;
            DrawExtra();
            if( ImGui.InputText( "Search" + id, ref SearchInput, 255 ) ) {
                Searched = Loader.Items.Where( x => CheckMatch( x, SearchInput ) ).ToList();
                ResetScroll = true;
            }

            // Navigate through items using the up and down arrow buttons
            if( KeybindConfiguration.NavigateUpDown( Searched, Selected, out var newSelected ) ) Select( newSelected );

            ImGui.Columns( 2, id + "-Columns", true );
            ImGui.BeginChild( id + "-Tree" );
            DisplayVisible( Searched.Count, out var preItems, out var showItems, out var postItems, out var itemHeight );
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
    }

    public abstract class SelectTab<T,S> : SelectTab<T> where T : class where S : class {
        protected S Loaded;
        private readonly SheetLoader<T, S> Loader;

        protected SelectTab( string name, SheetLoader<T, S> loader, SelectDialog dialog ) : base( name, loader, dialog ) {
            Loader = loader;
        }

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
                Loader.SelectItem( item, out Loaded );
            } );
        }
    }
}