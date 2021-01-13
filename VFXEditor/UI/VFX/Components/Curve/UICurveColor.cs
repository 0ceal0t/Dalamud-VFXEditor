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
    public class UICurveColor : UIItem
    {
        public AVFXCurveColor Curve;
        public string Name;
        //=========================
        public List<UICurve> Curves;

        public UICurveColor(AVFXCurveColor curve, string name)
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
            Curves.Add(new UICurve(Curve.RGB, "RGB", color: true));
            Curves.Add(new UICurve(Curve.A, "A"));
            Curves.Add(new UICurve(Curve.SclR, "Scale R"));
            Curves.Add(new UICurve(Curve.SclG, "Scale G"));
            Curves.Add(new UICurve(Curve.SclB, "Scale B"));
            Curves.Add(new UICurve(Curve.SclA, "Scale A"));
            Curves.Add(new UICurve(Curve.Bri, "Bright"));
            Curves.Add(new UICurve(Curve.RanR, "Random R"));
            Curves.Add(new UICurve(Curve.RanG, "Random G"));
            Curves.Add(new UICurve(Curve.RanB, "Random B"));
            Curves.Add(new UICurve(Curve.RanA, "Random A"));
            Curves.Add(new UICurve(Curve.RBri, "Random Bright"));
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
        public override void DrawSelect( int idx, string parentId, ref UIItem selected )
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
            //====================
            if( UIUtils.RemoveButton( "Delete" + id, small: true ) )
            {
                Curve.Assigned = false;
                Init();
                return;
            }
            int idx = 0;
            foreach( var c in Curves )
            {
                if( !c.Assigned )
                {
                    ImGui.SameLine();
                    c.Draw( id );
                    if(idx == 5 )
                    {
                        ImGui.NewLine();
                    }
                    idx++;
                }
            }
            DrawAttrs( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            //====================
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

        public override string GetText( int idx ) {
            return Name;
        }
    }
}
