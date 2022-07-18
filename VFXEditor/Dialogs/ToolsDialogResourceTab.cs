using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.STD;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VFXEditor.Helper;

namespace VFXEditor.Dialogs {
    public unsafe class ToolsDialogResourceTab {
        private struct ResourceItemStruct {
            public uint Hash;
            public ulong Address;
            public string Path;
            public uint Refs;
        }

        private string Search = "";

        // Adapted from https://github.com/xivdev/Penumbra/blob/7e7e74a5346857328ee161d571c1f1ead6524e9a/Penumbra/UI/MenuTabs/TabResourceManager.cs
        public void Draw() {
            var resourceHandler = *( ResourceManager** )Plugin.SigScanner.GetStaticAddressFromSig( "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 32 C0" );

            if( resourceHandler == null ) return;

            ImGui.InputText( "Search##ResourceManager", ref Search, 256 );

            if( !ImGui.BeginChild( "##ResourceManager", -Vector2.One, true ) ) return;

            DrawCategoryContainer( ResourceCategory.Common, resourceHandler->ResourceGraph->CommonContainer );
            DrawCategoryContainer( ResourceCategory.BgCommon, resourceHandler->ResourceGraph->BgCommonContainer );
            DrawCategoryContainer( ResourceCategory.Bg, resourceHandler->ResourceGraph->BgContainer );
            DrawCategoryContainer( ResourceCategory.Cut, resourceHandler->ResourceGraph->CutContainer );
            DrawCategoryContainer( ResourceCategory.Chara, resourceHandler->ResourceGraph->CharaContainer );
            DrawCategoryContainer( ResourceCategory.Shader, resourceHandler->ResourceGraph->ShaderContainer );
            DrawCategoryContainer( ResourceCategory.Ui, resourceHandler->ResourceGraph->UiContainer );
            DrawCategoryContainer( ResourceCategory.Sound, resourceHandler->ResourceGraph->SoundContainer );
            DrawCategoryContainer( ResourceCategory.Vfx, resourceHandler->ResourceGraph->VfxContainer );
            DrawCategoryContainer( ResourceCategory.UiScript, resourceHandler->ResourceGraph->UiScriptContainer );
            DrawCategoryContainer( ResourceCategory.Exd, resourceHandler->ResourceGraph->ExdContainer );
            DrawCategoryContainer( ResourceCategory.GameScript, resourceHandler->ResourceGraph->GameScriptContainer );
            DrawCategoryContainer( ResourceCategory.Music, resourceHandler->ResourceGraph->MusicContainer );
            DrawCategoryContainer( ResourceCategory.SqpackTest, resourceHandler->ResourceGraph->SqpackTestContainer );
            DrawCategoryContainer( ResourceCategory.Debug, resourceHandler->ResourceGraph->DebugContainer );

            ImGui.EndChild();
        }

        private void DrawCategoryContainer( ResourceCategory category, ResourceGraph.CategoryContainer container ) {
            var map = container.MainMap;
            if( map == null || !ImGui.TreeNodeEx( $"({( uint )category:D2}) {category} - {map->Count}###{( uint )category}Debug" ) ) {
                return;
            }

            var node = map->SmallestValue;
            while( !node->IsNil ) {
                DrawResourceMap( GetNodeLabel( ( uint )category, node->KeyValuePair.Item1, node->KeyValuePair.Item2.Value->Count ),
                    node->KeyValuePair.Item2.Value );
                node = node->Next();
            }

            ImGui.TreePop();
        }

        private unsafe void DrawResourceMap( string label, StdMap<uint, Pointer<ResourceHandle>>* typeMap ) {
            if( typeMap == null ) return;

            var itemList = new List<ResourceItemStruct>();
            if( typeMap->Count > 0 ) {
                var node = typeMap->SmallestValue;
                while( !node->IsNil ) {
                    var path = node->KeyValuePair.Item2.Value->FileName.ToString();

                    if( string.IsNullOrEmpty( Search ) || path.Contains( Search ) ) { // match against search
                        itemList.Add( new ResourceItemStruct {
                            Hash = node->KeyValuePair.Item1,
                            Address = ( ulong )node->KeyValuePair.Item2.Value,
                            Path = path,
                            Refs = node->KeyValuePair.Item2.Value->RefCount
                        } );
                    }
                    node = node->Next();
                }
            }

            if( itemList.Count == 0 && !string.IsNullOrEmpty( Search ) ) return; // all filtered out

            if( !ImGui.TreeNodeEx( label ) ) {
                return;
            }

            if( itemList.Count == 0 || !ImGui.BeginTable( $"##{label}_table", 4, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg ) ) {
                ImGui.TreePop();
                return;
            }

            ImGui.TableSetupColumn( "Hash", ImGuiTableColumnFlags.WidthFixed, 100 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Ptr", ImGuiTableColumnFlags.WidthFixed, 100 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Path", ImGuiTableColumnFlags.WidthFixed,
                UIHelper.GetWindowContentRegionWidth() - 300 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Refs", ImGuiTableColumnFlags.WidthFixed, 30 * ImGuiHelpers.GlobalScale );
            ImGui.TableHeadersRow();

            foreach( var item in itemList ) {

                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text( $"0x{item.Hash:X8}" );
                ImGui.TableNextColumn();
                var address = $"0x{item.Address:X}";
                ImGui.Text( address );
                if( ImGui.IsItemClicked() ) {
                    ImGui.SetClipboardText( address );
                }

                ImGui.TableNextColumn();
                ImGui.Text( item.Path );
                ImGui.TableNextColumn();
                ImGui.Text( item.Refs.ToString() );
            }

            ImGui.EndTable();
            ImGui.TreePop();
        }

        private static string GetNodeLabel( uint label, uint type, ulong count ) {
            var byte1 = type >> 24;
            var byte2 = ( type >> 16 ) & 0xFF;
            var byte3 = ( type >> 8 ) & 0xFF;
            var byte4 = type & 0xFF;
            return byte1 == 0
                ? $"({type:X8}) {( char )byte2}{( char )byte3}{( char )byte4} - {count}###{label}{type}Debug"
                : $"({type:X8}) {( char )byte1}{( char )byte2}{( char )byte3}{( char )byte4} - {count}###{label}{type}Debug";
        }
    }
}
