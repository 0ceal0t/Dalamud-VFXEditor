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
        GameItem,
        GameStatus,
        GameAction
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
        public bool ShowGameStatus = true;
        public bool ShowGameActions = true;

        public string Id;
        public event Action<VFXSelectResult> OnSelect;

        public bool Visible = false;

        public VFXSelectDialog(Plugin plugin, string id)
        {
            _plugin = plugin;
            Id = id;
            // ===========================
        }

        public void Show(bool showLocal = true, bool showGamePath = true, bool showGameItems = true, bool showGameStatus = true, bool ShowGameActions = true)
        {
            ShowLocal = showLocal;
            ShowGamePath = showGamePath;
            ShowGameItems = showGameItems;
            ShowGameStatus = showGameStatus;
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
                ImGui.SetNextWindowSize( new Vector2( 800, 500 ) );
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
            if( ShowGameStatus )
                DrawGameStatus();
            if( ShowGameActions )
                DrawGameActions();
            ImGui.EndTabBar();

            ImGui.End();
        }

        // =========== LOCAL ================
        private string localPathInput = "";
        public void DrawLocal()
        {
            var ret = ImGui.BeginTabItem( "Local File##Select-" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( ".avfx file located on your computer, eg: C:/Users/me/Downloads/awesome.avfx" );
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

        // ============== GAME FILE ================
        private string gamePathInput = "";
        public void DrawGamePath()
        {
            var ret = ImGui.BeginTabItem( "Game File##Select/" + Id );
            if( !ret )
                return;
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.Text( "In-game .avfx file, eg: vfx/common/eff/wp_astro1h.avfx" );
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

        // =========== GAME ITEM =============
        public string gameItemsSearchInput = "";
        public XivItem SelectedItem = null;
        public XivSelectedItem LoadedItem = null;
        public void DrawGameItems()
        {
            var ret = ImGui.BeginTabItem( "Game Item##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadItems();
            if( !_plugin.Manager.ItemsLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
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

                    ImGui.Text( "IMC Path: " );
                    ImGui.SameLine();
                    DisplayPath( LoadedItem.ImcPath );

                    ImGui.Text( "VFX Path: " );
                    ImGui.SameLine();
                    DisplayPath( LoadedItem.GetVFXPath() );
                    if( LoadedItem.VfxExists )
                    {
                        if( ImGui.Button( "SELECT##Select-GameItem/" + Id ) )
                        {
                            OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameItem, "[ITEM] " + LoadedItem.Item.Name, LoadedItem.GetVFXPath() ) );
                        }
                        ImGui.SameLine();
                        Copy( LoadedItem.GetVFXPath(), id:"Select-GameItem/Copy/" + Id );
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

        // =========== GAME STATUS ================
        public string gameStatussSearchInput = "";
        public XivStatus SelectedStatus = null;
        public void DrawGameStatus()
        {
            var ret = ImGui.BeginTabItem( "Game Status##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadStatus();
            if( !_plugin.Manager.StatusLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search##Select/GameStatusSearch/" + Id, ref gameStatussSearchInput, 255 );
            ImGui.Columns( 2, "##Select/GameStatusColumns/" + Id, true );

            //
            ImGui.BeginChild( "##Select/GameStatussTree/" + Id );
            foreach( var status in _plugin.Manager.Status )
            {
                if( !status.Name.Contains( gameStatussSearchInput ) )
                    continue;

                if( ImGui.Selectable( status.Name + "##" + status.RowId, SelectedStatus == status ) )
                {
                    if( status != SelectedStatus )
                    {
                        SelectedStatus = status;
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if( SelectedStatus == null )
            {
                ImGui.Text( "Select a status..." );
            }
            else
            {
                ImGui.Text( SelectedStatus.Name );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                // ==== LOOP 1 =====
                ImGui.Text( "Loop VFX1: " );
                ImGui.SameLine();
                DisplayPath(SelectedStatus.GetLoopVFX1Path() );
                if( SelectedStatus.LoopVFX1Exists )
                {
                    if( ImGui.Button( "SELECT##Select-GameStatus-Loop1/" + Id ) )
                    {
                        OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX1Path() ) );
                    }
                    ImGui.SameLine();
                    Copy( SelectedStatus.GetLoopVFX1Path(), id: "Select-GameStatus-Loop1/Copy/" + Id );
                }
                // ==== LOOP 2 =====
                ImGui.Text( "Loop VFX2: " );
                ImGui.SameLine();
                DisplayPath( SelectedStatus.GetLoopVFX2Path() );
                if( SelectedStatus.LoopVFX2Exists )
                {
                    if( ImGui.Button( "SELECT##Select-GameStatus-Loop2/" + Id ) )
                    {
                        OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX2Path() ) );
                    }
                    ImGui.SameLine();
                    Copy( SelectedStatus.GetLoopVFX2Path(), id: "Select-GameStatus-Loop2/Copy/" + Id );
                }
                // ==== LOOP 3 =====
                ImGui.Text( "Loop VFX3: " );
                ImGui.SameLine();
                DisplayPath( SelectedStatus.GetLoopVFX3Path() );
                if( SelectedStatus.LoopVFX3Exists )
                {
                    if( ImGui.Button( "SELECT##Select-GameStatus-Loop3/" + Id ) )
                    {
                        OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX3Path() ) );
                    }
                    ImGui.SameLine();
                    Copy( SelectedStatus.GetLoopVFX3Path(), id: "Select-GameStatus-Loop3/Copy/" + Id );
                }
            }
            ImGui.Columns( 1 );

            ImGui.EndTabItem();
        }

        // =========== GAME ACTIONS =============
        public string gameActionsSearchInput = "";
        public string gameActionVfxPath = "";
        public string gameActionImcPath = "";
        public XivAction SelectedAction = null;
        public XivSelectedAction LoadedAction = null;
        public void DrawGameActions()
        {
            var ret = ImGui.BeginTabItem( "Game Action##Select/" + Id );
            if( !ret )
                return;
            _plugin.Manager.LoadActions();
            if( !_plugin.Manager.ActionsLoaded )
            {
                ImGui.EndTabItem();
                return;
            }
            // ==========================
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search##Select/GameActionSearch/" + Id, ref gameActionsSearchInput, 255 );
            ImGui.Columns( 2, "##Select/GameActionColumns/" + Id, true );

            //
            ImGui.BeginChild( "##Select/GameActionsTree/" + Id );
            foreach( var action in _plugin.Manager.Actions )
            {
                if( !action.Name.Contains( gameActionsSearchInput ) )
                    continue;

                if( ImGui.Selectable( action.Name + "##" + action.RowId, SelectedAction == action ) )
                {
                    if( action != SelectedAction )
                    {
                        bool result = _plugin.Manager.SelectAction( action, out LoadedAction );
                        SelectedAction = action;
                    }
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if( SelectedAction == null )
            {
                ImGui.Text( "Select an action..." );
            }
            else
            {
                if( LoadedAction != null )
                {
                    ImGui.Text( LoadedAction.Action.Name );
                    ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

                    ImGui.Text( "Cast VFX Path: " );
                    ImGui.SameLine();
                    DisplayPath( LoadedAction.CastVfxPath);
                    if( LoadedAction.CastVfxExists )
                    {
                        if( ImGui.Button( "SELECT##Select-GameAction-Cast/" + Id ) )
                        {
                            OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + LoadedAction.Action.Name, LoadedAction.CastVfxPath ) );
                        }
                        ImGui.SameLine();
                        Copy( LoadedAction.CastVfxPath, id: "Select-GameAction-Cast/Copy/" + Id );
                    }

                    if( LoadedAction.SelfVfxExists )
                    {
                        ImGui.Text( "TMB Path: " );
                        ImGui.SameLine();
                        DisplayPath( LoadedAction.SelfTmbPath );
                        int idx = 0;
                        foreach(var _vfx in LoadedAction.SelfVfxPaths )
                        {
                            ImGui.Text( "VFX #" + idx + ": " );
                            ImGui.SameLine();
                            DisplayPath( _vfx );
                            if( ImGui.Button( "SELECT##Select-GameAction-" + idx + "/" + Id ) )
                            {
                                OnSelect?.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + LoadedAction.Action.Name, _vfx ) );
                            }
                            ImGui.SameLine();
                            Copy( _vfx, id: "Select-GameAction-" + idx + "/Copy/" + Id );
                            idx++;
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

        // ======== UTIL ==========
        public void DisplayPath(string path )
        {
            ImGui.PushStyleColor( ImGuiCol.Text, new Vector4( 0.8f, 0.8f, 0.8f, 1 ) );
            ImGui.TextWrapped( path );
            ImGui.PopStyleColor();
        }
        public void Copy(string copyPath, string id = "" )
        {
            ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.15f, 0.15f, 0.15f, 1 ) );
            if(ImGui.Button("Copy##" + id ) )
            {
                ImGui.SetClipboardText( copyPath );
            }
            ImGui.PopStyleColor();
        }
    }
}
