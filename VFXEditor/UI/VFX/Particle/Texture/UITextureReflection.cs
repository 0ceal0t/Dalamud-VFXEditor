using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UITextureReflection : UIBase
    {
        public AVFXTextureReflection Tex;
        //============================

        public UITextureReflection(AVFXTextureReflection tex)
        {
            Tex = tex;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Tex.Assigned) { Assigned = false; return; }
            //====================
            Attributes.Add(new UICheckbox("Enabled", Tex.Enabled));
            Attributes.Add(new UICheckbox("Use Screen Copy", Tex.UseScreenCopy));
            Attributes.Add(new UIInt("Texture Index", Tex.TextureIdx));
            Attributes.Add(new UICombo<TextureFilterType>("Texture Filter", Tex.TextureFilter));
            Attributes.Add(new UICombo<TextureCalculateColor>("Calculate Color", Tex.TextureCalculateColor));
            Attributes.Add( new UICurve( Tex.Rate, "Rate" ) );
            Attributes.Add(new UICurve(Tex.RPow, "Power"));
        }

        // =========== DRAW =====================
        public override void Draw( string parentId )
        {
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.Selectable( "Texture Reflection" + parentId, selected == this ) )
            {
                selected = this;
            }
        }
        private void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Texture Reflection" + parentId ) )
            {
                Tex.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            string id = parentId + "/TR";
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Tex.Assigned = false;
                Init();
            }
            DrawAttrs( id );
        }
    }
}
