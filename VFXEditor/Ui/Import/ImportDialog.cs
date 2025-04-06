using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using VfxEditor.Select.Base;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Import
{
    public class ImportDialog : DalamudWindow
    {
        protected readonly List<string> AllowedTypes = ["avfx", "atex", "tmb", "pap", "scd", "uld", "sklb", "skp", "phyb", "eid", "atch", "kdb", "pbd", "mdl", "mtrl", "shpk", "shcd"];

        protected readonly List<PenumbraItem> Items = new();

        protected readonly string Name;

        protected Action<ImportResult> Callback;

        protected Vector2 DefaultWindowPadding = new();

        protected PenumbraMod LoadedPenumbraMod;
        protected bool Reset = false;
        protected ImportResult Result = new();

        protected string SearchInput = "";

        protected Dictionary<string, bool> SelectedModOptions = [];
        protected PenumbraItem SelectedPenumbraMod;
        protected Dictionary<string, bool> SelectedTypes = [];

        public ImportDialog() : base( "Import from Penumbra", false, new( 800, 600 ), Plugin.WindowSystem )
        {
            foreach( var type in AllowedTypes )
            {
                SelectedTypes.Add( type, false );
            }
            ResetSelectedTypes();
        }

        public override void DrawBody()
        {
            Load(); // mods can change externally
            if( Items.Count == 0 ) return;

            using var _ = ImRaii.PushId( WindowName );

            DefaultWindowPadding = ImGui.GetStyle().WindowPadding;

            ImGui.InputTextWithHint( "##Search", "Search", ref SearchInput, 255 );
            ImGui.Separator();

            if( ImGui.CollapsingHeader( "Extensions" ) )
            {
                DrawExtensionsBasic();
                ImGui.SameLine();
                DrawExtensionPicker();
            }
            if( ImGui.CollapsingHeader( "Mods", ImGuiTreeNodeFlags.DefaultOpen ) )
            {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 0, 0 ) );
                using var table = ImRaii.Table( "ModsTable", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.NoHostExtendY, new( -1, ImGui.GetContentRegionAvail().Y ) );
                if( !table ) return;
                style.Dispose();

                ImGui.TableSetupColumn( "##Left", ImGuiTableColumnFlags.WidthFixed, 200 );
                ImGui.TableSetupColumn( "##Right", ImGuiTableColumnFlags.WidthStretch );

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                using( var tree = ImRaii.Child( "ModsList" ) )
                {
                    DrawModsTable();
                }

                ImGui.TableNextColumn();
                ImGui.Separator();
                if( SelectedPenumbraMod == null ) ImGui.Text( "Select a mod or double click it to import all options..." );
                else
                {
                    DrawModDesc();
                    DrawButtonsDesc();
                }
            }
        }

        public void SetCallback( Action<ImportResult> callback )
        {
            Callback = callback;
        }

        public void SetReset( bool reset )
        {
            Reset = reset;
        }

        private void DrawButtonsDesc()
        {
            ImGui.Separator();
            var text = Reset ? "Import Modpack into Workspace" : "Append Modpack to Workspace";
            if( ImGui.Button( text ) )
            {
                OnImport();
            }
        }

        private void DrawExtensionButtons()
        {
            if( ImGui.Button( "Select All Extensions" ) )
            {
                ExtensionSelectAll();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Unselect All" ) )
            {
                ExtensionSelectNone();
            }
        }

        private void DrawExtensionPicker()
        {
            ImGui.BeginChild( "ExtAdvanced", new Vector2( -1, ImGui.GetContentRegionAvail().Y * .3f ) );
            DrawExtensionButtons();
            if( ImGui.BeginListBox( "##Extensions", new Vector2( -1, -1 ) ) )
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
            ImGui.EndChild();
        }

        private void DrawExtensionsBasic()
        {
            var vfxCheck = SelectedTypes["avfx"] && SelectedTypes["atex"];
            var aniCheck = SelectedTypes["tmb"] && SelectedTypes["pap"];
            var sndCheck = SelectedTypes["scd"];
            var modCheck = SelectedTypes["mdl"] && SelectedTypes["mtrl"];
            var uiPicker = SelectedTypes["uld"];

            ImGui.BeginChild( "ExtBasic", new Vector2( ImGui.GetContentRegionAvail().X * .4f, ImGui.GetContentRegionAvail().Y * .3f ) );
            ImGui.Text( "Quick Picker" );
            if( ImGui.Checkbox( "VFX", ref vfxCheck ) )
            {
                var toggle = vfxCheck;
                SelectedTypes["avfx"] = toggle;
                SelectedTypes["atex"] = toggle;
                RefreshLoadedMod();
            }
            if( ImGui.Checkbox( "Animation", ref aniCheck ) )
            {
                var toggle = aniCheck;
                SelectedTypes["tmb"] = toggle;
                SelectedTypes["pap"] = toggle;
                RefreshLoadedMod();
            }
            if( ImGui.Checkbox( "Sound", ref sndCheck ) )
            {
                var toggle = sndCheck;
                SelectedTypes["scd"] = toggle;
                RefreshLoadedMod();
            }
            if( ImGui.Checkbox( "Model", ref modCheck ) )
            {
                var toggle = modCheck;
                SelectedTypes["mdl"] = toggle;
                SelectedTypes["mtrl"] = toggle;
                RefreshLoadedMod();
            }
            if( ImGui.Checkbox( "UI", ref uiPicker ) )
            {
                var toggle = uiPicker;
                SelectedTypes["uld"] = toggle;
                RefreshLoadedMod();
            }
            ImGui.EndChild();
        }

        private void DrawModBtn()
        {
            if( ImGui.Button( "Select All Options" ) )
            {
                OptionsSelectAll();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Unselect All" ) )
            {
                OptionsSelectNone();
            }
        }

        private void DrawModDesc()
        {
            ImGui.BeginChild( "ModDesc", new Vector2( ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y * .9f ) );
            ImGui.Text( SelectedPenumbraMod.GetName() );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            if( LoadedPenumbraMod.Meta != null )
            {
                ImGui.TextDisabled( $"by {LoadedPenumbraMod.Meta.Author}" );
            }
            DrawModBtn();
            DrawModOptions();
            ImGui.EndChild();
        }

        private void DrawModOptions()
        {
            var idx = 0;
            foreach( var option in LoadedPenumbraMod.SourceFiles )
            {
                var optionName = option.Key;
                var selected = SelectedModOptions[optionName];
                if( ImGui.Checkbox( "##SelectedOption" + idx, ref selected ) )
                {
                    ToggleSelectedOption( optionName );
                }
                ImGui.SameLine();
                if( ImGui.CollapsingHeader( optionName, ImGuiTreeNodeFlags.DefaultOpen ) )
                {
                    if( selected ) DrawModPaths( option.Value );
                    else DrawModPathsDisabled( option.Value );
                }
                idx++;
            }
        }

        private void DrawModPaths( List<string> option )
        {
            foreach( var path in option )
            {
                var split = path.Split( '|' );
                var gamePath = split[0];
                var filePath = split[1];

                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 32 );
                ImGui.Text( gamePath );
                if( ImGui.IsItemHovered() )
                {
                    ImGui.SetTooltip( filePath );
                }
            }
        }

        private void DrawModPathsDisabled( List<string> option )
        {
            foreach( var path in option )
            {
                var split = path.Split( '|' );
                var gamePath = split[0];
                var filePath = split[1];

                ImGui.SetCursorPosX( ImGui.GetCursorPosX() + 32 );
                ImGui.TextDisabled( gamePath );
                if( ImGui.IsItemHovered() )
                {
                    ImGui.SetTooltip( filePath );
                }
            }
        }

        private void DrawModsTable()
        {
            using var style = ImRaii.PushStyle( ImGuiStyleVar.CellPadding, new Vector2( 0, 3 ) );
            using var padding = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 8, 3 ) );
            using var child = ImRaii.Child( "ModsListRow", new Vector2( -1, -1 ), true );
            using var table = ImRaii.Table( "ModsList", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit );
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

        private bool DrawRow( PenumbraItem item, int idx )
        {
            using var _ = ImRaii.PushId( idx );

            ImGui.TableNextColumn();
            if( ImGui.Selectable( item.Name, SelectedPenumbraMod?.Name == item.Name, ImGuiSelectableFlags.SpanAllColumns ) )
            {
                SelectedPenumbraMod = item;
                OnRefresh();
            }

            if( PostRow( item, idx ) ) return true;
            return false;
        }

        private void ExtensionSelectAll()
        {
            foreach( var type in SelectedTypes )
            {
                SelectedTypes[type.Key] = true;
            }
            OnRefresh();
        }

        private void ExtensionSelectNone()
        {
            foreach( var type in SelectedTypes )
            {
                SelectedTypes[type.Key] = false;
            }
            OnRefresh();
        }

        private List<string> GetSelectedTypes()
        {
            var typesList = new List<string>();
            foreach( var type in SelectedTypes )
            {
                if( type.Value ) typesList.Add( type.Key );
            }

            return typesList;
        }

        private void Load()
        {
            LoadData();
        }

        private void LoadData()
        {
            Items.Clear();
            Items.AddRange( Plugin.PenumbraIpc.GetMods().Select( x => new PenumbraItem( x ) ) );
        }

        private void OnImport()
        {
            Result = new();
            Result.Extensions = GetSelectedTypes();
            Result.Reset = Reset;
            Result.Mod = LoadedPenumbraMod;
            var listOptions = new List<string>();
            foreach( var option in SelectedModOptions )
            {
                if( option.Value ) listOptions.Add( option.Key );
            }
            Result.Options = listOptions;
            Callback( Result );
            Hide();
        }

        private void OnRefresh()
        {
            RefreshLoadedMod();
        }

        private void OptionsReset()
        {
            SelectedModOptions = [];
            foreach( var source in LoadedPenumbraMod.SourceFiles )
            {
                SelectedModOptions.Add( source.Key, true );
            }
        }

        private void OptionsSelectAll()
        {
            foreach( var option in SelectedModOptions )
            {
                SelectedModOptions[option.Key] = true;
            }
        }

        private void OptionsSelectNone()
        {
            foreach( var option in SelectedModOptions )
            {
                SelectedModOptions[option.Key] = false;
            }
        }

        private bool PostRow( PenumbraItem item, int idx )
        {
            if( ImGui.IsMouseDoubleClicked( ImGuiMouseButton.Left ) && ImGui.IsItemHovered() )
            {
                OnImport();
                return true;
            }

            return false;
        }

        private void RefreshLoadedMod()
        {
            if( SelectedPenumbraMod == null ) return;
            var sameMod = LoadedPenumbraMod != null && LoadedPenumbraMod.Meta != null && SelectedPenumbraMod.Name == LoadedPenumbraMod.Meta.Name;
            LoadedPenumbraMod = new();
            PenumbraUtils.LoadFromName( SelectedPenumbraMod.Name, GetSelectedTypes(), out LoadedPenumbraMod );
            if( !sameMod )
            {
                OptionsReset();
            }
        }

        private void ResetSelectedTypes()
        {
            SelectedTypes["avfx"] = true;
            SelectedTypes["atex"] = true;
            SelectedTypes["pap"] = true;
            SelectedTypes["tmb"] = true;
            SelectedTypes["scd"] = true;
            SelectedTypes["mdl"] = true;
            SelectedTypes["mtrl"] = true;
            SelectedTypes["uld"] = true;
        }

        private void ToggleSelectedOption( string name )
        {
            SelectedModOptions[name] = !SelectedModOptions[name];
        }

        private void ToggleType( string key )
        {
            SelectedTypes[key] = !SelectedTypes[key];
            OnRefresh();
        }

        public class ImportResult
        {
            public List<string> Extensions;
            public PenumbraMod Mod;
            public List<string> Options;
            public bool Reset;

            public ImportResult()
            { }
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
    }
}