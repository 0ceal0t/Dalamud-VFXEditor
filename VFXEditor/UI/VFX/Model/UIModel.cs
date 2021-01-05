using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VFXEditor.UI.VFX
{
    public class UIModel : UIBase
    {
        public AVFXModel Model;
        public UIModelView View;
        //=======================
        public List<UIBase> EmitterVerts;
        public UIModelEmitSplitView EmitSplit;

        public UIModel(AVFXModel model, UIModelView view)
        {
            Model = model;
            View = view;
            //===============
            Init();
        }
        public override void Init()
        {
            base.Init();
            EmitterVerts = new List<UIBase>();
            for(int i = 0; i < Math.Min(Model.VNums.Count, Model.EmitVertices.Count); i++ )
            {
                EmitterVerts.Add( new UIModelEmitterVertex( Model.VNums[i], Model.EmitVertices[i], this ) );
            }
            EmitSplit = new UIModelEmitSplitView( EmitterVerts, this );
        }

        // ============== DRAW ===============
        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                return;
            }
            if( ImGui.Selectable( "Model " + Idx + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/Model" + Idx;
            if( UIUtils.RemoveButton( "Delete" + id, small:true ) )
            {
                View.AVFX.removeModel( Idx );
                View.Init();
                return;
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Import" + id ) )
            {
                ImportDialog();
            }
            ImGui.SameLine();
            if( ImGui.SmallButton( "Export" + id ) )
            {
                ExportDialog();
            }
            ImGui.Text( "Vertices: " + Model.Vertices.Count + " " + "Indexes: " + Model.Indexes.Count);

            if(ImGui.CollapsingHeader("Emitter Vertices" + id ) )
            {
                EmitSplit.Draw( id );
            }
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
    }
}
