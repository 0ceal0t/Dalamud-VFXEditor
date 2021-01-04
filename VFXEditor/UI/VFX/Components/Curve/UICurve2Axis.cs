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
    public class UICurve2Axis : UIBase
    {
        public AVFXCurve2Axis Curve;
        public string Name;
        //=========================

        public UICurve2Axis(AVFXCurve2Axis curve, string name)
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
            Attributes.Add(new UICombo<AxisConnect>("Axis Connect", Curve.AxisConnectType));
            Attributes.Add(new UICombo<RandomType>("Axis Connect Random", Curve.AxisConnectRandomType));
            Attributes.Add(new UICurve(Curve.X, "X"));
            Attributes.Add(new UICurve(Curve.Y, "Y"));
            Attributes.Add(new UICurve(Curve.RX, "RX"));
            Attributes.Add(new UICurve(Curve.RY, "RY"));
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
