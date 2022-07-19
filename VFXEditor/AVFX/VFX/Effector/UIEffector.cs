using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Effector;

namespace VFXEditor.AVFX.VFX {
    public class UIEffector : UINode {
        public readonly AVFXEffector Effector;
        public readonly UICombo<EffectorType> Type;
        public readonly UINodeGraphView NodeView;
        private readonly List<UIBase> Parameters;

        public UIData Data;

        public UIEffector( AVFXEffector effector, bool hasDependencies = false ) : base( UINodeGroup.EffectorColor, hasDependencies ) {
            Effector = effector;
            NodeView = new UINodeGraphView( this );

            Type = new UICombo<EffectorType>( "Type", Effector.EffectorVariety, onChange: () => {
                Effector.SetType( Effector.EffectorVariety.GetValue() );
                SetType();
            } );
            Parameters = new List<UIBase> {
                new UICombo<RotationOrder>( "Rotation Order", Effector.RotationOrder ),
                new UICombo<CoordComputeOrder>( "Coordinate Compute Order", Effector.CoordComputeOrder ),
                new UICheckbox( "Affect Other VFX", Effector.AffectOtherVfx ),
                new UICheckbox( "Affect Game", Effector.AffectGame ),
                new UIInt( "Loop Start", Effector.LoopPointStart ),
                new UIInt( "Loop End", Effector.LoopPointEnd )
            };

            SetType();
            HasDependencies = false; // if imported, all set now
        }

        private void SetType() {
            Data?.Dispose();
            Data = Effector.EffectorVariety.GetValue() switch {
                EffectorType.PointLight => new UIEffectorDataPointLight( ( AVFXEffectorDataPointLight )Effector.Data ),
                EffectorType.RadialBlur => new UIEffectorDataRadialBlur( ( AVFXEffectorDataRadialBlur )Effector.Data ),
                EffectorType.CameraQuake => new UIEffectorDataCameraQuake( ( AVFXEffectorDataCameraQuake )Effector.Data ),
                EffectorType.DirectionalLight => new UIEffectorDataDirectionalLight( ( AVFXEffectorDataDirectionalLight )Effector.Data ),
                _ => null
            };
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Effector";
            DrawRename( id );
            Type.Draw( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

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

        private void DrawParameters( string id ) {
            ImGui.BeginChild( id );
            NodeView.Draw( id );
            DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        public override string GetDefaultText() => $"Effector {Idx}({Effector.EffectorVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Effct{Idx}";

        public override void Write( BinaryWriter writer ) => Effector.Write( writer );
    }
}
