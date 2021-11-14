using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx
{
    public class UIEffector : UINode {
        public AVFXEffector Effector;
        public AvfxFile Main;
        //========================
        public UICombo<EffectorType> Type;
        public UIData Data;
        public UINodeGraphView NodeView;

        public UIEffector(AvfxFile main, AVFXEffector effector, bool has_dependencies = false ) : base( UINodeGroup.EffectorColor, has_dependencies ) {
            Effector = effector;
            Main = main;
            NodeView = new UINodeGraphView( this );
            //======================
            Type = new UICombo<EffectorType>( "Type", Effector.EffectorVariety, onChange: () => {
                Effector.SetVariety( Effector.EffectorVariety.Value );
                SetType();
            } );
            Attributes.Add( new UICombo<RotationOrder>( "Rotation Order", Effector.RotationOrder ) );
            Attributes.Add( new UICombo<CoordComputeOrder>( "Coordinate Compute Order", Effector.CoordComputeOrder ) );
            Attributes.Add( new UICheckbox( "Affect Other VFX", Effector.AffectOtherVfx ) );
            Attributes.Add( new UICheckbox( "Affect Game", Effector.AffectGame ) );
            Attributes.Add( new UIInt( "Loop Start", Effector.LoopPointStart ) );
            Attributes.Add( new UIInt( "Loop End", Effector.LoopPointEnd ) );
            //=======================
            SetType();
            HasDependencies = false; // if imported, all set now
        }
        
        public void SetType() {
            Data?.Dispose();
            Data = Effector.EffectorVariety.Value switch {
                EffectorType.PointLight => new UIEffectorDataPointLight( ( AVFXEffectorDataPointLight )Effector.Data ),
                EffectorType.RadialBlur => new UIEffectorDataRadialBlur( ( AVFXEffectorDataRadialBlur )Effector.Data ),
                EffectorType.CameraQuake => new UIEffectorDataCameraQuake( ( AVFXEffectorDataCameraQuake )Effector.Data ),
                EffectorType.DirectionalLight => new UIEffectorDataDirectionalLight( ( AVFXEffectorDataDirectionalLight )Effector.Data ),
                _ => null
            };
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
            var id = parentId + "/Effector";
            DrawRename( id );
            Type.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
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

        public override string GetDefaultText() {
            return "Effector " + Idx + "(" + Effector.EffectorVariety.StringValue() + ")";
        }

        public override string GetWorkspaceId() {
            return $"Effct{Idx}";
        }

        public override byte[] ToBytes() {
            return Effector.ToAVFX().ToBytes();
        }
    }
}
