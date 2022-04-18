using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VFXEditor.AVFXLib.Model;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UIModel : UINode {
        public AVFXModel Model;
        public AvfxFile Main;
        public List<UIModelEmitterVertex> EmitterVerts;
        public UIModelEmitSplitView EmitSplit;
        public UINodeGraphView NodeView;
        private int Mode = 1;
        private bool Refresh = false;
        public bool Open = true;

        public UIModel( AvfxFile main, AVFXModel model ) : base( UINodeGroup.ModelColor, false ) {
            Model = model;
            Main = main;
            NodeView = new UINodeGraphView( this );

            EmitterVerts = new List<UIModelEmitterVertex>();
            for( var i = 0; i < Math.Min( Model.VNums.Nums.Count, Model.EmitVertexes.EmitVertexes.Count ); i++ ) {
                EmitterVerts.Add( new UIModelEmitterVertex( Model.VNums.Nums[i], Model.EmitVertexes.EmitVertexes[i], this ) );
            }
            EmitSplit = new UIModelEmitSplitView( EmitterVerts, this );
            HasDependencies = false; // if imported, all set now
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Model";
            NodeView.Draw( id );
            DrawRename( id );
            ImGui.Text( "Vertices: " + Model.Vertexes.Vertexes.Count + " " + "Indexes: " + Model.Indexes.Indexes.Count );
            if( ImGui.Button( "Export" + id ) ) {
                ImGui.OpenPopup( "Save_Popup" + id );
            }

            if( ImGui.BeginPopup( "Save_Popup" + id ) ) {
                if( ImGui.Selectable( "GLTF" + id ) ) {
                    ExportDialog();
                }
                if( ImGui.Selectable( "AVFX" + id ) ) {
                    AvfxFile.ExportDialog( this );
                }
                ImGui.EndPopup();
            }

            ImGui.SameLine();

            if( ImGui.Button( "Replace" + id ) ) {
                ImportDialog();
            }

            ImGui.BeginTabBar( "ModelTabs" );
            DrawModel3D( id );
            DrawEmitterVerts( id );
            ImGui.EndTabBar();
        }

        private void DrawModel3D( string parentId ) {
            var ret = ImGui.BeginTabItem( "3D View" + parentId );
            if( !ret ) return;
            if( Refresh ) {
                Plugin.DirectXManager.ModelView.LoadModel( Model, mode: Mode );
                Refresh = false;
            }

            var wireframe = Plugin.DirectXManager.ModelView.IsWireframe;
            if( ImGui.Checkbox( "Wireframe##3DModel", ref wireframe ) ) {
                Plugin.DirectXManager.ModelView.IsWireframe = wireframe;
                Plugin.DirectXManager.ModelView.RefreshRasterizeState();
                Plugin.DirectXManager.ModelView.Draw();
            }
            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Edges##3DModel", ref Plugin.DirectXManager.ModelView.ShowEdges ) ) {
                Plugin.DirectXManager.ModelView.Draw();
            }
            ImGui.SameLine();
            if( ImGui.Checkbox( "Show Emitter Vertices##3DModel", ref Plugin.DirectXManager.ModelView.ShowEmitter ) ) {
                Plugin.DirectXManager.ModelView.Draw();
            }
            if( ImGui.RadioButton( "Color", ref Mode, 1 ) ) {
                Plugin.DirectXManager.ModelView.LoadModel( Model, mode: 1 );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 1", ref Mode, 2 ) ) {
                Plugin.DirectXManager.ModelView.LoadModel( Model, mode: 2 );
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 2", ref Mode, 3 ) ) {
                Plugin.DirectXManager.ModelView.LoadModel( Model, mode: 3 );
            }

            var cursor = ImGui.GetCursorScreenPos();
            ImGui.BeginChild( "3DViewChild" );

            var space = ImGui.GetContentRegionAvail();
            Plugin.DirectXManager.ModelView.Resize( space );

            ImGui.ImageButton( Plugin.DirectXManager.ModelView.Output, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );

            if( ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                Plugin.DirectXManager.ModelView.Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                Plugin.DirectXManager.ModelView.Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                Plugin.DirectXManager.ModelView.IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) {
                Plugin.DirectXManager.ModelView.Zoom( ImGui.GetIO().MouseWheel );
            }

            ImGui.EndChild();
            ImGui.EndTabItem();
        }

        private void DrawEmitterVerts( string parentId ) {
            var ret = ImGui.BeginTabItem( "Emitter Vertices" + parentId );
            if( !ret )
                return;
            EmitSplit.Draw( parentId );
            ImGui.EndTabItem();
        }

        private void ImportDialog() {
            FileDialogManager.OpenFileDialog( "Select a File", ".gltf,.*", ( bool ok, string res ) => {
                if( !ok ) return;
                try {
                    if( GltfHelper.ImportModel( res, out var v_s, out var i_s ) ) {
                        Model.Vertexes.Vertexes.Clear();
                        Model.Vertexes.Vertexes.AddRange(v_s);

                        Model.Indexes.Indexes.Clear();
                        Model.Indexes.Indexes.AddRange(i_s);
                        Refresh = true;
                    }
                }
                catch( Exception e ) {
                    PluginLog.Error( "Could not import data", e );
                }
            } );
        }

        private void ExportDialog() {
            FileDialogManager.SaveFileDialog( "Select a Save Location", ".gltf", "model", "gltf", ( bool ok, string res ) => {
                if( !ok ) return;
                GltfHelper.ExportModel( Model, res );
            } );
        }

        public override string GetDefaultText() => $"Model {Idx}";

        public override string GetWorkspaceId() => $"Mdl{Idx}";

        public override void Write( BinaryWriter writer ) => Model.Write( writer );
    }
}
