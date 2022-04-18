using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Binder;

namespace VFXEditor.Avfx.Vfx {
    public class UIBinder : UINode {
        public readonly AVFXBinder Binder;
        public readonly AvfxFile Main;

        private readonly UICombo<BinderType> Type;
        private readonly List<UIBinderProperties> Properties;
        private readonly UIItemSplitView<UIBinderProperties> PropSplit;
        private readonly UINodeGraphView NodeView;
        private readonly List<UIBase> Parameters;
        private UIData Data;

        public UIBinder( AvfxFile main, AVFXBinder binder, bool has_dependencies = false ) : base( UINodeGroup.BinderColor, has_dependencies ) {
            Binder = binder;
            Main = main;
            NodeView = new UINodeGraphView( this );
            Properties = new List<UIBinderProperties>();
            Type = new UICombo<BinderType>( "Type", Binder.BinderVariety, onChange: () => {
                Binder.SetType( Binder.BinderVariety.GetValue() );
                SetType();
            } );

            Parameters = new List<UIBase> {
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

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Binder";
            DrawRename( id );
            Type.Draw( id );
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
            NodeView.Draw( id );
            DrawList( Parameters, id );
            ImGui.EndChild();
        }

        private void DrawData( string id ) {
            ImGui.BeginChild( id );
            Data.Draw( id );
            ImGui.EndChild();
        }

        private void DrawProperties( string id ) {
            PropSplit.Draw( id );
        }

        public override string GetDefaultText() => $"Binder {Idx}({Binder.BinderVariety.GetValue()})";

        public override string GetWorkspaceId() => $"Bind{Idx}";

        public override void Write( BinaryWriter writer ) => Binder.Write( writer );

        public override bool IsAssigned() => true;
    }
}
