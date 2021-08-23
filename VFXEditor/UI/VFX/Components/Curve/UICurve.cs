using AVFXLib.Models;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UICurve : UIItem
    {
        public AVFXCurve Curve;
        public string Name;
        public bool Color = false;
        public bool Locked;
        public UICurveEditor CurveEdit;

        public UICurve(AVFXCurve curve, string name, bool color = false, bool locked = false)
        {
            Curve = curve;
            Name = name;
            Color = color;
            Locked = locked;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Curve.Assigned) { Assigned = false; return; }
            //=====================
            CurveEdit = new UICurveEditor( Curve, Color );
            Attributes.Add(new UICombo<CurveBehavior>("Pre Behavior", Curve.PreBehavior));
            Attributes.Add(new UICombo<CurveBehavior>("Post Behavior", Curve.PostBehavior));
            if (!Color) {
                Attributes.Add(new UICombo<RandomType>("Random Type", Curve.Random));
            }
        }

        // ======= DRAW ================
        public override void Draw( string parentId ) {
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
        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) )
            {
                Curve.ToDefault();
                Init();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked ) {
                if( UIUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                    Curve.Assigned = false;
                    Init();
                    return;
                }
            }
            // =====================
            DrawAttrs( id );
            CurveEdit.Draw(id);
        }

        public override string GetDefaultText() {
            return Name;
        }
    }
}
