using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffector : UIItem
    {
        public AVFXEffector Effector;
        public UIEffectorView View;
        //========================
        public UICombo<EffectorType> Type;
        public UIBase Data;

        public UIEffector(AVFXEffector effector, UIEffectorView view)
        {
            Effector = effector;
            View = view;
            Init();
        }
        public override void Init()
        {
            base.Init();
            //======================
            Type = new UICombo<EffectorType>("Type", Effector.EffectorVariety, changeFunction: ChangeType);
            Attributes.Add(new UICombo<RotationOrder>("Rotation Order", Effector.RotationOrder));
            Attributes.Add(new UICombo<CoordComputeOrder>("Coordinate Compute Order", Effector.CoordComputeOrder));
            Attributes.Add(new UICheckbox("Affect Other VFX", Effector.AffectOtherVfx));
            Attributes.Add(new UICheckbox("Affect Game", Effector.AffectGame));
            Attributes.Add(new UIInt("Loop Start", Effector.LoopPointStart));
            Attributes.Add(new UIInt("Loop End", Effector.LoopPointEnd));
            //=======================
            switch (Effector.EffectorVariety.Value)
            {
                case EffectorType.PointLight:
                    Data = new UIEffectorDataPointLight((AVFXEffectorDataPointLight)Effector.Data);
                    break;
                case EffectorType.RadialBlur:
                    Data = new UIEffectorDataRadialBlur((AVFXEffectorDataRadialBlur)Effector.Data);
                    break;
                case EffectorType.CameraQuake:
                    Data = new UIEffectorDataCameraQuake((AVFXEffectorDataCameraQuake)Effector.Data);
                    break;
                case EffectorType.DirectionalLight:
                    Data = new UIEffectorDataDirectionalLight( ( AVFXEffectorDataDirectionalLight )Effector.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType(LiteralEnum<EffectorType> literal)
        {
            Effector.SetVariety(literal.Value);
            Init();
        }

        private void DrawParameters( string id )
        {
            ImGui.BeginChild( id );
            DrawAttrs( id );
            ImGui.EndChild();
        }
        private void DrawData( string id )
        {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Effector";
            Type.Draw( id );
            //==========================
            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id ) ) {
                    DrawParameters( id + "/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( "Data" + id ) ) {
                    DrawData( id + "/Data" );
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }

        public override void DrawSelect( int idx, string parentId, ref UIItem selected ) {
        }

        public override string GetText( int idx ) {
            return "Effector " + idx + "(" + Effector.EffectorVariety.stringValue() + ")";
        }
    }
}
