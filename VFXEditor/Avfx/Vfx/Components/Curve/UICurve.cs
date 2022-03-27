using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VFXEditor.Helper;

namespace VFXEditor.Avfx.Vfx {
    public class UICurve : UIItem {
        public readonly string Name;
        private readonly AVFXCurve Curve;
        private readonly bool Color = false;
        private readonly bool Locked;
        private List<UIBase> Parameters;
        private  UICurveEditor CurveEdit;

        public UICurve( AVFXCurve curve, string name, bool color = false, bool locked = false ) {
            Curve = curve;
            Name = name;
            Color = color;
            Locked = locked;
            Init();
        }

        public override void Init() {
            base.Init();
            if( !Curve.Assigned ) { Assigned = false; return; }

            CurveEdit = new UICurveEditor( Curve, Color );
            Parameters = new List<UIBase> {
                new UICombo<CurveBehavior>( "Pre Behavior", Curve.PreBehavior ),
                new UICombo<CurveBehavior>( "Post Behavior", Curve.PostBehavior )
            };
            if( !Color ) {
                Parameters.Add( new UICombo<RandomType>( "Random Type", Curve.Random ) );
            }
        }

        public override void Draw( string parentId ) {
            if( !Assigned ) {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.TreeNode( Name + parentId ) ) {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                Curve.ToDefault();
                Init();
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked ) {
                if( UiHelper.RemoveButton( "Delete " + Name + id, small: true ) ) {
                    Curve.Assigned = false;
                    Init();
                    return;
                }
            }
            DrawList( Parameters, id );
            CurveEdit.Draw( id );
        }

        public override string GetDefaultText() => Name;
    }
}
