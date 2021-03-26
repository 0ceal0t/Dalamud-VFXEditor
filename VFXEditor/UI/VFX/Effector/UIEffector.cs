using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIEffector : UINode {
        public AVFXEffector Effector;
        public UIEffectorView View;
        //========================
        public UICombo<EffectorType> Type;
        public UIData Data;
        public UINodeGraphView NodeView;

        public UIEffector(AVFXEffector effector, UIEffectorView view, bool imported = false )
        {
            Effector = effector;
            View = view;
            _Color = EffectorColor;
            Imported = imported;
            NodeView = new UINodeGraphView( this );
            //======================
            Type = new UICombo<EffectorType>( "Type", Effector.EffectorVariety, changeFunction: ChangeType );
            Attributes.Add( new UICombo<RotationOrder>( "Rotation Order", Effector.RotationOrder ) );
            Attributes.Add( new UICombo<CoordComputeOrder>( "Coordinate Compute Order", Effector.CoordComputeOrder ) );
            Attributes.Add( new UICheckbox( "Affect Other VFX", Effector.AffectOtherVfx ) );
            Attributes.Add( new UICheckbox( "Affect Game", Effector.AffectGame ) );
            Attributes.Add( new UIInt( "Loop Start", Effector.LoopPointStart ) );
            Attributes.Add( new UIInt( "Loop End", Effector.LoopPointEnd ) );
            //=======================
            SetType();
        }
        
        public void SetType() {
            Data?.Cleanup();

            switch( Effector.EffectorVariety.Value ) {
                case EffectorType.PointLight:
                    Data = new UIEffectorDataPointLight( ( AVFXEffectorDataPointLight )Effector.Data );
                    break;
                case EffectorType.RadialBlur:
                    Data = new UIEffectorDataRadialBlur( ( AVFXEffectorDataRadialBlur )Effector.Data );
                    break;
                case EffectorType.CameraQuake:
                    Data = new UIEffectorDataCameraQuake( ( AVFXEffectorDataCameraQuake )Effector.Data );
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
            SetType();
        }

        private void DrawParameters( string id )
        {
            ImGui.BeginChild( id );
            NodeView.Draw( id );
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

        public override string GetText() {
            return "Effector " + Idx + "(" + Effector.EffectorVariety.stringValue() + ")";
        }

        public override byte[] toBytes() {
            return Effector.toAVFX().toBytes();
        }
    }
}
