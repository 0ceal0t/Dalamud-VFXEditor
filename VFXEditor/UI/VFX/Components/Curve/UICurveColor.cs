using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UICurveColor : UIBase
    {
        public AVFXCurveColor Curve;
        public string Name;
        //=========================

        public UICurveColor(AVFXCurveColor curve, string name)
        {
            Curve = curve;
            Name = name;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Curve.Assigned) { Assigned = false; return; }
            // ======================
            Attributes.Add(new UICurve(Curve.RGB, "RGB", color: true));
            Attributes.Add(new UICurve(Curve.A, "Alpha"));
            Attributes.Add(new UICurve(Curve.SclR, "Scale R"));
            Attributes.Add(new UICurve(Curve.SclG, "Scale G"));
            Attributes.Add(new UICurve(Curve.SclB, "Scale B"));
            Attributes.Add(new UICurve(Curve.SclA, "Scale Alpha"));
            Attributes.Add(new UICurve(Curve.Bri, "Brightness"));
            Attributes.Add(new UICurve(Curve.RanR, "Random R"));
            Attributes.Add(new UICurve(Curve.RanG, "Random G"));
            Attributes.Add(new UICurve(Curve.RanB, "Random B"));
            Attributes.Add(new UICurve(Curve.RanA, "Random Alpha"));
            Attributes.Add(new UICurve(Curve.RBri, "Random Brightness"));
        }
        // =========== DRAW =====================
        public override void Draw( string parentId )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.TreeNode( Name + parentId ) )
            {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.Selectable( Name + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        private void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ " + Name + parentId ) )
            {
                Curve.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/" + Name;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Curve.Assigned = false;
                Init();
            }
            DrawAttrs( id );
            ImGui.TreePop();
        }
    }
}
