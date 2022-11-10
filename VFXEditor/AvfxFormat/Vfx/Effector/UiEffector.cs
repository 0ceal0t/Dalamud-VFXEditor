using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Effector;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEffector : UiNode {
        public readonly AVFXEffector Effector;
        public readonly UiCombo<EffectorType> Type;
        private readonly UiNodeGraphView NodeView;
        private readonly List<IUiBase> Parameters;

        public UiData Data;

        public UiEffector( AVFXEffector effector, bool hasDependencies = false ) : base( UiNodeGroup.EffectorColor, hasDependencies ) {
            Effector = effector;
            NodeView = new UiNodeGraphView( this );

            Type = new UiCombo<EffectorType>( "Type", Effector.EffectorVariety, extraCommand: () => {
                return new UiEffectorDataExtraCommand( this );
            } );
            Parameters = new List<IUiBase> {
                new UiCombo<RotationOrder>( "Rotation Order", Effector.RotationOrder ),
                new UiCombo<CoordComputeOrder>( "Coordinate Compute Order", Effector.CoordComputeOrder ),
                new UiCheckbox( "Affect Other VFX", Effector.AffectOtherVfx ),
                new UiCheckbox( "Affect Game", Effector.AffectGame ),
                new UiInt( "Loop Start", Effector.LoopPointStart ),
                new UiInt( "Loop End", Effector.LoopPointEnd )
            };

            UpdateDataType();
            HasDependencies = false; // if imported, all set now
        }

        public void UpdateDataType() {
            Data?.Disable();
            Data = Effector.EffectorVariety.GetValue() switch {
                EffectorType.PointLight => new UiEffectorDataPointLight( ( AVFXEffectorDataPointLight )Effector.Data ),
                EffectorType.RadialBlur => new UiEffectorDataRadialBlur( ( AVFXEffectorDataRadialBlur )Effector.Data ),
                EffectorType.CameraQuake => new UiEffectorDataCameraQuake( ( AVFXEffectorDataCameraQuake )Effector.Data ),
                EffectorType.DirectionalLight => new UiEffectorDataDirectionalLight( ( AVFXEffectorDataDirectionalLight )Effector.Data ),
                _ => null
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Effector";
            DrawRename( id );
            Type.DrawInline( id );
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
            NodeView.DrawInline( id );
            IUiBase.DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.DrawInline( id );
            ImGui.EndChild();
        }

        public override string GetDefaultText() => $"Effector {Idx}({Effector.EffectorVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Effct{Idx}";

        public override void Write( BinaryWriter writer ) => Effector.Write( writer );
    }
}
