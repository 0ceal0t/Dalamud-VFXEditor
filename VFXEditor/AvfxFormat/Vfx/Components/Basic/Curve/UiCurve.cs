using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCurve : UiAssignableItem {
        public readonly string Name;
        private readonly AVFXCurve Curve;
        private readonly bool Color = false;
        private readonly bool Locked;
        private readonly List<IUiBase> Parameters;
        private readonly UiCurveEditor CurveEdit;

        public UiCurve( AVFXCurve curve, string name, bool color = false, bool locked = false ) {
            Curve = curve;
            Name = name;
            Color = color;
            Locked = locked;

            CurveEdit = new UiCurveEditor( Curve, Color );
            Parameters = new List<IUiBase> {
                new UiCombo<CurveBehavior>( "Pre Behavior", Curve.PreBehavior ),
                new UiCombo<CurveBehavior>( "Post Behavior", Curve.PostBehavior )
            };
            if( !Color ) Parameters.Add( new UiCombo<RandomType>( "Random Type", Curve.Random ) );
        }

        public override void DrawUnassigned( string parentId ) {
            IUiBase.DrawAddButtonRecurse( Curve, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked && IUiBase.DrawRemoveButton( Curve, Name, id ) ) return;
            IUiBase.DrawList( Parameters, id );
            CurveEdit.DrawInline( id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();

        public static void DrawUnassignedCurves( List<UiCurve> curves, string id ) {
            var idx = 0;
            foreach( var curve in curves ) {
                if( !curve.IsAssigned() ) {
                    if( idx % 5 != 0 ) ImGui.SameLine();
                    curve.DrawInline( id );
                    idx++;
                }
            }
        }

        public static void DrawAssignedCurves( List<UiCurve> curves, string id ) {
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
