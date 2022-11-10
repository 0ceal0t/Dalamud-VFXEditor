using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Curve;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiCurve3Axis : UiAssignableItem {
        public readonly AVFXCurve3Axis Curve;
        public readonly string Name;
        public readonly bool Locked;
        public readonly UiCombo<AxisConnect> AxisConnectSelect;
        public readonly UiCombo<RandomType> AxisConnectRandomSelect;
        private readonly List<UiCurve> Curves;

        public UiCurve3Axis( AVFXCurve3Axis curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;

            AxisConnectSelect = new UiCombo<AxisConnect>( "Axis Connect", Curve.AxisConnectType );
            AxisConnectRandomSelect = new UiCombo<RandomType>( "Axis Connect Random", Curve.AxisConnectRandomType );

            Curves = new List<UiCurve> {
                new UiCurve( Curve.X, "X" ),
                new UiCurve( Curve.Y, "Y" ),
                new UiCurve( Curve.Z, "Z" ),
                new UiCurve( Curve.RX, "Random X" ),
                new UiCurve( Curve.RY, "Random Y" ),
                new UiCurve( Curve.RZ, "Random Z" )
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
