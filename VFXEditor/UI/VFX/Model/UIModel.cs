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

namespace VFXEditor.UI.VFX
{
    public class UIModel : UIItem {
        public AVFXModel Model;
        public UIModelView View;
        public Model3D Mdl3D;
        //=======================
        public List<UIModelEmitterVertex> EmitterVerts;
        public UIModelEmitSplitView EmitSplit;

        public UIModel( AVFXModel model, UIModelView view ) {
            Model = model;
            View = view;
            //===============
            EmitterVerts = new List<UIModelEmitterVertex>();
            for( int i = 0; i < Math.Min( Model.VNums.Count, Model.EmitVertices.Count ); i++ ) {
                EmitterVerts.Add( new UIModelEmitterVertex( Model.VNums[i], Model.EmitVertices[i], this ) );
            }
            EmitSplit = new UIModelEmitSplitView( EmitterVerts, this );
            Mdl3D = View._plugin.DXManager.Model;
        }

        public override void DrawSelect( string parentId, ref UIItem selected ) {
            if( ImGui.Selectable( GetText() + parentId, selected == this ) ) {
                selected = this;
                Mdl3D.LoadModel( Model );
            }
        }

        public bool Open = true;
        public override void DrawBody( string parentId ) {
            string id = parentId + "/Model";
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) ) {
                View.AVFX.removeModel( Model );
                View.ModelSplit.OnDelete( this );
                return;
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Import" + id ) ) {
                ImportDialog();
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Export" + id ) ) {
                ExportDialog();
            }
            ImGui.Text( "Vertices: " + Model.Vertices.Count + " " + "Indexes: " + Model.Indexes.Count );

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
            ImGui.BeginChild( "3DViewChild" );
            var space = ImGui.GetContentRegionAvail();
            Mdl3D.Resize( space );

            ImGui.ImageButton( Mdl3D.RenderShad.NativePointer, space, new Vector2( 0, 0 ), new Vector2( 1, 1 ), 0 );
            if(ImGui.IsItemActive() && ImGui.IsMouseDragging() ) {
                var delta = ImGui.GetMouseDragDelta();
                Mdl3D.Drag( delta );
            }
            else {
                Mdl3D.IsDragging = false;
            }

            if( ImGui.IsItemHovered() ) {
                Mdl3D.Zoom( ImGui.GetIO().MouseWheel );
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

        public void ImportDialog()
        {
            Task.Run( async () =>
            {
                var picker = new OpenFileDialog
                {
                    Filter = "GLTF File (*.gltf)|*.gltf*|All files (*.*)|*.*",
                    CheckFileExists = true,
                    Title = "Select GLTF File."
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK )
                {
                    try
                    {
                        if(GLTFManager.ImportModel( picker.FileName, out List<Vertex> v_s, out List<Index> i_s ) )
                        {
                            Model.Vertices = v_s;
                            Model.Indexes = i_s;
                            Mdl3D.LoadModel( Model );
                        }
                    }
                    catch( Exception ex )
                    {
                        PluginLog.LogError( ex, "Could not select the GLTF file." );
                    }
                }
            } );
        }

        public void ExportDialog()
        {
            Task.Run( async () =>
            {
                var picker = new SaveFileDialog
                {
                    Filter = "GLTF File (*.gltf)|*.gltf*|All files (*.*)|*.*",
                    Title = "Select a Save Location.",
                    DefaultExt = "gltf",
                    AddExtension = true
                };
                var result = await picker.ShowDialogAsync();
                if( result == DialogResult.OK )
                {
                    try
                    {
                        GLTFManager.ExportModel( Model, picker.FileName);
                    }
                    catch( Exception ex )
                    {
                        PluginLog.LogError( ex, "Could not select a save location" );
                    }
                }
            } );
        }

        public override string GetText() {
            return "Model " + Idx;
        }
    }
}
