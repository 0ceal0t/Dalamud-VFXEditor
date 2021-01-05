using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ImGuiNET;

namespace VFXEditor.UI
{
    public class VFXActionSelect
    {
        public Plugin _plugin;
        public VFXSelectDialog _dialog;
        public string ParentId;
        public string TabId;
        public List<XivActionBase> Data;
        public string Id;

        public string SearchInput = "";
        public XivActionBase SelectedAction = null;
        public XivSelectedAction LoadedAction = null;

        public VFXActionSelect(string parentId, string tabId, List<XivActionBase> data, Plugin plugin, VFXSelectDialog dialog)
        {
            _plugin = plugin;
            _dialog = dialog;
            ParentId = parentId;
            TabId = tabId;
            Data = data;
            Id = "##Select/" + tabId + "/" + parentId;
            // =====================
        }

        public void Draw()
        {
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            ImGui.InputText( "Search" + Id, ref SearchInput, 255 );
            ImGui.Columns( 2, Id + "Columns", true );

            ImGui.BeginChild( Id + "Tree" );
            foreach( var action in Data )
            {
                if( !VFXSelectDialog.Matches(action.Name, SearchInput) )
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
                    _dialog.DisplayPath( LoadedAction.CastVfxPath );
                    if( LoadedAction.CastVfxExists )
                    {
                        if( ImGui.Button( "SELECT" + Id + "Cast" ) )
                        {
                            _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + LoadedAction.Action.Name, LoadedAction.CastVfxPath ) );
                        }
                        ImGui.SameLine();
                        _dialog.Copy( LoadedAction.CastVfxPath, id: Id + "CastCopy" );
                    }

                    if( LoadedAction.SelfVfxExists )
                    {
                        ImGui.Text( "TMB Path: " );
                        ImGui.SameLine();
                        _dialog.DisplayPath( LoadedAction.SelfTmbPath );
                        int idx = 0;
                        foreach( var _vfx in LoadedAction.SelfVfxPaths )
                        {
                            ImGui.Text( "VFX #" + idx + ": " );
                            ImGui.SameLine();
                            _dialog.DisplayPath( _vfx );
                            if( ImGui.Button( "SELECT" + Id + idx ) )
                            {
                                _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameAction, "[ACTION] " + LoadedAction.Action.Name, _vfx ) );
                            }
                            ImGui.SameLine();
                            _dialog.Copy( _vfx, id: Id + "Copy" + idx );
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
        }
    }
}
