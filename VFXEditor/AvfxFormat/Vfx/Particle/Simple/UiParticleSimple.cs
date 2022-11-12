using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;
using VfxEditor.AVFXLib.Particle;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiParticleSimple : UiParticleAttribute {
        public readonly AVFXParticleSimple Simple;

        public readonly UiParameters Animation;
        public readonly UiParameters Texture;
        public readonly UiParameters Color;

        public UiParticleSimple( AVFXParticleSimple simple, UiParticle particle ) : base( particle ) {
            Simple = simple;
            InitNodeSelects();

            Parameters.Add( new UiInt( "Injection Position Type", Simple.InjectionPositionType ) );
            Parameters.Add( new UiInt( "Injection Direction Type", Simple.InjectionDirectionType ) );
            Parameters.Add( new UiInt( "Base Direction Type", Simple.BaseDirectionType ) );
            Parameters.Add( new UiInt( "Create Count", Simple.CreateCount ) );
            Parameters.Add( new UiFloat3( "Create Area", Simple.CreateAreaX, Simple.CreateAreaY, Simple.CreateAreaZ ) );
            Parameters.Add( new UiFloat3( "Coord Accuracy", Simple.CoordAccuracyX, Simple.CoordAccuracyY, Simple.CoordAccuracyZ ) );
            Parameters.Add( new UiFloat3( "Coord Gra", Simple.CoordGraX, Simple.CoordGraY, Simple.CoordGraZ ) );
            Parameters.Add( new UiFloat( "Injection Radial Direction 0", Simple.InjectionRadialDir0 ) );
            Parameters.Add( new UiFloat( "Injection Radial Direction 1", Simple.InjectionRadialDir1 ) );
            Parameters.Add( new UiFloat( "Pivot X", Simple.PivotX ) );
            Parameters.Add( new UiFloat( "Pivot Y", Simple.PivotY ) );
            Parameters.Add( new UiInt( "Block Number", Simple.BlockNum ) );
            Parameters.Add( new UiFloat( "Line Length Minimum", Simple.LineLengthMin ) );
            Parameters.Add( new UiFloat( "Line Length Maximum", Simple.LineLengthMax ) );
            Parameters.Add( new UiInt( "Create Interval", Simple.CreateIntervalVal ) );
            Parameters.Add( new UiInt( "Create Interval Random", Simple.CreateIntervalRandom ) );
            Parameters.Add( new UiInt( "Create Interval Count", Simple.CreateIntervalCount ) );
            Parameters.Add( new UiInt( "Create Interval Life", Simple.CreateIntervalLife ) );
            Parameters.Add( new UiInt( "Create New After Delete", Simple.CreateNewAfterDelete ) );

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
            if( ImGui.SmallButton( "+ Simple Animation" + parentId ) ) Assign( Simple );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Simple";
            if( UiUtils.RemoveButton( "Delete" + id, small: true ) ) {
                Unassign( Simple );
                return;
            }
            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );
            DrawListTabs( Tabs, id );
        }

        public override string GetDefaultText() => "Simple Animation";

        public override bool IsAssigned() => Simple.IsAssigned();

        public override List<UiNodeSelect> GetNodeSelects() => new() {
            new UiNodeSelect<UiModel>( Particle, "Injection Model", Particle.NodeGroups.Models, Simple.InjectionModelIdx ),
            new UiNodeSelect<UiModel>( Particle, "Injection Vertex Bind Model", Particle.NodeGroups.Models, Simple.InjectionVertexBindModelIdx )
        };
    }

    public class UISimpleColor : IUiBase {
        private readonly AVFXParticleSimple Simple;
        private readonly int Idx;

        private bool ColorDrag = false;
        private DateTime ColorDragTime = DateTime.Now;
        private Vector4 ColorDragState;

        public UISimpleColor( AVFXParticleSimple simple, int idx ) {
            Simple = simple;
            Idx = idx;
        }

        public void DrawInline( string parentId ) {
            var frame = GetFrame();
            if( ImGui.InputInt( "Frame " + Idx + parentId, ref frame ) ) CommandManager.Avfx.Add( new UiParticleSimpleFrameCommand( this, frame ) );

            var color = GetColor();
            if( ImGui.ColorEdit4( "Color " + Idx + parentId, ref color, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoDragDrop ) ) {
                if( !ColorDrag ) {
                    ColorDrag = true;
                    ColorDragState = color;
                }
                ColorDragTime = DateTime.Now;
                SetColor( color );
            }
            else if( ColorDrag && ( DateTime.Now - ColorDragTime ).TotalMilliseconds > 200 ) {
                ColorDrag = false;
                CommandManager.Avfx.Add( new UiParticleSimpleColorCommand( this, ColorDragState, color ) );
            }
        }

        public int GetFrame() => Simple.Frames.Frames[Idx];

        public void SetFrame( int value ) => Simple.Frames.Frames[Idx] = value;

        public Vector4 GetColor() => new (
            ( float )( int )Simple.Colors.Colors[Idx * 4 + 0] / 255,
            ( float )( int )Simple.Colors.Colors[Idx * 4 + 1] / 255,
            ( float )( int )Simple.Colors.Colors[Idx * 4 + 2] / 255,
            ( float )( int )Simple.Colors.Colors[Idx * 4 + 3] / 255
        );

        public void SetColor( Vector4 value ) {
            Simple.Colors.Colors[Idx * 4 + 0] = ( byte )( int )( value.X * 255f );
            Simple.Colors.Colors[Idx * 4 + 1] = ( byte )( int )( value.Y * 255f );
            Simple.Colors.Colors[Idx * 4 + 2] = ( byte )( int )( value.Z * 255f );
            Simple.Colors.Colors[Idx * 4 + 3] = ( byte )( int )( value.W * 255f );
        }
    }
}
