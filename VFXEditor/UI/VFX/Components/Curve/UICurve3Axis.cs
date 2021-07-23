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
    public class UICurve3Axis : UIItem
    {
        public AVFXCurve3Axis Curve;
        public string Name;
        public bool Locked;
        //=========================
        public List<UICurve> Curves;

        public UICurve3Axis(AVFXCurve3Axis curve, string name, bool locked = false )
        {
            Curve = curve;
            Name = name;
            Locked = locked;
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
            Curves.Add(new UICurve(Curve.Z, "Z"));
            Curves.Add(new UICurve(Curve.RX, "Random X"));
            Curves.Add(new UICurve(Curve.RY, "Random Y"));
            Curves.Add(new UICurve(Curve.RZ, "Random Z"));
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
        public override void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ " + Name + parentId ) )
            {
                Curve.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/" + Name;
            // =================
            if( !Locked ) {
                if( UIUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                    Curve.Assigned = false;
                    Init();
                    return;
                }
            }

            int idx = 0;
            foreach( var c in Curves ) {
                if( !c.Assigned ) {
                    if( idx % 5 != 0 ) {
                        ImGui.SameLine();
                    }
                    c.Draw( id );
                    idx++;
                }
            }

            DrawAttrs( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            // ==================
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) )
            {
                foreach(var c in Curves )
                {
                    if( c.Assigned )
                    {
                        if( ImGui.BeginTabItem(c.Name + id) )
                        {
                            c.DrawBody( id );
                            ImGui.EndTabItem();
                        }
                    }
                }
                ImGui.EndTabBar();
            }
        }

        public override string GetDefaultText() {
            return Name;
        }
    }
}
