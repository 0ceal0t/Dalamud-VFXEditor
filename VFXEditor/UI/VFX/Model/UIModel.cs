using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.Data.DirectX;
using VFXEditor.External;

namespace VFXEditor.UI.VFX
{
    public class UIModel : UINode {
        public AVFXModel Model;
        public UIMain Main;

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
            for( var i = 0; i < Math.Min( Model.VNums.Count, Model.EmitVertices.Count ); i++ ) {
                EmitterVerts.Add( new UIModelEmitterVertex( Model.VNums[i], Model.EmitVertices[i], this ) );
            }
            EmitSplit = new UIModelEmitSplitView( EmitterVerts, this );
            HasDependencies = false; // if imported, all set now
        }

        public bool Open = true;
        public override void DrawBody( string parentId ) {
            var id = parentId + "/Model";
            NodeView.Draw( id );
            DrawRename( id );
            ImGui.Text( "Vertices: " + Model.Vertices.Count + " " + "Indexes: " + Model.Indexes.Count );
            if( ImGui.Button( "Replace" + id ) ) {
                ImportDialog();
            }
            ImGui.SameLine();
            if( ImGui.Button( "Export" + id ) ) {
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
                DirectXManager.ModelView.LoadModel( Model, mode: Mode );
                Refresh = false;
            }

            var wireframe = DirectXManager.ModelView.IsWireframe;
            if(ImGui.Checkbox("Wireframe##3DModel", ref wireframe ) ) {
                DirectXManager.ModelView.IsWireframe = wireframe;
                DirectXManager.ModelView.RefreshRasterizeState();
                DirectXManager.ModelView.Draw();
            }
            ImGui.SameLine();
            if(ImGui.Checkbox( "Show Edges##3DModel", ref DirectXManager.ModelView.ShowEdges ) ) {
                DirectXManager.ModelView.Draw();
            }
            ImGui.SameLine();
            if(ImGui.Checkbox( "Show Emitter Vertices##3DModel", ref DirectXManager.ModelView.ShowEmitter ) ) {
                DirectXManager.ModelView.Draw();
            }
            if(ImGui.RadioButton( "Color", ref Mode, 1 ) ) {
                DirectXManager.ModelView.LoadModel( Model, mode:1);
            }
            ImGui.SameLine();
            if(ImGui.RadioButton( "UV 1", ref Mode, 2 ) ) {
                DirectXManager.ModelView.LoadModel( Model, mode: 2 );
            }
            ImGui.SameLine();
            if(ImGui.RadioButton( "UV 2", ref Mode, 3 ) ) {
                DirectXManager.ModelView.LoadModel( Model, mode: 3 );
            }

            var cursor = ImGui.GetCursorScreenPos();
            ImGui.BeginChild( "3DViewChild" );

            var space = ImGui.GetContentRegionAvail();
            DirectXManager.ModelView.Resize( space );

            ImGui.ImageButton( DirectXManager.ModelView.Output, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );

            if(ImGui.IsItemActive() && ImGui.IsMouseDragging( ImGuiMouseButton.Left ) ) {
                var delta = ImGui.GetMouseDragDelta();
                DirectXManager.ModelView.Drag( delta, true );
            }
            else if( ImGui.IsWindowHovered() && ImGui.IsMouseDragging( ImGuiMouseButton.Right ) ) {
                DirectXManager.ModelView.Drag( ImGui.GetMousePos() - cursor, false );
            }
            else {
                DirectXManager.ModelView.IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) {
                DirectXManager.ModelView.Zoom( ImGui.GetIO().MouseWheel );
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
            Plugin.DialogManager.OpenFileDialog( "Select a File", ".gltf,.*", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                try {
                    if( GLTF.ImportModel( res, out var v_s, out var i_s ) ) {
                        Model.Vertices = v_s;
                        Model.Indexes = i_s;
                        Refresh = true;
                    }
                }
                catch( Exception e ) {
                    PluginLog.LogError( "Could not import data", e );
                }
            } );
        }

        public void ExportDialog() {
            Plugin.DialogManager.SaveFileDialog( "Select a Save Location", ".gltf", "model", "gltf", ( bool ok, string res ) =>
            {
                if( !ok ) return;
                GLTF.ExportModel( Model, res );
            } );
        }

        public override string GetDefaultText() {
            return "Model " + Idx;
        }

        public override string GetWorkspaceId() {
            return $"Mdl{Idx}";
        }

        public override byte[] ToBytes() {
            return Model.ToAVFX().ToBytes();
        }
    }
}
