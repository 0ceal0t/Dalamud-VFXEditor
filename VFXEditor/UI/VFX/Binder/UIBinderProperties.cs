using AVFXLib.Models;
using ImGuiNET;
using System.Collections.Generic;

using VFXEditor.Helper;

namespace VFXEditor.UI.Vfx {
    public class UIBinderProperties : UIItem {
        public AVFXBinderProperty Prop;
        public string Name;
        //===================
        // TODO: Name
        public UIParameters Parameters;
        public UICurve3Axis Position;
        public List<UIItem> Tabs;

        public UIBinderProperties( string name, AVFXBinderProperty prop ) {
            Prop = prop;
            Name = name;
            Init();
        }
        public override void Init() {
            base.Init();
            if( !Prop.Assigned ) { Assigned = false; return; }
            //====================
            Tabs = new List<UIItem> {
                ( Parameters = new UIParameters( "Parameters" ) ),
                ( Position = new UICurve3Axis( Prop.Position, "Position" ) )
            };
            Parameters.Add( new UICombo<BindPoint>( "Bind Point Type", Prop.BindPointType ) );
            Parameters.Add( new UICombo<BindTargetPoint>( "Bind Target Point Type", Prop.BindTargetPointType ) );
            Parameters.Add( new UIInt( "Bind Point Id", Prop.BindPointId ) );
            Parameters.Add( new UIInt( "Generate Delay", Prop.GenerateDelay ) );
            Parameters.Add( new UIInt( "Coord Update Frame", Prop.CoordUpdateFrame ) );
            Parameters.Add( new UICheckbox( "Ring Enabled", Prop.RingEnable ) );
            Parameters.Add( new UIInt( "Ring Progress Time", Prop.RingProgressTime ) );
            Parameters.Add( new UIFloat3( "Ring Position", Prop.RingPositionX, Prop.RingPositionY, Prop.RingPositionZ ) );
            Parameters.Add( new UIFloat( "Ring Radius", Prop.RingRadius ) );
        }

        // =========== DRAW =====================
        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ " + Name + parentId ) ) {
                Prop.ToDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId ) {
            var id = parentId + "/" + Name;
            if( UiHelper.RemoveButton( "Delete " + Name + id, small: true ) ) {
                Prop.Assigned = false;
                Init();
            }
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() {
            return Name;
        }
    }
}
