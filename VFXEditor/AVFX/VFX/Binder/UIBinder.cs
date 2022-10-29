using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Binder;

namespace VfxEditor.AVFX.VFX {
    public class UIBinder : UINode {
        public readonly AVFXBinder Binder;

        private readonly UICombo<BinderType> Type;
        private readonly List<UIBinderProperties> Properties;
        private readonly UIItemSplitView<UIBinderProperties> PropSplit;
        private readonly UINodeGraphView NodeView;
        private readonly List<IUIBase> Parameters;
        private UIData Data;

        public UIBinder( AVFXBinder binder, bool hasDependencies = false ) : base( UINodeGroup.BinderColor, hasDependencies ) {
            Binder = binder;
            NodeView = new UINodeGraphView( this );
            Properties = new List<UIBinderProperties>();
            Type = new UICombo<BinderType>( "Type", Binder.BinderVariety, onChange: () => {
                Binder.SetType( Binder.BinderVariety.GetValue() );
                SetType();
            } );

            Parameters = new List<IUIBase> {
                new UICheckbox( "Start to Global Direction", Binder.StartToGlobalDirection ),
                new UICheckbox( "VFX Scale", Binder.VfxScaleEnabled ),
                new UIFloat( "VFX Scale Bias", Binder.VfxScaleBias ),
                new UICheckbox( "VFX Scale Depth Offset", Binder.VfxScaleDepthOffset ),
                new UICheckbox( "VFX Scale Interpolation", Binder.VfxScaleInterpolation ),
                new UICheckbox( "Transform Scale", Binder.TransformScale ),
                new UICheckbox( "Transform Scale Depth Offset", Binder.TransformScaleDepthOffset ),
                new UICheckbox( "Transform Scale Interpolation", Binder.TransformScaleInterpolation ),
                new UICheckbox( "Following Target Orientation", Binder.FollowingTargetOrientation ),
                new UICheckbox( "Document Scale Enabled", Binder.DocumentScaleEnabled ),
                new UICheckbox( "Adjust to Screen", Binder.AdjustToScreenEnabled ),
                new UICheckbox( "BET (Unknown)", Binder.BET_Unknown ),
                new UIInt( "Life", Binder.Life ),
                new UICombo<BinderRotation>( "Binder Rotation Type", Binder.BinderRotationType )
            };

            Properties.Add( new UIBinderProperties( "Properties Start", Binder.PropStart ) );
            Properties.Add( new UIBinderProperties( "Properties 1", Binder.Prop1 ) );
            Properties.Add( new UIBinderProperties( "Properties 2", Binder.Prop2 ) );
            Properties.Add( new UIBinderProperties( "Properties Goal", Binder.PropGoal ) );

            SetType();

            PropSplit = new UIItemSplitView<UIBinderProperties>( Properties );
            HasDependencies = false; // if imported, all set now
        }

        private void SetType() {
            Data?.Dispose();
            Data = Binder.BinderVariety.GetValue() switch {
                BinderType.Point => new UIBinderDataPoint( ( AVFXBinderDataPoint )Binder.Data ),
                BinderType.Linear => new UIBinderDataLinear( ( AVFXBinderDataLinear )Binder.Data ),
                BinderType.Spline => new UIBinderDataSpline( ( AVFXBinderDataSpline )Binder.Data ),
                BinderType.Camera => new UIBinderDataCamera( ( AVFXBinderDataCamera )Binder.Data ),
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
            IUIBase.DrawList( Parameters, id );
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
