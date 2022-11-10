using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCurveColor : UiAssignableItem {
        public readonly AVFXCurveColor Curve;
        public readonly string Name;
        public readonly bool Locked;
        private readonly List<UiCurve> Curves;

        public UiCurveColor( AVFXCurveColor curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;

            Curves = new List<UiCurve> {
                new UiCurve( Curve.RGB, "RGB", color: true ),
                new UiCurve( Curve.A, "A" ),
                new UiCurve( Curve.SclR, "Scale R" ),
                new UiCurve( Curve.SclG, "Scale G" ),
                new UiCurve( Curve.SclB, "Scale B" ),
                new UiCurve( Curve.SclA, "Scale A" ),
                new UiCurve( Curve.Bri, "Bright" ),
                new UiCurve( Curve.RanR, "Random R" ),
                new UiCurve( Curve.RanG, "Random G" ),
                new UiCurve( Curve.RanB, "Random B" ),
                new UiCurve( Curve.RanA, "Random A" ),
                new UiCurve( Curve.RBri, "Random Bright" )
            };
        }

        public override void DrawUnassigned( string parentId ) {
            IUiBase.DrawAddButtonRecurse( Curve, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked && IUiBase.DrawRemoveButton( Curve, Name, id ) ) return;

            UiCurve.DrawUnassignedCurves( Curves, id );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            UiCurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();
    }
}
