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
        //=======================
        public List<UICurve> Curves;

        public UICurve2Axis(AVFXCurve2Axis curve, string name)
        {
            Curve = curve;
            Name = name;
            Init();
        }
        public override void Init()
        {
            base.Init();
            Curves = new List<UICurve>();
            if (!Curve.Assigned) { Assigned = false; return; }
            // ======================
            Attributes.Add(new UICombo<AxisConnect>("Axis Connect", Curve.AxisConnectType));
            Attributes.Add(new UICombo<RandomType>("Axis Connect Random", Curve.AxisConnectRandomType));
            Curves.Add(new UICurve(Curve.X, "X"));
            Curves.Add(new UICurve(Curve.Y, "Y"));
            Curves.Add(new UICurve(Curve.RX, "RX"));
            Curves.Add(new UICurve(Curve.RY, "RY"));
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
            // =================
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Curve.Assigned = false;
                Init();
                return;
            }
            foreach( var c in Curves )
            {
                if( !c.Assigned )
                {
                    ImGui.SameLine();
                    c.Draw( id );
                }
            }
            DrawAttrs( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            // ==================
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) )
            {
                foreach( var c in Curves )
                {
                    if( c.Assigned )
                    {
                        if( ImGui.BeginTabItem( c.Name + id ) )
                        {
                            c.DrawBody( id );
                            ImGui.EndTabItem();
                        }
                    }
                }
                ImGui.EndTabBar();
            }
        }
    }
}
