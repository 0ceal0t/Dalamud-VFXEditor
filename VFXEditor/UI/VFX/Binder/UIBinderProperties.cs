using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIBinderProperties : UIItem
    {
        public AVFXBinderProperty Prop;
        public string Name;
        //===================
        // TODO: Name

        public UIBinderProperties(string name, AVFXBinderProperty prop)
        {
            Prop = prop;
            Name = name;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Prop.Assigned) { Assigned = false; return; }
            //====================
            Attributes.Add(new UICombo<BindPoint>("Bind Point Type", Prop.BindPointType));
            Attributes.Add(new UICombo<BindTargetPoint>("Bind Target Point Type", Prop.BindTargetPointType));
            Attributes.Add(new UIInt("Bind Point Id", Prop.BindPointId));
            Attributes.Add(new UIInt("Generate Delay", Prop.GenerateDelay));
            Attributes.Add(new UIInt("Coord Update Frame", Prop.CoordUpdateFrame));
            Attributes.Add(new UICheckbox("Ring Enabled", Prop.RingEnable));
            Attributes.Add(new UIInt("Ring Progress Time", Prop.RingProgressTime));
            Attributes.Add(new UIFloat3("Ring Position", Prop.RingPositionX, Prop.RingPositionY, Prop.RingPositionZ));
            Attributes.Add(new UIFloat("Ring Radius", Prop.RingRadius));
            Attributes.Add(new UICurve3Axis(Prop.Position, "Position"));
        }

        // =========== DRAW =====================
        public override void DrawSelect( int idx, string parentId, ref UIItem selected )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.Selectable( Name + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        private void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ " + Name + parentId ) )
            {
                Prop.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/" + Name;
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Prop.Assigned = false;
                Init();
            }
            DrawAttrs( id );
        }

        public override string GetText( int idx ) {
            return Name;
        }
    }
}
