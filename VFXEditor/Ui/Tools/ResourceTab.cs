using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Interop;

namespace VfxEditor.Ui.Tools {
    public unsafe class ResourceTab {
        private struct ResourceItemStruct {
            public uint Hash;
            public ulong Address;
            public string Path;
            public uint Refs;
        }

        private string Search = "";

        // Adapted from https://github.com/xivdev/Penumbra/blob/7e7e74a5346857328ee161d571c1f1ead6524e9a/Penumbra/UI/MenuTabs/TabResourceManager.cs
        public void Draw() {
            var resourceHandler = *( ResourceManager** )Dalamud.SigScanner.GetStaticAddressFromSig( Constants.ResourceManagerSig );

            if( resourceHandler == null ) return;

            ImGui.InputTextWithHint( "##ResourceManager/Search", "Search", ref Search, 256 );

            if( !ImGui.BeginChild( "##ResourceManager", -Vector2.One, true ) ) return;

            DrawCategoryContainer( ResourceCategory.Common, resourceHandler->ResourceGraph->Containers[0] );
            DrawCategoryContainer( ResourceCategory.BgCommon, resourceHandler->ResourceGraph->Containers[1] );
            DrawCategoryContainer( ResourceCategory.Bg, resourceHandler->ResourceGraph->Containers[2] );
            DrawCategoryContainer( ResourceCategory.Cut, resourceHandler->ResourceGraph->Containers[3] );
            DrawCategoryContainer( ResourceCategory.Chara, resourceHandler->ResourceGraph->Containers[4] );
            DrawCategoryContainer( ResourceCategory.Shader, resourceHandler->ResourceGraph->Containers[5] );
            DrawCategoryContainer( ResourceCategory.Ui, resourceHandler->ResourceGraph->Containers[6] );
            DrawCategoryContainer( ResourceCategory.Sound, resourceHandler->ResourceGraph->Containers[7] );
            DrawCategoryContainer( ResourceCategory.Vfx, resourceHandler->ResourceGraph->Containers[8] );
            DrawCategoryContainer( ResourceCategory.UiScript, resourceHandler->ResourceGraph->Containers[9] );
            DrawCategoryContainer( ResourceCategory.Exd, resourceHandler->ResourceGraph->Containers[10] );
            DrawCategoryContainer( ResourceCategory.GameScript, resourceHandler->ResourceGraph->Containers[11] );
            DrawCategoryContainer( ResourceCategory.Music, resourceHandler->ResourceGraph->Containers[12] );
            DrawCategoryContainer( ResourceCategory.SqpackTest, resourceHandler->ResourceGraph->Containers[13] );
            DrawCategoryContainer( ResourceCategory.Debug, resourceHandler->ResourceGraph->Containers[14] );

            ImGui.EndChild();
        }

        private void DrawCategoryContainer( ResourceCategory category, ResourceGraph.CategoryContainer container ) {
            var map = container.CategoryMaps[0].Value;
            if( map == null || !ImGui.TreeNodeEx( $"({( uint )category:D2}) {category} - {map->Count}###{( uint )category}Debug" ) ) return;

            foreach( var key in map->Keys ) {
                var value = map->GetValueOrDefault( key ).Value;
                DrawResourceMap( GetNodeLabel( ( uint )category, key, ( ulong )value->Count ), value );
            }

            ImGui.TreePop();
        }

        private unsafe void DrawResourceMap( string label, StdMap<uint, Pointer<ResourceHandle>>* typeMap ) {
            if( typeMap == null ) return;

            var itemList = new List<ResourceItemStruct>();
            if( typeMap->Count > 0 ) {
                foreach( var key in typeMap->Keys ) {
                    var value = typeMap->GetValueOrDefault( key ).Value;
                    var path = value->FileName.ToString();

                    if( string.IsNullOrEmpty( Search ) || path.Contains( Search, System.StringComparison.CurrentCultureIgnoreCase ) ) { // match against search
                        itemList.Add( new ResourceItemStruct {
                            Hash = key,
                            Address = ( ulong )value,
                            Path = path,
                            Refs = value->RefCount
                        } );
                    }
                }
            }

            if( itemList.Count == 0 && !string.IsNullOrEmpty( Search ) ) return; // all filtered out

            if( !ImGui.TreeNodeEx( label ) ) return;

            if( itemList.Count == 0 || !ImGui.BeginTable( $"##{label}_table", 4, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg ) ) {
                ImGui.TreePop();
                return;
            }

            ImGui.TableSetupColumn( "Hash", ImGuiTableColumnFlags.WidthFixed, 100 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Ptr", ImGuiTableColumnFlags.WidthFixed, 100 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Path", ImGuiTableColumnFlags.WidthFixed,
                ( ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X ) - 300 * ImGuiHelpers.GlobalScale );
            ImGui.TableSetupColumn( "Refs", ImGuiTableColumnFlags.WidthFixed, 30 * ImGuiHelpers.GlobalScale );
            ImGui.TableHeadersRow();

            foreach( var item in itemList ) {
                ImGui.TableNextRow();
                ImGui.TableNextColumn();
                ImGui.Text( $"0x{item.Hash:X8}" );
                ImGui.TableNextColumn();
                var address = $"0x{item.Address:X}";
                ImGui.Text( address );
                if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( address );

                ImGui.TableNextColumn();
                ImGui.Text( item.Path );
                if( ImGui.IsItemClicked() ) ImGui.SetClipboardText( item.Path );

                ImGui.TableNextColumn();
                ImGui.Text( item.Refs.ToString() );
            }

            ImGui.EndTable();
            ImGui.TreePop();
        }

        private static string GetNodeLabel( uint label, uint type, ulong count ) {
            var byte1 = type >> 24;
            var byte2 = type >> 16 & 0xFF;
            var byte3 = type >> 8 & 0xFF;
            var byte4 = type & 0xFF;
            return byte1 == 0
                ? $"({type:X8}) {( char )byte2}{( char )byte3}{( char )byte4} - {count}###{label}{type}Debug"
                : $"({type:X8}) {( char )byte1}{( char )byte2}{( char )byte3}{( char )byte4} - {count}###{label}{type}Debug";
        }
    }
}
