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
    public class VFXStatusSelect
    {
        public Plugin _plugin;
        public VFXSelectDialog _dialog;
        public string ParentId;
        public string TabId;
        public List<XivStatus> Data;
        public string Id;

        public string SearchInput = "";
        public XivStatus SelectedStatus = null;

        public VFXStatusSelect( string parentId, string tabId, List<XivStatus> data, Plugin plugin, VFXSelectDialog dialog )
        {
            _plugin = plugin;
            _dialog = dialog;
            ParentId = parentId;
            TabId = tabId;
            Data = data;
            Id = "##Select/" + tabId + "/" + parentId;
            // =====================
        }

        public List<XivStatus> SearchedStatus;
        public void Draw()
        {
            if( SearchedStatus == null ) { SearchedStatus = new List<XivStatus>(); SearchedStatus.AddRange( Data ); }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            bool ResetScroll = false;
            if( ImGui.InputText( "Search" + Id, ref SearchInput, 255 ) )
            {
                SearchedStatus = Data.Where( x => VFXSelectDialog.Matches( x.Name, SearchInput ) ).ToList();
                ResetScroll = true;
            }
            ImGui.Columns( 2, Id + "Columns", true );
            ImGui.BeginChild( Id + "Tree" );
            VFXSelectDialog.DisplayVisible( SearchedStatus.Count, out int preItems, out int showItems, out int postItems, out float itemHeight );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + preItems * itemHeight );
            if( ResetScroll ) { ImGui.SetScrollHereY(); };
            int idx = 0;
            foreach( var status in SearchedStatus )
            {
                if( idx < preItems || idx > ( preItems + showItems ) ) { idx++; continue; }
                if( ImGui.Selectable( status.Name + "##" + status.RowId, SelectedStatus == status ) )
                {
                    if( status != SelectedStatus )
                    {
                        SelectedStatus = status;
                    }
                }
                idx++;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + postItems * itemHeight );
            ImGui.EndChild();
            ImGui.NextColumn();
            // =======================
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
                _dialog.DisplayPath( SelectedStatus.GetLoopVFX1Path() );
                if( SelectedStatus.LoopVFX1Exists )
                {
                    if( ImGui.Button( "SELECT" + Id + "Loop1" ) )
                    {
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX1Path() ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( SelectedStatus.GetLoopVFX1Path(), id: Id + "Loop1Copy" );
                }
                // ==== LOOP 2 =====
                ImGui.Text( "Loop VFX2: " );
                ImGui.SameLine();
                _dialog.DisplayPath( SelectedStatus.GetLoopVFX2Path() );
                if( SelectedStatus.LoopVFX2Exists )
                {
                    if( ImGui.Button( "SELECT" + Id + "Loop2" ) )
                    {
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX2Path() ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( SelectedStatus.GetLoopVFX2Path(), id: Id + "Loop2Copy" );
                }
                // ==== LOOP 3 =====
                ImGui.Text( "Loop VFX3: " );
                ImGui.SameLine();
                _dialog.DisplayPath( SelectedStatus.GetLoopVFX3Path() );
                if( SelectedStatus.LoopVFX3Exists )
                {
                    if( ImGui.Button( "SELECT" + Id + "Loop3" ) )
                    {
                        _dialog.Invoke( new VFXSelectResult( VFXSelectType.GameStatus, "[STATUS] " + SelectedStatus.Name, SelectedStatus.GetLoopVFX3Path() ) );
                    }
                    ImGui.SameLine();
                    _dialog.Copy( SelectedStatus.GetLoopVFX3Path(), id: Id + "Loop3Copy" );
                }
            }
            ImGui.Columns( 1 );
        }
    }
}