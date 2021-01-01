using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Interface;
using Dalamud.Plugin;
using ImGuiNET;

namespace VFXEditor.UI
{
    public enum VFXSelectType
    {
        Local,
        GamePath,
        GameItem
    }
    public struct VFXSelectResult
    {
        public VFXSelectType Type;
        public string DisplayString;
        public string Path;

        public VFXSelectResult(VFXSelectType type, string displayString, string path)
        {
            Type = type;
            DisplayString = displayString;
            Path = path;
        }
    }

    public class VFXSelectDialog
    {
        public Plugin _plugin;
        public bool ShowLocal = true;
        public bool ShowGamePath = true;
        public bool ShowGameItems = true;
        public string Id;
        public XivSelectedItem LoadedItem = null;
        public event Action<VFXSelectResult> OnSelect;

        public bool Visible = false;

        public VFXSelectDialog(Plugin plugin, string id)
        {
            _plugin = plugin;
            Id = id;
            // ===========================
        }

        public void Show(bool showLocal = true, bool showGamePath = true, bool showGameItems = true)
        {
            ShowLocal = showLocal;
            ShowGamePath = showGamePath;
            ShowGameItems = showGameItems;
            // ======================
            Visible = true;
        }

        public bool DrawOnce = false;
        public void Draw()
        {
            if( !Visible )
                return;
            if( !DrawOnce )
            {
                ImGui.SetNextWindowSize( new Vector2( 600, 500 ) );
                DrawOnce = true;
            }
            // ================
            var ret = ImGui.Begin("File Select##" + Id, ref Visible );
            if( !ret )
                return;

            ImGui.BeginTabBar( "VFXSelectDialogTabs##" + Id );
            if( ShowLocal )
                DrawLocal();
            if( ShowGamePath )
                DrawGamePath();
            if( ShowGameItems )
                DrawGameItems();
            ImGui.EndTabBar();

            ImGui.End();
        }

        private string localPathInput = "";
        public void DrawLocal()
        {
            var ret = ImGui.BeginTabItem( "Local File##Select-" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Select/Local/" + Id, ref localPathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( ( "Browse##Select/Local/" + Id ) ) )
            {
                Task.Run( async () =>
                {
                    var picker = new OpenFileDialog
                    {
                        Filter = "AVFX File (*.avfx)|*.avfx*|All files (*.*)|*.*",
                        CheckFileExists = true,
                        Title = "Select AVFX File."
                    };
                    var result = await picker.ShowDialogAsync();
                    if( result == DialogResult.OK )
                    {
                        try
                        {
                            localPathInput = picker.FileName;
                        }
                        catch( Exception ex )
                        {
                            PluginLog.LogError( ex, "Could not select the local file." );
                        }
                    }
                } );
            }
            ImGui.SameLine();
            if( ImGui.Button( "SELECT##Select/Local/" + Id ) )
            {
                OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.Local, "[LOCAL] " + localPathInput, localPathInput ) );
            }

            ImGui.EndTabItem();
        }

        private string gamePathInput = "";
        public void DrawGamePath()
        {
            var ret = ImGui.BeginTabItem( "Game File##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "Path" );
            ImGui.SameLine();
            ImGui.InputText( "##Select/GamePath/" + Id, ref gamePathInput, 255 );
            ImGui.SameLine();
            if( ImGui.Button( "SELECT##Select/GamePath/" + Id ) )
            {
                OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GamePath, "[GAME] " + gamePathInput, gamePathInput ) );
            }

            ImGui.EndTabItem();
        }

        public string gameItemsSearchInput = "";
        public string gameItemVfxPath = "";
        public string gameItemImcPath = "";
        public XivItem SelectedItem = null;
        public void DrawGameItems()
        {
            var ret = ImGui.BeginTabItem( "Game Item##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search##Select/GameItemSearch/" + Id, ref gameItemsSearchInput, 255 );
            ImGui.Columns( 2, "##Select/GameItemColumns/" + Id, true );

            //
            ImGui.BeginChild("##Select/GameItemsTree/" + Id);
            foreach(var item in _plugin.Manager.Items )
            {
                if( !item.Name.Contains( gameItemsSearchInput ) )
                    continue;

                if(ImGui.Selectable(item.Name, SelectedItem == item ) )
                {
                    if(item != SelectedItem )
                    {
                        bool result =_plugin.Manager.SelectItem( item, out LoadedItem);
                        if( result )
                        {
                            gameItemVfxPath = LoadedItem.GetVFXPath();
                            gameItemImcPath = LoadedItem.ImcPath;
                        }
                        SelectedItem = item;
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if( SelectedItem == null ) {
                ImGui.Text( "Select an item..." );
            }
            else
            {
                if(LoadedItem != null )
                {
                    ImGui.Text( LoadedItem.Item.Name );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
                    ImGui.Text( "Variant: " + LoadedItem.Item.Variant);
                    ImGui.Text( "IMC Count: " + LoadedItem.Count );
                    ImGui.Text( "VFX Id: " + LoadedItem.VfxId );

                    ImGui.InputText( "IMC Path##Select/GameItems/" + Id, ref gameItemImcPath, 255, ImGuiInputTextFlags.ReadOnly );
                    ImGui.InputText( "VFX Path##Select/GameItems/" + Id, ref gameItemVfxPath, 255, ImGuiInputTextFlags.ReadOnly);
                    if( LoadedItem.VfxExists )
                    {
                        if( ImGui.Button( "SELECT##Select-GameItem/" + Id ) )
                        {
                            OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameItem, "[ITEM] " + LoadedItem.Item.Name, gameItemVfxPath ) );
                        }
                    }
                }
                else
                {
                    ImGui.Text( "No data found" );
                }
            }
            ImGui.Columns( 1 );

            ImGui.EndTabItem();
        }
    }
}
