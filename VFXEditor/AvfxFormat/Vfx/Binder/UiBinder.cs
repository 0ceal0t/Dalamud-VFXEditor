using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiBinder : UiNode {
        public readonly AVFXBinder Binder;

        private readonly UiCombo<BinderType> Type;
        private readonly List<UiBinderProperties> Properties;
        private readonly UiItemSplitView<UiBinderProperties> PropSplit;
        private readonly UiNodeGraphView NodeView;
        private readonly List<IUiBase> Parameters;
        private UiData Data;

        public UiBinder( AVFXBinder binder, bool hasDependencies = false ) : base( UiNodeGroup.BinderColor, hasDependencies ) {
            Binder = binder;
            NodeView = new UiNodeGraphView( this );
            Properties = new List<UiBinderProperties>();
            Type = new UiCombo<BinderType>( "Type", Binder.BinderVariety, extraCommand: () => {
                //Binder.SetType( Binder.BinderVariety.GetValue() );
                //SetType();
                // TODO
                return null;
            } );

            Parameters = new List<IUiBase> {
                new UiCheckbox( "Start to Global Direction", Binder.StartToGlobalDirection ),
                new UiCheckbox( "VFX Scale", Binder.VfxScaleEnabled ),
                new UiFloat( "VFX Scale Bias", Binder.VfxScaleBias ),
                new UiCheckbox( "VFX Scale Depth Offset", Binder.VfxScaleDepthOffset ),
                new UiCheckbox( "VFX Scale Interpolation", Binder.VfxScaleInterpolation ),
                new UiCheckbox( "Transform Scale", Binder.TransformScale ),
                new UiCheckbox( "Transform Scale Depth Offset", Binder.TransformScaleDepthOffset ),
                new UiCheckbox( "Transform Scale Interpolation", Binder.TransformScaleInterpolation ),
                new UiCheckbox( "Following Target Orientation", Binder.FollowingTargetOrientation ),
                new UiCheckbox( "Document Scale Enabled", Binder.DocumentScaleEnabled ),
                new UiCheckbox( "Adjust to Screen", Binder.AdjustToScreenEnabled ),
                new UiCheckbox( "BET (Unknown)", Binder.BET_Unknown ),
                new UiInt( "Life", Binder.Life ),
                new UiCombo<BinderRotation>( "Binder Rotation Type", Binder.BinderRotationType )
            };

            Properties.Add( new UiBinderProperties( "Properties Start", Binder.PropStart ) );
            Properties.Add( new UiBinderProperties( "Properties 1", Binder.Prop1 ) );
            Properties.Add( new UiBinderProperties( "Properties 2", Binder.Prop2 ) );
            Properties.Add( new UiBinderProperties( "Properties Goal", Binder.PropGoal ) );

            SetType();

            PropSplit = new UiItemSplitView<UiBinderProperties>( Properties );
            HasDependencies = false; // if imported, all set now
        }

        private void SetType() {
            Data?.Disable();
            Data = Binder.BinderVariety.GetValue() switch {
                BinderType.Point => new UiBinderDataPoint( ( AVFXBinderDataPoint )Binder.Data ),
                BinderType.Linear => new UiBinderDataLinear( ( AVFXBinderDataLinear )Binder.Data ),
                BinderType.Spline => new UiBinderDataSpline( ( AVFXBinderDataSpline )Binder.Data ),
                BinderType.Camera => new UiBinderDataCamera( ( AVFXBinderDataCamera )Binder.Data ),
                _ => null
            };
        }

        public override void DrawInline( string parentId ) {
            var id = parentId + "/Binder";
            DrawRename( id );
            Type.DrawInline( id );
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( ImGui.BeginTabBar( id + "Tabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton ) ) {
                if( ImGui.BeginTabItem( "Parameters" + id + "/Tab" ) ) {
                    DrawParameters( id + "/Param" );
                    ImGui.EndTabItem();
                }
                if( Data != null && ImGui.BeginTabItem( "Data" + id + "/Tab" ) ) {
                    DrawData( id + "/Data" );
                    ImGui.EndTabItem();
                }
                if( ImGui.BeginTabItem( "Properties" + id + "/Tab" ) ) {
                    DrawProperties( id + "/Prop" );
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

        private void DrawProperties( string id ) {
            PropSplit.DrawInline( id );
        }

        public override string GetDefaultText() => $"Binder {Idx}({Binder.BinderVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Bind{Idx}";

        public override void Write( BinaryWriter writer ) => Binder.Write( writer );
    }
}
