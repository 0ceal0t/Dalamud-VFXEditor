using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Curve;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public class UICurveColor : UIAssignableItem {
        public readonly AVFXCurveColor Curve;
        public readonly string Name;
        public readonly bool Locked;
        private readonly List<UICurve> Curves;

        public UICurveColor( AVFXCurveColor curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;

            Curves = new List<UICurve> {
                new UICurve( Curve.RGB, "RGB", color: true ),
                new UICurve( Curve.A, "A" ),
                new UICurve( Curve.SclR, "Scale R" ),
                new UICurve( Curve.SclG, "Scale G" ),
                new UICurve( Curve.SclB, "Scale B" ),
                new UICurve( Curve.SclA, "Scale A" ),
                new UICurve( Curve.Bri, "Bright" ),
                new UICurve( Curve.RanR, "Random R" ),
                new UICurve( Curve.RanG, "Random G" ),
                new UICurve( Curve.RanB, "Random B" ),
                new UICurve( Curve.RanA, "Random A" ),
                new UICurve( Curve.RBri, "Random Bright" )
            };
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

            UICurve.DrawUnassignedCurves( Curves, id );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            UICurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();
    }
}
