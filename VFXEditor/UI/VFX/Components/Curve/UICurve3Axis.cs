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
    public class UICurve3Axis : UIBase
    {
        public AVFXCurve3Axis Curve;
        public string Name;
        //=========================
        public UICurve X;
        public UICurve Y;
        public UICurve Z;
        public UICurve RX;
        public UICurve RY;
        public UICurve RZ;

        public UICurve3Axis(AVFXCurve3Axis curve, string name)
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
            Attributes.Add(new UICurve(Curve.Z, "Z"));
            Attributes.Add(new UICurve(Curve.RX, "RX"));
            Attributes.Add(new UICurve(Curve.RY, "RY"));
            Attributes.Add(new UICurve(Curve.RZ, "RZ"));
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
