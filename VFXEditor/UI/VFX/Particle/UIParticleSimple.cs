using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX
{
    public class UIParticleSimple : UIBase
    {
        public AVFXParticleSimple Simple;
        //=============
        public Vector4[] Colors;
        public int[] Frames;

        public UIParticleSimple(AVFXParticleSimple simple)
        {
            Simple = simple;
            Init();
        }
        public override void Init()
        {
            base.Init();
            if (!Simple.Assigned) { Assigned = false; return; }
            //=======================
            Attributes.Add(new UIInt("Injection Position Type", Simple.InjectionPositionType));
            Attributes.Add(new UIInt("Injection Direction Type", Simple.InjectionDirectionType));
            Attributes.Add(new UIInt("Base Direction Type", Simple.BaseDirectionType));
            Attributes.Add(new UIInt("Create Count", Simple.CreateCount));
            Attributes.Add(new UIFloat3("Create Area", Simple.CreateAreaX, Simple.CreateAreaY, Simple.CreateAreaZ));
            Attributes.Add(new UIFloat3("Coord Accuracy", Simple.CoordAccuracyX, Simple.CoordAccuracyY, Simple.CoordAccuracyZ));
            Attributes.Add(new UIFloat3("Coord Gra", Simple.CoordGraX, Simple.CoordGraY, Simple.CoordGraZ));
            Attributes.Add(new UIFloat2("Scale Start", Simple.ScaleXStart, Simple.ScaleYStart));
            Attributes.Add(new UIFloat2("Scale End", Simple.ScaleXEnd, Simple.ScaleYEnd));
            Attributes.Add(new UIFloat("Scale Curve", Simple.ScaleCurve));
            Attributes.Add(new UIFloat2("Scale X Random", Simple.ScaleRandX0, Simple.ScaleRandX1));
            Attributes.Add(new UIFloat2("Scale Y Random", Simple.ScaleRandY0, Simple.ScaleRandY1));
            Attributes.Add(new UIFloat3("Rotation Add", Simple.RotXAdd, Simple.RotYAdd, Simple.RotZAdd));
            Attributes.Add(new UIFloat3("Rotation Base", Simple.RotXBase, Simple.RotYBase, Simple.RotZBase));
            Attributes.Add(new UIFloat3("Rotation Velocity", Simple.RotXVel, Simple.RotYVel, Simple.RotZVel));
            Attributes.Add(new UIFloat("Minimum Velocity", Simple.VelMin));
            Attributes.Add(new UIFloat("Maximum Velocity", Simple.VelMax));
            Attributes.Add(new UIFloat("Velocity Flattery Rate", Simple.VelFlatteryRate));
            Attributes.Add(new UIFloat("Velocity Flattery Speed", Simple.VelFlatterySpeed));
            Attributes.Add(new UIInt("UV Cell U", Simple.UvCellU));
            Attributes.Add(new UIInt("UV Cell V", Simple.UvCellV));
            Attributes.Add(new UIInt("UV Interval", Simple.UvInterval));
            Attributes.Add(new UIInt("UV Number Random", Simple.UvNoRandom));
            Attributes.Add(new UIInt("UV Number Loop Count", Simple.UvNoLoopCount));
            Attributes.Add(new UIInt("Injection Model Index", Simple.InjectionModelIdx));
            Attributes.Add(new UIInt("Injection Vertex Bind Model Index", Simple.InjectionVertexBindModelIdx));
            Attributes.Add(new UIInt("Injection Radial Direction 0", Simple.InjectionRadialDir0));
            Attributes.Add(new UIInt("Injection Radial Direction 1", Simple.InjectionRadialDir1));
            Attributes.Add(new UIFloat("Pivot X", Simple.PivotX));
            Attributes.Add(new UIFloat("Pivot Y", Simple.PivotY));
            Attributes.Add(new UIInt("Block Number", Simple.BlockNum));
            Attributes.Add(new UIFloat("Line Length Minimum", Simple.LineLengthMin));
            Attributes.Add(new UIFloat("Line Length Maximum", Simple.LineLengthMax));
            Attributes.Add(new UIInt("Create Interval", Simple.CreateIntervalVal));
            Attributes.Add(new UIInt("Create Interval Random", Simple.CreateIntervalRandom));
            Attributes.Add(new UIInt("Create Interval Count", Simple.CreateIntervalCount));
            Attributes.Add(new UIInt("Create Interval Life", Simple.CreateIntervalLife));
            Attributes.Add(new UIInt("Create New After Delete", Simple.CreateNewAfterDelete));
            Attributes.Add(new UIInt("UV Reverse", Simple.UvReverse));
            Attributes.Add(new UIInt("Scale Random Link", Simple.ScaleRandomLink));
            Attributes.Add(new UIInt("Bind Parent", Simple.BindParent));
            Attributes.Add(new UIInt("Scale By Parent", Simple.ScaleByParent));
            Attributes.Add(new UIInt("Polyline Tag", Simple.PolyLineTag));
            //=================
            Colors = new Vector4[4];
            Frames = new int[4];
            for (int i = 0; i < 4; i++)
            {
                Colors[i] = new Vector4(
                    (float)AVFXLib.Main.Util.Bytes1ToInt(new byte[] { Simple.Colors.colors[i * 4 + 0] }) / 255,
                    (float)AVFXLib.Main.Util.Bytes1ToInt(new byte[] { Simple.Colors.colors[i * 4 + 1] }) / 255,
                    (float)AVFXLib.Main.Util.Bytes1ToInt(new byte[] { Simple.Colors.colors[i * 4 + 2] }) / 255,
                    (float)AVFXLib.Main.Util.Bytes1ToInt(new byte[] { Simple.Colors.colors[i * 4 + 3] }) / 255
                );
                Frames[i] = Simple.Frames.frames[i];
            }
        }
        // =========== DRAW =====================
        public override void Draw( string parentId )
        {
            if( !Assigned )
            {
                DrawUnAssigned( parentId );
                return;
            }
            if( ImGui.TreeNode( "Simple Animation" + parentId ) )
            {
                DrawBody( parentId );
                ImGui.TreePop();
            }
        }
        public override void DrawSelect( string parentId, ref UIBase selected )
        {
            string id = parentId + "/Simple";
            if( !Assigned )
            {
                DrawUnAssigned( id );
                return;
            }
            if( ImGui.Selectable( "Simple Animation" + id, selected == this ) )
            {
                selected = this;
            }
        }
        private void DrawUnAssigned( string parentId )
        {
            if( ImGui.SmallButton( "+ Simple Animation" + parentId ) )
            {
                Simple.toDefault();
                Init();
            }
        }
        public override void DrawBody( string parentId )
        {
            var id = parentId + "/Simple";
            if( UIUtils.RemoveButton( "Delete" + id ) )
            {
                Simple.Assigned = false;
                Init();
            }
            DrawAttrs( id );
            //====================
            for( int i = 0; i < 4; i++ )
            {
                if( ImGui.InputInt( "Frame#" + i + id, ref Frames[i] ) )
                {
                    Simple.Frames.frames[i] = Frames[i];
                }
                if( ImGui.ColorEdit4( "Color#" + i + id, ref Colors[i], ImGuiColorEditFlags.Float ) )
                {
                    Simple.Colors.colors[i * 4 + 0] = AVFXLib.Main.Util.IntTo1Bytes( ( int )( Colors[i].X * 255f ) )[0];
                    Simple.Colors.colors[i * 4 + 1] = AVFXLib.Main.Util.IntTo1Bytes( ( int )( Colors[i].Y * 255f ) )[0];
                    Simple.Colors.colors[i * 4 + 2] = AVFXLib.Main.Util.IntTo1Bytes( ( int )( Colors[i].Z * 255f ) )[0];
                    Simple.Colors.colors[i * 4 + 3] = AVFXLib.Main.Util.IntTo1Bytes( ( int )( Colors[i].W * 255f ) )[0];
                }
            }
        }
    }
}
