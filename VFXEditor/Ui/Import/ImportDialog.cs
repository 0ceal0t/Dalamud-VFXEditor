using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Select.Base;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Import
{
    public class ImportDialog : DalamudWindow
    {

        public class ImportResult
        {
            public PenumbraMod Mod;
            public List<string> Extensions;
            public ImportResult() { }
        }

        public class PenumbraItem : ISelectItem
        {
            public readonly string Name;

            public PenumbraItem( string name )
            {
                Name = name;
            }

            public string GetName() => Name;
        }

        public ImportDialog() : base( "Import from Penumbra", false, new( 800, 600 ), Plugin.WindowSystem )
        {
        }

        protected readonly List<PenumbraItem> Items = new();
        protected readonly string Name;

        protected Vector2 DefaultWindowPadding = new();
        protected string SearchInput = "";

        protected PenumbraItem SelectedPenumbraMod;
        protected ImportResult Result = new();

        protected Action<ImportResult> Callback;

        public void SetCallback( Action<ImportResult> callback )
        {
            Callback = callback;
        }

        public void OnImport()
        {
            Callback( Result );
            Hide();
        }

        public void Load()
        {
            LoadData();
        }

        public void LoadData()
        {
            Items.Clear();
            Items.AddRange( Plugin.PenumbraIpc.GetMods().Select( x => new PenumbraItem( x ) ) );
        }

        public override void DrawBody()
        {
            Load();
            if( Items.Count == 0 ) return;

            using var _ = ImRaii.PushId( WindowName );

            DefaultWindowPadding = ImGui.GetStyle().WindowPadding;

            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 4, 3 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 4, 3 ) );
            using var child = ImRaii.Child( "Child", new Vector2( -1, -1 ), true );
            using var table = ImRaii.Table( "Table", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit );
            if( !table ) return;
            padding.Dispose();

            ImGui.TableSetupColumn( "##Column1", ImGuiTableColumnFlags.WidthStretch );

            var idx = 0;
            foreach( var item in Items )
            {
                if( !( string.IsNullOrEmpty( SearchInput ) ||
                    item.Name.Contains( SearchInput, System.StringComparison.CurrentCultureIgnoreCase )
                ) ) continue;
                ImGui.TableNextRow();
                DrawRow( item, idx );
                idx++;
            }
        }

        protected bool DrawRow( PenumbraItem item, int idx )
        {
            using var _ = ImRaii.PushId( idx );

            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) )
            using( var color = ImRaii.PushColor( ImGuiCol.Text, UiUtils.PARSED_GREEN ) )
            {
                ImGui.Text( FontAwesomeIcon.Check.ToIconString() );
            }

            ImGui.TableNextColumn();
            ImGui.Selectable( item.Name, false, ImGuiSelectableFlags.SpanAllColumns );

            if( PostRow( item, idx ) ) return true;
            return false;
        }

        protected virtual bool PostRow( PenumbraItem item, int idx )
        {
            if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() )
            {
                SelectedPenumbraMod = item;
                Result = new();
                Result.Extensions = ["avfx", "tmb", "pap", "uld", "sklb", "skp", "eid", "phyb", "atch", "shcd", "shpk", "sgb", "atex"];
                PenumbraMod LoadedPenumbraMod = new();
                PenumbraUtils.LoadFromName( SelectedPenumbraMod.Name, Result.Extensions, out LoadedPenumbraMod );

                Result.Mod = LoadedPenumbraMod;
                OnImport();
                return true;
            }

            return false;
        }
    }
}