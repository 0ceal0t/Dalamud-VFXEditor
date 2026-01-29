using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImPlot;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using VfxEditor.PapFormat;
using VfxEditor.PapFormat.Motion;
using VfxEditor.Utils;
using VfxEditor.DirectX;

namespace VfxEditor.Formats.PapFormat.Motion.Preview {
    public class PapMotionMaterialData {
        public readonly List<(int, Vector3)> Color = [];
        public double[] R {
            get {
                _InternalR ??= [.. Color.Select( x => ( double )x.Item2.X )];
                return _InternalR;
            }
        }
        public double[] G {
            get {
                _InternalG ??= [.. Color.Select( x => ( double )x.Item2.Y )];
                return _InternalG;
            }
        }
        public double[] B {
            get {
                _InternalB ??= [.. Color.Select( x => ( double )x.Item2.Z )];
                return _InternalB;
            }
        }

        private double[] _InternalR;
        private double[] _InternalG;
        private double[] _InternalB;
    }

    public unsafe class PapMotionMaterial : PapMotionPreview {
        public readonly PapFile File;

        public readonly int RenderId = RenderInstance.NewId;

        private static int MATERIAL_ID = 0;
        private readonly int Id = MATERIAL_ID++;

        private static bool IsColor => Plugin.Configuration.PapMaterialDisplay == 1;

        private string BoneSelected = "";
        private double[] AllFrames;
        private Dictionary<string, PapMotionMaterialData> Data;

        public PapMotionMaterial( PapFile file, PapMotion motion ) : base( motion ) {
            File = file;
        }

        public override void Draw( int idx ) {
            if( Data == null ) Update();

            // ======== CONTROLS ==========

            if( ImGui.RadioButton( "Color", ref Plugin.Configuration.PapMaterialDisplay, 1 ) ) Plugin.Configuration.Save();
            ImGui.SameLine();
            if( ImGui.RadioButton( "Graph", ref Plugin.Configuration.PapMaterialDisplay, 2 ) ) Plugin.Configuration.Save();
            ImGui.SameLine();

            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemSpacing with { X = ImGui.GetStyle().ItemInnerSpacing.X } ) ) {
                if( ImGui.Button( "Export" ) ) ExportDialog( Motion.File.Animations[idx].GetName() );
                ImGui.SameLine();
                if( ImGui.Button( "Replace" ) ) ImportDialog( idx );
                ImGui.SameLine();
                UiUtils.WikiButton( "https://github.com/0ceal0t/Dalamud-VFXEditor/wiki/Using-Blender-to-Edit-Skeletons-and-Animations" );
            }

            ImGui.Separator();

            if( IsColor ) {
                if( ImGui.ColorEdit3( "Base Preview Color", ref Plugin.Configuration.PapMaterialBaseColor, ImGuiColorEditFlags.NoInputs ) ) {
                    Plugin.Configuration.Save();
                    UpdateRender();
                }
            }
            else {
                ImGui.SetNextItemWidth( 150f );
                using( var combo = ImRaii.Combo( "##Material", BoneSelected ) ) {
                    if( combo ) {
                        foreach( var name in Data.Keys ) {
                            if( ImGui.Selectable( name, name == BoneSelected ) ) BoneSelected = name;
                        }
                    }
                }

                ImGui.SameLine();
                using var font = ImRaii.PushFont( UiBuilder.IconFont );
                if( ImGui.Button( FontAwesomeIcon.ArrowsLeftRightToLine.ToIconString() ) ) ImPlot.SetNextAxesToFit();
            }

            // ====================

            if( IsColor ) ImPlot.SetNextAxisLimits( ImAxis.Y1, -1, 1, ImPlotCond.Always );
            ImPlot.SetNextAxisLimits( ImAxis.X1, 0, Motion.TotalFrames, ImPlotCond.Once );
            ImPlot.PushStyleVar( ImPlotStyleVar.FitPadding, new Vector2( 0.5f, 0.5f ) );

            using var plot = ImRaii.Plot( "##CurveEditor", new Vector2( -1, -1 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle | ( IsColor ? ImPlotFlags.NoLegend : ImPlotFlags.None ) );
            if( plot ) {
                if( IsColor ) ImPlot.SetupAxisLimitsConstraints( ImAxis.X1, 0, double.MaxValue - 1 );
                ImPlot.SetupAxes( "Frame", "", ImPlotAxisFlags.None,
                    IsColor ? ( ImPlotAxisFlags.Lock | ImPlotAxisFlags.NoGridLines | ImPlotAxisFlags.NoDecorations | ImPlotAxisFlags.NoLabel ) : ImPlotAxisFlags.NoLabel );

                var element = Data[BoneSelected];

                if( IsColor ) {
                    var topLeft = new ImPlotPoint { X = 0, Y = 1 };
                    var bottomRight = new ImPlotPoint { X = Motion.TotalFrames, Y = -1 };

                    Plugin.DirectXManager.GradientRenderer.UpdateTexture( RenderId, File.GradientInstance, UpdateRender );
                    ImPlot.PlotImage( "##Gradient", new ImTextureID( File.GradientInstance.Output ), topLeft, bottomRight );

                    for( var i = 0; i < Data.Count - 1; i++ ) {
                        var yPos = -1f + 2f * ( i + 1f ) / ( Data.Count );
                        var xs = new double[] { 0, Motion.TotalFrames };
                        var ys = new double[] { yPos, yPos };
                        ImPlot.SetNextLineStyle( new( 0f, 0f, 0f, 1f ), 5f );
                        ImPlot.PlotLine( $"Line {i}", ref xs[0], ref ys[0], xs.Length );
                    }
                }
                else {
                    ImPlot.SetNextLineStyle( new( 1, 0, 0, 1 ), 3 );
                    ImPlot.PlotLine( "R", ref AllFrames[0], ref element.R[0], AllFrames.Length );

                    ImPlot.SetNextLineStyle( new( 0, 1, 0, 1 ), 3 );
                    ImPlot.PlotLine( "G", ref AllFrames[0], ref element.G[0], AllFrames.Length );

                    ImPlot.SetNextLineStyle( new( 0, 0, 1, 1 ), 3 );
                    ImPlot.PlotLine( "B", ref AllFrames[0], ref element.B[0], AllFrames.Length );
                }
            }

            ImPlot.PopStyleVar( 1 );
        }

        private void Update() {
            var allFrames = new List<double>();
            Data = [];

            for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                var name = Motion.Skeleton->Bones[i].Name.String;
                if( name == "n_material" ) continue;

                Data[name] = new();
                if( string.IsNullOrEmpty( BoneSelected ) ) BoneSelected = name;
            }

            for( var frame = 0; frame < Motion.TotalFrames; frame++ ) {
                allFrames.Add( frame );

                Motion.AnimationControl->LocalTime = frame * ( 1 / 30f );
                var transforms = ( hkQsTransformf* )Marshal.AllocHGlobal( Motion.Skeleton->Bones.Length * sizeof( hkQsTransformf ) );
                var floats = ( float* )Marshal.AllocHGlobal( Motion.Skeleton->FloatSlots.Length * sizeof( float ) );
                Motion.AnimatedSkeleton->sampleAndCombineAnimations( transforms, floats );

                for( var i = 0; i < Motion.Skeleton->Bones.Length; i++ ) {
                    var name = Motion.Skeleton->Bones[i].Name.String;
                    if( name == "n_material" ) continue;

                    var position = transforms[i].Translation;
                    Data[name].Color.Add( (frame, new( position.X, position.Y, position.Z )) );

                }

                Marshal.FreeHGlobal( ( nint )transforms );
                Marshal.FreeHGlobal( ( nint )floats );
            }

            AllFrames = [.. allFrames];
            UpdateRender();
        }

        private void UpdateRender() {
            Plugin.DirectXManager.GradientRenderer.SetGradient( RenderId, File.GradientInstance,
                [.. Data.Select( x => x.Value.Color.Select( y => (y.Item1, y.Item2 + Plugin.Configuration.PapMaterialBaseColor) ).ToList() )]
            );
        }
    }
}
