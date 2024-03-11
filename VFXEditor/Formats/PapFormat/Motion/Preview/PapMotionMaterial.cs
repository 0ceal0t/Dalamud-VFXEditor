using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok;
using ImGuiNET;
using ImPlotNET;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.DirectX;
using VfxEditor.DirectX.Renderers;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Utils;

namespace VfxEditor.Formats.PapFormat.Motion.Preview {
    public unsafe class PapMotionMaterial : PapMotionPreview {
        private static int MATERIAL_ID = 0;
        private static GradientRenderer Preview => Plugin.DirectXManager.PapMaterialPreview;

        public readonly int RenderId = Renderer.NewId;
        private readonly int Id = MATERIAL_ID++;

        private List<List<(int, Vector3)>> Data;

        public PapMotionMaterial( PapMotion motion ) : base( motion ) { }

        public override void Draw( int idx ) {
            if( Data == null || Preview.CurrentRenderId != RenderId ) UpdatePreview();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                if( ImGui.Button( "Export" ) ) ExportDialog( Motion.File.Animations[idx].GetName() );
                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog( idx );
                ImGui.SameLine();
                UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );
            }

            // ====================

            ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
            ImPlot.SetNextAxisLimits( ImAxis.X1, 0, Motion.TotalFrames, ImPlotCond.Once );
            ImPlot.PushStyleVar( ImPlotStyleVar.FitPadding, new Vector2( 0.5f, 0.5f ) );
            if( ImPlot.BeginPlot( "##CurveEditor", new Vector2( -1, -1 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle | ImPlotFlags.NoLegend ) ) {
                ImPlot.SetupAxisLimitsConstraints( ImAxis.X1, 0, double.MaxValue - 1 );
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None, ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel );

                var topLeft = new ImPlotPoint { x = 0, y = 1 };
                var bottomRight = new ImPlotPoint { x = Motion.TotalFrames, y = -1 };
                ImPlot.PlotImage( "##Gradient", Preview.Output, topLeft, bottomRight );

                for( var i = 0; i < Data.Count - 1; i++ ) {
                    var yPos = -1f + 2f * ( i + 1f ) / ( Data.Count );
                    var xs = new double[] { 0, Motion.TotalFrames };
                    var ys = new double[] { yPos, yPos };
                    ImPlot.SetNextLineStyle( new( 0f, 0f, 0f, 1f ), 5f );
                    ImPlot.PlotLine( $"Line {i}", ref xs[0], ref ys[0], xs.Length );
                }

                ImPlot.EndPlot();
            }
            ImPlot.PopStyleVar( 1 );
        }

        private void UpdatePreview() {
            Data = new();

            for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                var bone = Motion.Skeleton->Bones[i];
                if( bone.Name.String == "n_material" ) continue;
                Data.Add( new() );
            }

            for( var frame = 0; frame < Motion.TotalFrames; frame++ ) {
                Motion.AnimationControl->LocalTime = frame * ( 1 / 30f );

                var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( Motion.Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
                var floats = ( float* )Marshal.AllocHGlobal( Motion.Skeleton->FloatSlots.Length * sizeof( float ) );
                Motion.AnimatedSkeleton->sampleAndCombineAnimations( transforms, floats );

                var rowIdx = 0;
                for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                    var bone = Motion.Skeleton->Bones[i];
                    if( bone.Name.String == "n_material" ) continue;

                    var position = transforms[i].Translation;
                    Data[rowIdx].Add( (frame, new( position.X, position.Y, position.Z )) );

                    rowIdx++;
                }

                Marshal.FreeHGlobal( ( nint )transforms );
                Marshal.FreeHGlobal( ( nint )floats );
            }

            Preview.SetGradient( RenderId, Data );
        }
    }
}
