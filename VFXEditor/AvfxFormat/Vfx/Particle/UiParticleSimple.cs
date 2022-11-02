using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleSimple : UiAssignableItem {
        public readonly AVFXParticleSimple Simple;
        public readonly UiParticle Particle;

        public UiNodeSelect<UiModel> InjectionModelSelect;
        public UiNodeSelect<UiModel> InjectionVertexModelSelect;

        public readonly List<UiItem> Tabs;
        public readonly UiParameters Creation;
        public readonly UiParameters Animation;
        public readonly UiParameters Texture;
        public readonly UiParameters Color;

        public UiParticleSimple( AVFXParticleSimple simple, UiParticle particle ) {
            Simple = simple;
            Particle = particle;

            Tabs = new List<UiItem> {
                ( Creation = new UiParameters( "Creation" ) )
            };

            if( IsAssigned() ) {
                Creation.Add( InjectionModelSelect = new UiNodeSelect<UiModel>( Particle, "Injection Model", Particle.NodeGroups.Models, Simple.InjectionModelIdx ) );
                Creation.Add( InjectionVertexModelSelect = new UiNodeSelect<UiModel>( Particle, "Injection Vertex Bind Model", Particle.NodeGroups.Models, Simple.InjectionVertexBindModelIdx ) );
            }

            Creation.Add( new UiInt( "Injection Position Type", Simple.InjectionPositionType ) );
            Creation.Add( new UiInt( "Injection Direction Type", Simple.InjectionDirectionType ) );
            Creation.Add( new UiInt( "Base Direction Type", Simple.BaseDirectionType ) );
            Creation.Add( new UiInt( "Create Count", Simple.CreateCount ) );
            Creation.Add( new UiFloat3( "Create Area", Simple.CreateAreaX, Simple.CreateAreaY, Simple.CreateAreaZ ) );
            Creation.Add( new UiFloat3( "Coord Accuracy", Simple.CoordAccuracyX, Simple.CoordAccuracyY, Simple.CoordAccuracyZ ) );
            Creation.Add( new UiFloat3( "Coord Gra", Simple.CoordGraX, Simple.CoordGraY, Simple.CoordGraZ ) );
            Creation.Add( new UiFloat( "Injection Radial Direction 0", Simple.InjectionRadialDir0 ) );
            Creation.Add( new UiFloat( "Injection Radial Direction 1", Simple.InjectionRadialDir1 ) );
            Creation.Add( new UiFloat( "Pivot X", Simple.PivotX ) );
            Creation.Add( new UiFloat( "Pivot Y", Simple.PivotY ) );
            Creation.Add( new UiInt( "Block Number", Simple.BlockNum ) );
            Creation.Add( new UiFloat( "Line Length Minimum", Simple.LineLengthMin ) );
            Creation.Add( new UiFloat( "Line Length Maximum", Simple.LineLengthMax ) );
            Creation.Add( new UiInt( "Create Interval", Simple.CreateIntervalVal ) );
            Creation.Add( new UiInt( "Create Interval Random", Simple.CreateIntervalRandom ) );
            Creation.Add( new UiInt( "Create Interval Count", Simple.CreateIntervalCount ) );
            Creation.Add( new UiInt( "Create Interval Life", Simple.CreateIntervalLife ) );
            Creation.Add( new UiInt( "Create New After Delete", Simple.CreateNewAfterDelete ) );

            Tabs.Add( Animation = new UiParameters( "Animation" ) );
            Animation.Add( new UiFloat2( "Scale Start", Simple.ScaleXStart, Simple.ScaleYStart ) );
            Animation.Add( new UiFloat2( "Scale End", Simple.ScaleXEnd, Simple.ScaleYEnd ) );
            Animation.Add( new UiFloat( "Scale Curve", Simple.ScaleCurve ) );
            Animation.Add( new UiFloat2( "Scale X Random", Simple.ScaleRandX0, Simple.ScaleRandX1 ) );
            Animation.Add( new UiFloat2( "Scale Y Random", Simple.ScaleRandY0, Simple.ScaleRandY1 ) );
            Animation.Add( new UiFloat3( "Rotation Add", Simple.RotXAdd, Simple.RotYAdd, Simple.RotZAdd ) );
            Animation.Add( new UiFloat3( "Rotation Base", Simple.RotXBase, Simple.RotYBase, Simple.RotZBase ) );
            Animation.Add( new UiFloat3( "Rotation Velocity", Simple.RotXVel, Simple.RotYVel, Simple.RotZVel ) );
            Animation.Add( new UiFloat( "Minimum Velocity", Simple.VelMin ) );
            Animation.Add( new UiFloat( "Maximum Velocity", Simple.VelMax ) );
            Animation.Add( new UiFloat( "Velocity Flattery Rate", Simple.VelFlatteryRate ) );
            Animation.Add( new UiFloat( "Velocity Flattery Speed", Simple.VelFlatterySpeed ) );
            Animation.Add( new UiInt( "Scale Random Link", Simple.ScaleRandomLink ) );
            Animation.Add( new UiInt( "Bind Parent", Simple.BindParent ) );
            Animation.Add( new UiInt( "Scale By Parent", Simple.ScaleByParent ) );
            Animation.Add( new UiInt( "Polyline Tag", Simple.PolyLineTag ) );

            Tabs.Add( Texture = new UiParameters( "Texture" ) );
            Texture.Add( new UiInt( "UV Cell U", Simple.UvCellU ) );
            Texture.Add( new UiInt( "UV Cell V", Simple.UvCellV ) );
            Texture.Add( new UiInt( "UV Interval", Simple.UvInterval ) );
            Texture.Add( new UiInt( "UV Number Random", Simple.UvNoRandom ) );
            Texture.Add( new UiInt( "UV Number Loop Count", Simple.UvNoLoopCount ) );
            Texture.Add( new UiInt( "UV Reverse", Simple.UvReverse ) );

            Tabs.Add( Color = new UiParameters( "Color" ) );
            Color.Add( new UISimpleColor( Simple, 0 ) );
            Color.Add( new UISimpleColor( Simple, 1 ) );
            Color.Add( new UISimpleColor( Simple, 2 ) );
            Color.Add( new UISimpleColor( Simple, 3 ) );
        }

        public override void DrawUnassigned( string parentId ) {
            if( ImGui.SmallButton( "+ Simple Animation" + parentId ) ) {
                AVFXBase.RecurseAssigned( Simple, true );

                // Refresh model selects
                Creation.Remove( InjectionModelSelect );
                Creation.Remove( InjectionVertexModelSelect );

                Creation.Prepend( InjectionModelSelect = new UiNodeSelect<UiModel>( Particle, "Injection Model", Particle.NodeGroups.Models, Simple.InjectionModelIdx ) );
                Creation.Prepend( InjectionVertexModelSelect = new UiNodeSelect<UiModel>( Particle, "Injection Vertex Bind Model", Particle.NodeGroups.Models, Simple.InjectionVertexBindModelIdx ) );
            }
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Simple";
            if( UiUtils.RemoveButton( "Delete" + id, small: true ) ) {
                InjectionModelSelect.DeleteSelect();
                InjectionVertexModelSelect.DeleteSelect();

                Simple.SetAssigned( false );
                return;
            }

            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Simple Animation";

        public override bool IsAssigned() => Simple.IsAssigned();
    }

    internal class UISimpleColor : IUiBase {
        private readonly AVFXParticleSimple Simple;
        private readonly int Idx;
        private Vector4 Color;
        private int Frame;

        public UISimpleColor( AVFXParticleSimple simple, int idx ) {
            Simple = simple;
            Idx = idx;

            Color = new Vector4(
                ( float )( int )Simple.Colors.Colors[Idx * 4 + 0] / 255,
                ( float )( int )Simple.Colors.Colors[Idx * 4 + 1] / 255,
                ( float )( int )Simple.Colors.Colors[Idx * 4 + 2] / 255,
                ( float )( int )Simple.Colors.Colors[Idx * 4 + 3] / 255
            );
            Frame = Simple.Frames.Frames[Idx];
        }

        public void DrawInline( string parentId ) {
            if( ImGui.InputInt( "Frame " + Idx + parentId, ref Frame ) ) {
                Simple.Frames.Frames[Idx] = Frame;
            }
            if( ImGui.ColorEdit4( "Color " + Idx + parentId, ref Color, ImGuiColorEditFlags.Float ) ) {
                Simple.Colors.Colors[Idx * 4 + 0] = ( byte )( int )( Color.X * 255f );
                Simple.Colors.Colors[Idx * 4 + 1] = ( byte )( int )( Color.Y * 255f );
                Simple.Colors.Colors[Idx * 4 + 2] = ( byte )( int )( Color.Z * 255f );
                Simple.Colors.Colors[Idx * 4 + 3] = ( byte )( int )( Color.W * 255f );
            }
        }
    }
}
