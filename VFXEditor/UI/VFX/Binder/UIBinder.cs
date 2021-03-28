using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VFXEditor.UI.VFX
{
    public class UIBinder : UINode
    {
        public AVFXBinder Binder;
        public UIBinderView View;
        //====================
        public UICombo<BinderType> Type;
        public UIData Data;
        public List<UIBinderProperties> Properties;
        //====================
        public UIItemSplitView<UIBinderProperties> PropSplit;
        public UINodeGraphView NodeView;

        public UIBinder(AVFXBinder binder, UIBinderView view, bool has_dependencies = false) : base(BinderColor, has_dependencies) {
            Binder = binder;
            View = view;
            NodeView = new UINodeGraphView( this );
            //=====================
            Properties = new List<UIBinderProperties>();
            //====================
            Type = new UICombo<BinderType>( "Type", Binder.BinderVariety, changeFunction: ChangeType );
            Attributes.Add( new UICheckbox( "Start to Global Direction", Binder.StartToGlobalDirection ) );
            Attributes.Add( new UICheckbox( "VFX Scale", Binder.VfxScaleEnabled ) );
            Attributes.Add( new UIFloat( "VFX Scale Bias", Binder.VfxScaleBias ) );
            Attributes.Add( new UICheckbox( "VFX Scale Depth Offset", Binder.VfxScaleDepthOffset ) );
            Attributes.Add( new UICheckbox( "VFX Scale Interpolation", Binder.VfxScaleInterpolation ) );
            Attributes.Add( new UICheckbox( "Transform Scale", Binder.TransformScale ) );
            Attributes.Add( new UICheckbox( "Transform Scale Depth Offset", Binder.TransformScaleDepthOffset ) );
            Attributes.Add( new UICheckbox( "Transform Scale Interpolation", Binder.TransformScaleInterpolation ) );
            Attributes.Add( new UICheckbox( "Following Target Orientation", Binder.FollowingTargetOrientation ) );
            Attributes.Add( new UICheckbox( "Document Scale Enabled", Binder.DocumentScaleEnabled ) );
            Attributes.Add( new UICheckbox( "Adjust to Screen", Binder.AdjustToScreenEnabled ) );
            Attributes.Add( new UIInt( "Life", Binder.Life ) );
            Attributes.Add( new UICombo<BinderRotation>( "Binder Rotation Type", Binder.BinderRotationType ) );
            //=============================
            Properties.Add( new UIBinderProperties( "Properties Start", Binder.PropStart ) );
            Properties.Add( new UIBinderProperties( "Properties 1", Binder.Prop1 ) );
            Properties.Add( new UIBinderProperties( "Properties 2", Binder.Prop2 ) );
            Properties.Add( new UIBinderProperties( "Properties Goal", Binder.PropGoal ) );
            //======================
            SetType();
            //======================
            PropSplit = new UIItemSplitView<UIBinderProperties>( Properties );
            HasDependencies = false; // if imported, all set now
        }
        public void SetType() {
            Data?.Cleanup();

            switch( Binder.BinderVariety.Value ) {
                case BinderType.Point:
                    Data = new UIBinderDataPoint( ( AVFXBinderDataPoint )Binder.Data );
                    break;
                case BinderType.Linear:
                    Data = new UIBinderDataLinear( ( AVFXBinderDataLinear )Binder.Data );
                    break;
                case BinderType.Spline:
                    Data = new UIBinderDataSpline( ( AVFXBinderDataSpline )Binder.Data );
                    break;
                case BinderType.Camera:
                    Data = new UIBinderDataCamera( ( AVFXBinderDataCamera )Binder.Data );
                    break;
                default:
                    Data = null;
                    break;
            }
        }
        public void ChangeType(LiteralEnum<BinderType> literal)
        {
            Binder.SetVariety(literal.Value);
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
        public void DrawProperties(string id )
        {
            PropSplit.Draw( id );
        }

        public override void DrawBody( string parentId ) {
            string id = parentId + "/Binder";
            Type.Draw( id );
            // =====================
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

        public override string GetText() {
            return "Binder " + Idx + "(" + Binder.BinderVariety.stringValue() + ")";
        }

        public override byte[] toBytes() {
            return Binder.toAVFX().toBytes();
        }
    }
}
