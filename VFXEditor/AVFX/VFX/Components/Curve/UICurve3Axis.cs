using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Curve;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public class UICurve3Axis : UIAssignableItem {
        public readonly AVFXCurve3Axis Curve;
        public readonly string Name;
        public readonly bool Locked;
        public readonly UICombo<AxisConnect> AxisConnectSelect;
        public readonly UICombo<RandomType> AxisConnectRandomSelect;
        private readonly List<UICurve> Curves;

        public UICurve3Axis( AVFXCurve3Axis curve, string name, bool locked = false ) {
            Curve = curve;
            Name = name;
            Locked = locked;

            AxisConnectSelect = new UICombo<AxisConnect>( "Axis Connect", Curve.AxisConnectType );
            AxisConnectRandomSelect = new UICombo<RandomType>( "Axis Connect Random", Curve.AxisConnectRandomType );

            Curves = new List<UICurve> {
                new UICurve( Curve.X, "X" ),
                new UICurve( Curve.Y, "Y" ),
                new UICurve( Curve.Z, "Z" ),
                new UICurve( Curve.RX, "Random X" ),
                new UICurve( Curve.RY, "Random Y" ),
                new UICurve( Curve.RZ, "Random Z" )
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

            AxisConnectSelect.DrawInline( id );
            AxisConnectRandomSelect.DrawInline( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            UICurve.DrawAssignedCurves( Curves, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Curve.IsAssigned();
    }
}
