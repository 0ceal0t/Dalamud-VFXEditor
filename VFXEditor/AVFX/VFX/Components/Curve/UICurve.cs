using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Curve;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public class UICurve : UIAssignableItem {
        public readonly string Name;
        private readonly AVFXCurve Curve;
        private readonly bool Color = false;
        private readonly bool Locked;
        private readonly List<IUIBase> Parameters;
        private readonly UICurveEditor CurveEdit;

        public UICurve( AVFXCurve curve, string name, bool color = false, bool locked = false ) {
            Curve = curve;
            Name = name;
            Color = color;
            Locked = locked;

            CurveEdit = new UICurveEditor( Curve, Color );
            Parameters = new List<IUIBase> {
                new UICombo<CurveBehavior>( "Pre Behavior", Curve.PreBehavior ),
                new UICombo<CurveBehavior>( "Post Behavior", Curve.PostBehavior )
            };
            if( !Color ) {
                Parameters.Add( new UICombo<RandomType>( "Random Type", Curve.Random ) );
            }
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                AVFXBase.RecurseAssigned( Curve, true );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked ) {
                if( UiUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                    Curve.SetAssigned( false );
                    return;
                }
            }
            IUIBase.DrawList( Parameters, id );
            CurveEdit.DrawInline( id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();

        public static void DrawUnassignedCurves( List<UICurve> curves, string id ) {
            var idx = 0;
            foreach( var curve in curves ) {
                if( !curve.IsAssigned() ) {
                    if( idx % 5 != 0 ) {
                        ImGui.SameLine();
                    }
                    curve.DrawInline( id );
                    idx++;
                }
            }
        }

        public static void DrawAssignedCurves( List<UICurve> curves, string id ) {
            if( ImGui.BeginTabBar( id + "/Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                foreach( var curve in curves ) {
                    if( curve.IsAssigned() ) {
                        if( ImGui.BeginTabItem( curve.Name + id ) ) {
                            curve.DrawAssigned( id );
                            ImGui.EndTabItem();
                        }
                    }
                }
                ImGui.EndTabBar();
            }
        }
    }
}
