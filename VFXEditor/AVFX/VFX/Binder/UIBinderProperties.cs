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

        private static readonly Dictionary<int, string> BinderIds = new() {
            { 0, "Not working" },
            { 1, "Head" },
            { 3, "Left hand weapon" },
            { 4, "Right hand weapon" },
            { 6, "Right shoulder" },
            { 7, "Left shoulder" },
            { 8, "Right forearm" },
            { 9, "Left forearm" },
            { 10, "Right calves" },
            { 11, "Left calves" },
            { 16, "Front of character" },
            { 25, "Head" },
            { 26, "Head" },
            { 27, "Head" },
            { 28, "Cervical" },
            { 29, "Center of the character" },
            { 30, "Center of the character" },
            { 31, "Center of the character" },
            { 32, "Right hand" },
            { 33, "Left hand" },
            { 34, "Right foot" },
            { 35, "Left foot" },
            { 42, "Above character?" },
            { 43, "Head (near right eye)" },
            { 44, "Head (near left eye )" },
            { 77, "Monsters weapon" },
        };

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
            Parameters.Add( new UIIntCombo( "Bind Point Id", Prop.BindPointId, BinderIds ) );
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
