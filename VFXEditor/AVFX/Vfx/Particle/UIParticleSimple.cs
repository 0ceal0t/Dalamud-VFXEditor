using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using VFXEditor.AVFXLib;
using VFXEditor.AVFXLib.Particle;
using VFXEditor.Helper;

namespace VFXEditor.AVFX.VFX {
    public class UIParticleSimple : UIItem {
        public readonly AVFXParticleSimple Simple;
        public readonly UIParticle Particle;

        public UINodeSelect<UIModel> InjectionModelSelect;
        public UINodeSelect<UIModel> InjectionVertexModelSelect;

        public readonly List<UIItem> Tabs;
        public readonly UIParameters Creation;
        public readonly UIParameters Animation;
        public readonly UIParameters Texture;
        public readonly UIParameters Color;

        public UIParticleSimple( AVFXParticleSimple simple, UIParticle particle ) {
            Simple = simple;
            Particle = particle;

            Tabs = new List<UIItem> {
                ( Creation = new UIParameters( "Creation" ) )
            };

            if(IsAssigned()) {
                Creation.Add( InjectionModelSelect = new UINodeSelect<UIModel>( Particle, "Injection Model", Particle.Main.Models, Simple.InjectionModelIdx ) );
                Creation.Add( InjectionVertexModelSelect = new UINodeSelect<UIModel>( Particle, "Injection Vertex Bind Model", Particle.Main.Models, Simple.InjectionVertexBindModelIdx ) );
            }

            Creation.Add( new UIInt( "Injection Position Type", Simple.InjectionPositionType ) );
            Creation.Add( new UIInt( "Injection Direction Type", Simple.InjectionDirectionType ) );
            Creation.Add( new UIInt( "Base Direction Type", Simple.BaseDirectionType ) );
            Creation.Add( new UIInt( "Create Count", Simple.CreateCount ) );
            Creation.Add( new UIFloat3( "Create Area", Simple.CreateAreaX, Simple.CreateAreaY, Simple.CreateAreaZ ) );
            Creation.Add( new UIFloat3( "Coord Accuracy", Simple.CoordAccuracyX, Simple.CoordAccuracyY, Simple.CoordAccuracyZ ) );
            Creation.Add( new UIFloat3( "Coord Gra", Simple.CoordGraX, Simple.CoordGraY, Simple.CoordGraZ ) );
            Creation.Add( new UIFloat( "Injection Radial Direction 0", Simple.InjectionRadialDir0 ) );
            Creation.Add( new UIFloat( "Injection Radial Direction 1", Simple.InjectionRadialDir1 ) );
            Creation.Add( new UIFloat( "Pivot X", Simple.PivotX ) );
            Creation.Add( new UIFloat( "Pivot Y", Simple.PivotY ) );
            Creation.Add( new UIInt( "Block Number", Simple.BlockNum ) );
            Creation.Add( new UIFloat( "Line Length Minimum", Simple.LineLengthMin ) );
            Creation.Add( new UIFloat( "Line Length Maximum", Simple.LineLengthMax ) );
            Creation.Add( new UIInt( "Create Interval", Simple.CreateIntervalVal ) );
            Creation.Add( new UIInt( "Create Interval Random", Simple.CreateIntervalRandom ) );
            Creation.Add( new UIInt( "Create Interval Count", Simple.CreateIntervalCount ) );
            Creation.Add( new UIInt( "Create Interval Life", Simple.CreateIntervalLife ) );
            Creation.Add( new UIInt( "Create New After Delete", Simple.CreateNewAfterDelete ) );

            Tabs.Add( Animation = new UIParameters( "Animation" ) );
            Animation.Add( new UIFloat2( "Scale Start", Simple.ScaleXStart, Simple.ScaleYStart ) );
            Animation.Add( new UIFloat2( "Scale End", Simple.ScaleXEnd, Simple.ScaleYEnd ) );
            Animation.Add( new UIFloat( "Scale Curve", Simple.ScaleCurve ) );
            Animation.Add( new UIFloat2( "Scale X Random", Simple.ScaleRandX0, Simple.ScaleRandX1 ) );
            Animation.Add( new UIFloat2( "Scale Y Random", Simple.ScaleRandY0, Simple.ScaleRandY1 ) );
            Animation.Add( new UIFloat3( "Rotation Add", Simple.RotXAdd, Simple.RotYAdd, Simple.RotZAdd ) );
            Animation.Add( new UIFloat3( "Rotation Base", Simple.RotXBase, Simple.RotYBase, Simple.RotZBase ) );
            Animation.Add( new UIFloat3( "Rotation Velocity", Simple.RotXVel, Simple.RotYVel, Simple.RotZVel ) );
            Animation.Add( new UIFloat( "Minimum Velocity", Simple.VelMin ) );
            Animation.Add( new UIFloat( "Maximum Velocity", Simple.VelMax ) );
            Animation.Add( new UIFloat( "Velocity Flattery Rate", Simple.VelFlatteryRate ) );
            Animation.Add( new UIFloat( "Velocity Flattery Speed", Simple.VelFlatterySpeed ) );
            Animation.Add( new UIInt( "Scale Random Link", Simple.ScaleRandomLink ) );
            Animation.Add( new UIInt( "Bind Parent", Simple.BindParent ) );
            Animation.Add( new UIInt( "Scale By Parent", Simple.ScaleByParent ) );
            Animation.Add( new UIInt( "Polyline Tag", Simple.PolyLineTag ) );

            Tabs.Add( Texture = new UIParameters( "Texture" ) );
            Texture.Add( new UIInt( "UV Cell U", Simple.UvCellU ) );
            Texture.Add( new UIInt( "UV Cell V", Simple.UvCellV ) );
            Texture.Add( new UIInt( "UV Interval", Simple.UvInterval ) );
            Texture.Add( new UIInt( "UV Number Random", Simple.UvNoRandom ) );
            Texture.Add( new UIInt( "UV Number Loop Count", Simple.UvNoLoopCount ) );
            Texture.Add( new UIInt( "UV Reverse", Simple.UvReverse ) );

            Tabs.Add( Color = new UIParameters( "Color" ) );
            Color.Add( new UISimpleColor( Simple, 0 ) );
            Color.Add( new UISimpleColor( Simple, 1 ) );
            Color.Add( new UISimpleColor( Simple, 2 ) );
            Color.Add( new UISimpleColor( Simple, 3 ) );
        }

        public override void DrawUnAssigned( string parentId ) {
            if( ImGui.SmallButton( "+ Simple Animation" + parentId ) ) {
                AVFXBase.RecurseAssigned( Simple, true );

                // Refresh model selects
                Creation.Remove( InjectionModelSelect );
                Creation.Remove( InjectionVertexModelSelect );

                Creation.Prepend( InjectionModelSelect = new UINodeSelect<UIModel>( Particle, "Injection Model", Particle.Main.Models, Simple.InjectionModelIdx ) );
                Creation.Prepend( InjectionVertexModelSelect = new UINodeSelect<UIModel>( Particle, "Injection Vertex Bind Model", Particle.Main.Models, Simple.InjectionVertexBindModelIdx ) );
            }
        }

        public override void DrawBody( string parentId ) {
            var id = parentId + "/Simple";
            if( UIHelper.RemoveButton( "Delete" + id, small: true ) ) {
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

    internal class UISimpleColor : UIBase {
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

        public override void Draw( string parentId ) {
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
