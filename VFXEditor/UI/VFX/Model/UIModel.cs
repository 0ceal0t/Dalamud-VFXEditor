using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIModel : UIBase
    {
        public AVFXModel Model;
        public UIModelView View;
        //=======================

        public UIModel(AVFXModel model, UIModelView view)
        {
            Model = model;
            View = view;
            //=======================
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
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                View.AVFX.removeModel( Idx );
                View.Init();
                return;
            }
            ImGui.Text( "Vertices: " + Model.Vertices.Count + " " + "Indexes: " + Model.Indexes.Count);
            ImGui.SameLine();
            if(ImGui.Button("Import" + id ) )
            {

            }
            ImGui.SameLine();
            if(ImGui.Button("Export" + id ) )
            {

            }

            ImGui.Separator();
            // [Delete]
            // Vertices + Indexes #
            // IMPORT | EXPORT
            // Emitter Vertices:
            //      Position
            //      Normal
            //      Color
            //      Order
            //      [+ New]
        }
    }
}
