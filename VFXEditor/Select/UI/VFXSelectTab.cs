using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using VFXSelect.Data.Sheets;

namespace VFXSelect.UI {
    public abstract class VFXSelectTab {
        public abstract void Draw();
    }

    public abstract class VFXSelectTab<T, S> : VFXSelectTab {
        public DalamudPluginInterface _pi;
        public VFXSelectDialog _dialog;
        public SheetLoader<T, S> Loader;
        public string Id;

        public string Name;
        public string ParentId;

        public string SearchInput = "";
        public T Selected = default(T);
        public S Loaded = default(S);

        public VFXSelectTab( string parentId, string tabId, SheetLoader<T,S> loader, DalamudPluginInterface pi, VFXSelectDialog dialog ) {
            _pi = pi;
            _dialog = dialog;
            Loader = loader;
            Name = tabId;
            ParentId = parentId;
            Id = "##Select/" + tabId + "/" + parentId;
        }

        public abstract bool CheckMatch( T item, string searchInput);
        public abstract string UniqueRowTitle( T item );
        public abstract void DrawSelected( S loadedItem );
        public virtual void DrawExtra() { }
        public virtual void OnSelect() { }

        public List<T> Searched;
        public override void Draw() {
            var ret = ImGui.BeginTabItem( Name + "##Select/" + ParentId );
            if( !ret )
                return;
            Loader.Load();
            if( !Loader.Loaded ) {
                ImGui.EndTabItem();
                return;
            }
            //
            if( Searched == null ) { Searched = new List<T>(); Searched.AddRange( Loader.Items ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            bool ResetScroll = false;
            DrawExtra();
            if( ImGui.InputText( "Search" + Id, ref SearchInput, 255 ) ) {
                Searched = Loader.Items.Where( x => CheckMatch(x, SearchInput )).ToList();
                ResetScroll = true;
            }
            ImGui.Columns( 2, Id + "Columns", true );
            ImGui.BeginChild( Id + "Tree" );
            VFXSelectDialog.DisplayVisible( Searched.Count, out int preItems, out int showItems, out int postItems, out float itemHeight );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
            if( ResetScroll ) { ImGui.SetScrollHereY(); };
            int idx = 0;
            foreach( var item in Searched ) {
                if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                if( ImGui.Selectable( UniqueRowTitle(item), EqualityComparer<T>.Default.Equals( Selected, item) ) ) {
                    if( !EqualityComparer<T>.Default.Equals( Selected, item ) ) {
                        Task.Run( async () => {
                            bool result = Loader.SelectItem( item, out Loaded );
                        });
                        Selected = item;
                        OnSelect();
                    }
                }
                idx++;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            ImGui.EndChild();
            ImGui.NextColumn();
            // ========================
            if( Selected == null ) {
                ImGui.Text( "Select an item..." );
            }
            else {
                if( Loaded != null ) {
                    ImGui.BeginChild( Id + "Selected" );

                    DrawSelected( Loaded );

                    ImGui.EndChild();
                }
                else {
                    ImGui.Text( "No data found" );
                }
            }
            ImGui.Columns( 1 );
            //
            ImGui.EndTabItem();
        }

        public void LoadIcon( ushort iconId, ref ImGuiScene.TextureWrap texWrap ) {
            texWrap?.Dispose();
            texWrap = null;
            if( iconId > 0 ) {
                var tex = _pi.Data.GetIcon( iconId );
                texWrap = _pi.UiBuilder.LoadImageRaw( BGRA_to_RGBA(tex.ImageData), tex.Header.Width, tex.Header.Height, 4 );
            }
        }

        public static byte[] BGRA_to_RGBA( byte[] data ) {
            byte[] ret = new byte[data.Length];
            for( int i = 0; i < data.Length / 4; i++ ) {
                var idx = i * 4;
                ret[idx + 0] = data[idx + 2];
                ret[idx + 1] = data[idx + 1];
                ret[idx + 2] = data[idx + 0];
                ret[idx + 3] = data[idx + 3];
            }
            return ret;
        }
    }
}