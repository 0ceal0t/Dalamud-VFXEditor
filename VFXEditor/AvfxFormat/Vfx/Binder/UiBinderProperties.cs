using ImGuiNET;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Binder;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinderProperties : UiAssignableItem {
        private readonly AVFXBinderProperty Prop;
        private readonly string Name;
        private readonly UiParameters Parameters;
        private readonly List<UiItem> Tabs;

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

        public UiBinderProperties( string name, AVFXBinderProperty prop ) {
            Prop = prop;
            Name = name;

            Tabs = new List<UiItem> {
                ( Parameters = new UiParameters( "Parameters" ) ),
                new UiCurve3Axis( Prop.Position, "Position", locked:true )
            };
            Parameters.Add( new UiCombo<BindPoint>( "Bind Point Type", Prop.BindPointType ) );
            Parameters.Add( new UiCombo<BindTargetPoint>( "Bind Target Point Type", Prop.BindTargetPointType ) );
            Parameters.Add( new UiString( "Name", Prop.BinderName, showRemoveButton: true ) );
            Parameters.Add( new UiIntCombo( "Bind Point Id", Prop.BindPointId, BinderIds ) );
            Parameters.Add( new UiInt( "Generate Delay", Prop.GenerateDelay ) );
            Parameters.Add( new UiInt( "Coord Update Frame", Prop.CoordUpdateFrame ) );
            Parameters.Add( new UiCheckbox( "Ring Enabled", Prop.RingEnable ) );
            Parameters.Add( new UiInt( "Ring Progress Time", Prop.RingProgressTime ) );
            Parameters.Add( new UiFloat3( "Ring Position", Prop.RingPositionX, Prop.RingPositionY, Prop.RingPositionZ ) );
            Parameters.Add( new UiFloat( "Ring Radius", Prop.RingRadius ) );
        }

        public override void DrawUnassigned( string parentId ) {
            IUiBase.DrawAddButtonRecurse( Prop, Name, parentId );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/" + Name;
            IUiBase.DrawRemoveButton( Prop, Name, id );

            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => Name;

        public override bool IsAssigned() => Prop.IsAssigned();
    }
}
