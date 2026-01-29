using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImPlot;
using Dalamud.Interface.Utility.Raii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public class UiModelUvView : IUiItem {
        private static readonly Vector4 LINE_COLOR = new( 0, 0.1f, 1, 1 );

        private bool DrawOnce = false;
        private int UvMode = 1;

        private (float[], float[]) Uv1 = ([], []);
        private (float[], float[]) Uv2 = ([], []);
        private (float[], float[]) Uv3 = ([], []);
        private (float[], float[]) Uv4 = ([], []);

        private int NumPoints = 0;

        public UiModelUvView() { }

        public void LoadModel( AvfxModel model ) {
            var indexes = model.Indexes.Indexes;
            var vertexes = model.Vertexes.Vertexes;

            List<AvfxVertex> orderedVertexes = [];
            foreach( var index in indexes ) {
                var v1 = vertexes[index.I1];
                var v2 = vertexes[index.I2];
                var v3 = vertexes[index.I3];

                orderedVertexes.Add( v1 );
                orderedVertexes.Add( v2 );

                orderedVertexes.Add( v2 );
                orderedVertexes.Add( v3 );

                orderedVertexes.Add( v3 );
                orderedVertexes.Add( v1 );
            }

            NumPoints = orderedVertexes.Count;

            Uv1 = ([.. orderedVertexes.Select( x => x.Uv1.X )], [.. orderedVertexes.Select( x => x.Uv1.Y )]);
            Uv2 = ([.. orderedVertexes.Select( x => x.Uv2.X )], [.. orderedVertexes.Select( x => x.Uv2.Y )]);
            Uv3 = ([.. orderedVertexes.Select( x => x.Uv3.X )], [.. orderedVertexes.Select( x => x.Uv3.Y )]);
            Uv4 = ([.. orderedVertexes.Select( x => x.Uv4.X )], [.. orderedVertexes.Select( x => x.Uv4.Y )]);
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "UV" );

            ImGui.RadioButton( "UV 1", ref UvMode, 1 );

            ImGui.SameLine();
            ImGui.RadioButton( "UV 2", ref UvMode, 2 );

            ImGui.SameLine();
            ImGui.RadioButton( "UV 3", ref UvMode, 3 );

            ImGui.SameLine();
            ImGui.RadioButton( "UV 4", ref UvMode, 4 );

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( !DrawOnce || ImGui.Button( "Fit To Contents" ) ) {
                ImPlot.SetNextAxesToFit();
                DrawOnce = true;
            }

            using var plot = ImRaii.Plot( "##Plot", new Vector2( -1, -1 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle );
            if( plot ) {
                ImPlot.SetupAxes( "U", "V", ImPlotAxisFlags.None, ImPlotAxisFlags.None );

                ImPlot.PushStyleColor( ImPlotCol.Line, LINE_COLOR );
                ImPlot.PushStyleVar( ImPlotStyleVar.LineWeight, 2 );

                var coords = UvMode switch {
                    1 => Uv1,
                    2 => Uv2,
                    3 => Uv3,
                    4 => Uv4,
                    _ => Uv1
                };

                for( var i = 0; i < NumPoints; i += 2 ) { // draw them 2 at a time
                    ImPlot.PlotLine( "UV", ref coords.Item1[i], ref coords.Item2[i], 2 );
                }

                ImPlot.PopStyleColor();
                ImPlot.PopStyleVar();
            }
        }
    }
}
