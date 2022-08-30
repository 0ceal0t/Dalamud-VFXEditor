using ImGuiNET;
using System.Collections.Generic;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Binder;
using VFXEditor.Utils;

namespace VFXEditor.AVFX.VFX {
    public class UIBinderProperties : UIAssignableItem {
        private readonly AVFXBinderProperty Prop;
        private readonly string Name;
        private readonly UIParameters Parameters;
        private readonly List<UIItem> Tabs;

        public UIBinderProperties( string name, AVFXBinderProperty prop ) {
            Prop = prop;
            Name = name;

            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) ),
                new UICurve3Axis( Prop.Position, "Position", locked:true )
            };
            Parameters.Add( new UICombo<BindPoint>( "Bind Point Type", Prop.BindPointType ) );
            Parameters.Add( new UICombo<BindTargetPoint>( "Bind Target Point Type", Prop.BindTargetPointType ) );
            Parameters.Add( new UIString( "Name", Prop.BinderName, showRemoveButton: true ) );
            Parameters.Add( new UIInt( "Bind Point Id", Prop.BindPointId ) );
            Parameters.Add( new UIInt( "Generate Delay", Prop.GenerateDelay ) );
            Parameters.Add( new UIInt( "Coord Update Frame", Prop.CoordUpdateFrame ) );
            Parameters.Add( new UICheckbox( "Ring Enabled", Prop.RingEnable ) );
            Parameters.Add( new UIInt( "Ring Progress Time", Prop.RingProgressTime ) );
            Parameters.Add( new UIFloat3( "Ring Position", Prop.RingPositionX, Prop.RingPositionY, Prop.RingPositionZ ) );
            Parameters.Add( new UIFloat( "Ring Radius", Prop.RingRadius ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                AVFXBase.RecurseAssigned( Prop, true );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            if( UiUtils.RemoveButton( "Delete " + Name + id, small: true ) ) {
                Prop.SetAssigned( false );
            }
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Prop.IsAssigned();
    }
}
