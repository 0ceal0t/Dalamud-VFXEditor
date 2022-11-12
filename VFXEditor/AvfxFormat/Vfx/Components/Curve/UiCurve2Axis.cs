using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCurve2Axis : UiAssignableItem {
        public readonly AVFXCurve2Axis Curve;
        public readonly string Name;
        public readonly bool Locked;
        public readonly UiCombo<AxisConnect> AxisConnectSelect;
        public readonly UiCombo<RandomType> AxisConnectRandomSelect;
        public readonly UiCurve X;
        public readonly UiCurve Y;
        private readonly List<UiCurve> Curves;

        public UiCurve2Axis( AVFXCurve2Axis curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;

            AxisConnectSelect = new UiCombo<AxisConnect>( "Axis Connect", Curve.AxisConnectType );
            AxisConnectRandomSelect = new UiCombo<RandomType>( "Axis Connect Random", Curve.AxisConnectRandomType );

            Curves = new() {
                ( X = new UiCurve( Curve.X, "X" ) ),
                ( Y = new UiCurve( Curve.Y, "Y" ) ),
                new UiCurve( Curve.RX, "Random X" ),
                new UiCurve( Curve.RY, "Random Y" )
            };
        }

        public override void DrawUnassigned( string parentId ) {
            IUiBase.DrawAddButtonRecurse( Curve, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( !Locked && IUiBase.DrawRemoveButton( Curve, Name, id ) ) return;

            UiCurve.DrawUnassignedCurves( Curves, id );

            AxisConnectSelect.DrawInline( id );
            AxisConnectRandomSelect.DrawInline( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            UiCurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();
    }
}
