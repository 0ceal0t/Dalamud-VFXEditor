using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFXEditor.Data.DirectX;
using VFXEditor.External;

namespace VFXEditor.UI.VFX
{
    public class UIModel : UINode {
        public AVFXModel Model;
        public UIMain Main;
        public ModelPreview _ModelPreview;
        //=======================
        public List<UIModelEmitterVertex> EmitterVerts;
        public UIModelEmitSplitView EmitSplit;
        public UINodeGraphView NodeView;

        private int Mode = 1;
        private bool Refresh = false;

        public UIModel( UIMain main, AVFXModel model ) : base( UINodeGroup.ModelColor, false ) {
            Model = model;
            Main = main;
            NodeView = new UINodeGraphView( this );
            //===============
            EmitterVerts = new List<UIModelEmitterVertex>();
            for( int i = 0; i < Math.Min( Model.VNums.Count, Model.EmitVertices.Count ); i++ ) {
                EmitterVerts.Add( new UIModelEmitterVertex( Model.VNums[i], Model.EmitVertices[i], this ) );
            }
            EmitSplit = new UIModelEmitSplitView( EmitterVerts, this );
            _ModelPreview = DirectXManager.Manager.ModelView;
            HasDependencies = false; // if imported, all set now
        }

        public bool Open = true;
        public override void DrawBody( string parentId ) {
            string id = parentId + "/Model";
            NodeView.Draw( id );
            DrawRename( id );
            ImGui.Text( "Vertices: " + Model.Vertices.Count + " " + "Indexes: " + Model.Indexes.Count );
            if( ImGui.SmallButton( "Replace" + id ) ) {
                ImportDialog();
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Export" + id ) ) {
                ImGui.OpenPopup( "Save_Popup" + id );
            }
            // ==================
            if( ImGui.BeginPopup( "Save_Popup" + id ) ) {
                if( ImGui.Selectable( "GLTF" + id ) ) {
                    ExportDialog();
                }
                if( ImGui.Selectable( "AVFX" + id ) ) {
                    UIMain.ExportDialog( this );
                }
                ImGui.EndPopup();
            }

            ImGui.BeginTabBar( "ModelTabs" );
            DrawModel3D(id);
            DrawEmitterVerts(id);
            ImGui.EndTabBar();
        }

        public void DrawModel3D( string parentId ) {
            var ret = ImGui.BeginTabItem( "3D View" + parentId);
            if( !ret )
                return;
            // ===============
            if(Refresh) {
                _ModelPreview.LoadModel( Model, mode: Mode );
                Refresh = false;
            }

            bool wireframe = _ModelPreview.IsWireframe;
            if(ImGui.Checkbox("Wireframe##3DModel", ref wireframe ) ) {
                _ModelPreview.IsWireframe = wireframe;
                _ModelPreview.RefreshRasterizeState();
                _ModelPreview.Draw();
            }
            ImGui.SameLine();
            if(ImGui.Checkbox( "Show Edges##3DModel", ref _ModelPreview.ShowEdges ) ) {
                _ModelPreview.Draw();
            }
            ImGui.SameLine();
            if(ImGui.Checkbox( "Show Emitter Vertices##3DModel", ref _ModelPreview.ShowEmitter ) ) {
                _ModelPreview.Draw();
            }
            if(ImGui.RadioButton( "Color", ref Mode, 1 ) ) {
                _ModelPreview.LoadModel( Model, mode:1);
            }
            ImGui.SameLine();
            if(ImGui.RadioButton( "UV 1", ref Mode, 2 ) ) {
                _ModelPreview.LoadModel( Model, mode: 2 );
            }
            ImGui.SameLine();
            if(ImGui.RadioButton( "UV 2", ref Mode, 3 ) ) {
                _ModelPreview.LoadModel( Model, mode: 3 );
            }

            var cursor = ImGui.GetCursorScreenPos();
            ImGui.BeginChild( "3DViewChild" );

            var space = ImGui.GetContentRegionAvail();
            _ModelPreview.Resize( space );

            ImGui.ImageButton( _ModelPreview.RenderShad.NativePointer, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );

            if(ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                _ModelPreview.Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                _ModelPreview.Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                _ModelPreview.IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) {
                _ModelPreview.Zoom( ImGui.GetIO().MouseWheel );
            }

            ImGui.EndChild();
            // ===============
            ImGui.EndTabItem();
        }

        public void DrawEmitterVerts( string parentId ) {
            var ret = ImGui.BeginTabItem( "Emitter Vertices" + parentId );
            if( !ret )
                return;
            EmitSplit.Draw( parentId );
            ImGui.EndTabItem();
        }

        public void ImportDialog() {
            Plugin.ImportFileDialog( "GLTF File (*.gltf)|*.gltf*|All files (*.*)|*.*", "Select GLTF File.",
                ( string path ) => {
                    try {
                        if( GLTFManager.ImportModel( path, out List<Vertex> v_s, out List<Index> i_s ) ) {
                            Model.Vertices = v_s;
                            Model.Indexes = i_s;
                            Refresh = true;
                        }
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select the GLTF file." );
                    }
                }
            );
        }

        public void ExportDialog() {
            Plugin.SaveFileDialog( "GLTF File (*.gltf)|*.gltf*|All files (*.*)|*.*", "Select a Save Location.", "gltf",
                ( string path ) => {
                    try {
                        GLTFManager.ExportModel( Model, path );
                    }
                    catch( Exception ex ) {
                        PluginLog.LogError( ex, "Could not select a save location" );
                    }
                }
            );
        }

        public override string GetDefaultText() {
            return "Model " + Idx;
        }

        public override byte[] ToBytes() {
            return Model.ToAVFX().ToBytes();
        }
    }
}
