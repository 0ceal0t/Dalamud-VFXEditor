using Dalamud.Logging;
using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VFXEditor.AVFXLib.Model;

namespace VFXEditor.AVFX.VFX.Model {
    public class UIModelUvView {
        private static readonly Vector4 LINE_COLOR = new( 0, 0.1f, 1, 1 );

        private bool DrawOnce = false;
        private int UvMode = 1;

        private float[] Uv1_X = new float[0];
        private float[] Uv1_Y = new float[0];
        private float[] Uv2_X = new float[0];
        private float[] Uv2_Y = new float[0];
        private int NumPoints = 0;

        public UIModelUvView() {

        }

        public void LoadModel( AVFXModel model ) {
            var indexes = model.Indexes.Indexes;
            var vertexes = model.Vertexes.Vertexes;

            List<AVFXVertex> orderedVertexes = new();
            foreach ( var index in indexes ) {
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
            Uv1_X = orderedVertexes.Select( x => x.UV1[0] ).ToArray();
            Uv1_Y = orderedVertexes.Select( x => x.UV1[1] ).ToArray();
            Uv2_X = orderedVertexes.Select( x => x.UV2[2] ).ToArray();
            Uv2_Y = orderedVertexes.Select( x => x.UV2[3] ).ToArray();

            PluginLog.Log( $"Loaded {NumPoints} points" );
        }

        public void DrawInline(string parentId) {
            if( ImGui.RadioButton( "UV 1", ref UvMode, 1 ) ) {
            }
            ImGui.SameLine();
            if( ImGui.RadioButton( "UV 2", ref UvMode, 2 ) ) {
            }

            ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 5 );

            if( !DrawOnce || ImGui.Button( "Fit To Contents" + parentId ) ) {
                ImPlot.SetNextAxesToFit();
                DrawOnce = true;
            }

            if( ImPlot.BeginPlot( $"{parentId}-Plot", new Vector2( -1, -1 ), ImPlotFlags.NoMenus | ImPlotFlags.NoTitle ) ) {
                ImPlot.SetupAxes( "U", "V", ImPlotAxisFlags.None, ImPlotAxisFlags.None );

                ImPlot.PushStyleColor( ImPlotCol.Line, LINE_COLOR );
                ImPlot.PushStyleVar( ImPlotStyleVar.LineWeight, 2 );

                var x = UvMode == 1 ? Uv1_X : Uv2_X;
                var y = UvMode == 1 ? Uv1_Y : Uv2_Y;

                for( var i = 0; i < NumPoints; i += 2 ) { // draw them 2 at a time
                    ImPlot.PlotLine( "UV", ref x[i], ref y[i], 2 );
                }

                ImPlot.PopStyleColor();
                ImPlot.PopStyleVar();

                ImPlot.EndPlot();
            }
        }
    }
}
