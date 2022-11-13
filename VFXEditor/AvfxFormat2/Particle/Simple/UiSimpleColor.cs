using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VfxEditor;

namespace VfxEditor.AvfxFormat2 {
    public class UiSimpleColor : IUiBase {
        public readonly AvfxParticleSimple Simple;
        public readonly int Idx;

        private bool ColorDrag = false;
        private DateTime ColorDragTime = DateTime.Now;
        private Vector4 ColorDragState;

        public UiSimpleColor( AvfxParticleSimple simple, int idx ) {
            Simple = simple;
            Idx = idx;
        }

        public void Draw( string parentId ) {
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

        public Vector4 GetColor() => new(
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
