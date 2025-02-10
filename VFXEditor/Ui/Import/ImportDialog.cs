using Dalamud.Interface;
using Dalamud.Interface.FontIdentifier;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;
using VfxEditor.FileManager.Interfaces;
using VfxEditor.Select;
using VfxEditor.Select.Base;
using VfxEditor.Ui.Components;
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
            foreach( var type in AllowedTypes )
            {
                SelectedTypes.Add( type, true );
            }
        }

        protected readonly List<string> AllowedTypes = ["avfx", "atex", "tmb", "pap", "scd", "uld", "sklb", "skp", "phyb", "eid", "atch", "kdb", "pbd", "mdl", "mtrl", "shpk", "shcd"];

        protected readonly List<PenumbraItem> Items = new();
        protected readonly string Name;

        protected Vector2 DefaultWindowPadding = new();
        protected string SearchInput = "";

        protected Dictionary<string, bool> SelectedTypes = new Dictionary<string, bool>();
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

        public void ToggleType( string key )
        {
            SelectedTypes[key] = !SelectedTypes[key];
        }

        public override void DrawBody()
        {
            Load(); // mods can change externally
            if( Items.Count == 0 ) return;

            using var _ = ImRaii.PushId( WindowName );

            DefaultWindowPadding = ImGui.GetStyle().WindowPadding;

            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );

            if( ImGui.CollapsingHeader( "Extensions", ImGuiTreeNodeFlags.CollapsingHeader ) )
            {
                ExtensionPicker();
            }
            if( ImGui.CollapsingHeader( "Mods", ImGuiTreeNodeFlags.DefaultOpen ) )
            {
                ModsTable();
            }
        }

        private void ModsTable()
        {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 8, 3 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 8, 3 ) );
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

        private void ExtensionPicker()
        {
            ExtensionButtons();
            if( ImGui.BeginListBox( "##Extensions" ) )
            {
                foreach( var type in SelectedTypes )
                {
                    if( ImGui.Selectable( type.Key, type.Value, ImGuiSelectableFlags.None ) )
                    {
                        ToggleType( type.Key );
                    }
                }
                ImGui.EndListBox();
            }
        }

        private void ExtensionButtons()
        {
            if( ImGui.Button( "Select All" ) )
            {
                SelAll();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Unselect All" ) )
            {
                UnselAll();
            }
        }

        private void SelAll()
        {
            foreach( var type in SelectedTypes )
            {
                SelectedTypes[type.Key] = true;
            }
        }

        private void UnselAll()
        {
            foreach( var type in SelectedTypes )
            {
                SelectedTypes[type.Key] = false;
            }
        }

        protected bool DrawRow( PenumbraItem item, int idx )
        {
            using var _ = ImRaii.PushId( idx );

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
                var typesList = new List<string>();
                foreach( var type in SelectedTypes )
                {
                    if(type.Value) typesList.Add( type.Key );
                }

                Result = new();
                Result.Extensions = typesList;
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