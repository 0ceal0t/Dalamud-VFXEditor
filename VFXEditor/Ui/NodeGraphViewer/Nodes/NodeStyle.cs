using System.Numerics;
using VfxEditor.Ui.NodeGraphViewer.Utils;

namespace VfxEditor.Ui.NodeGraphViewer.Nodes {
    public class NodeStyle {
        private Vector2 Size;
        private Vector2 SizeHandle = Vector2.Zero;
        private Vector2 SizeBody = Vector2.Zero;
        private Vector2 MinSize;
        public Vector2 HandleTextPadding = new( 3, 0 );
        private Vector2 HandleTextSize = Vector2.Zero;
        private Vector2? MinestMinSize = null;

        public Vector4 ColorUnique = NodeUtils.Colors.GenObj_BlueAction;
        public Vector4 ColorBg = NodeUtils.Colors.NodeBg;
        public Vector4 ColorFg = NodeUtils.Colors.NodeFg;

        public NodeStyle( Vector2 size, Vector2 minSize, Vector2? minestMinSize = null ) {
            MinestMinSize = minestMinSize;
            SetMinSize( minSize );
            SetSize( size );
            UpdatePartialSizes();
        }

        public void SetSize( Vector2 size ) {
            Size.X = size.X < MinSize.X ? MinSize.X : size.X;
            Size.Y = size.Y < MinSize.Y ? MinSize.Y : size.Y;
            UpdatePartialSizes();
        }

        public void SetSizeScaled( Vector2 size, float canvasScaling = 1 ) => SetSize( size / canvasScaling );

        public Vector2 GetSize() => Size;

        public Vector2 GetSizeScaled( float canvasScaling ) => GetSize() * canvasScaling;

        public Vector2 GetHandleSize() => SizeHandle;

        public Vector2 GetHandleSizeScaled( float scaling ) => GetHandleSize() * scaling;

        public Vector2 GetHandleTextSize() => HandleTextSize;

        public void SetHandleTextSize( Vector2 handleTextSize ) {
            HandleTextSize = handleTextSize;
            SetMinSize( GetHandleTextSize() + Node.NodeInsidePadding * 2 );
        }

        private void SetMinSize( Vector2 handleSize ) {
            MinSize.X = handleSize.X < ( MinestMinSize == null ? Node.MinHandleSize.X : MinestMinSize.Value.X )
                             ? MinestMinSize == null ? Node.MinHandleSize.X : MinestMinSize.Value.X
                             : handleSize.X;
            MinSize.Y = handleSize.Y < ( MinestMinSize == null ? Node.MinHandleSize.Y : MinestMinSize.Value.Y )
                             ? MinestMinSize == null ? Node.MinHandleSize.Y : MinestMinSize.Value.Y
                             : handleSize.Y;
        }

        private void UpdatePartialSizes() {
            UpdateHandleSize();
            UpdateSizeBody();
        }

        private void UpdateHandleSize() {
            SizeHandle.X = GetSize().X;
            SizeHandle.Y = MinSize.Y;
        }

        private void UpdateSizeBody() {
            SizeBody.X = Size.X;
            SizeBody.Y = Size.Y - SizeHandle.Y;
        }

        public bool CheckPosWithin( Vector2 nodeOSP, float canvasScaling, Vector2 screenPos ) {
            var tNodeSize = GetSizeScaled( canvasScaling );
            Area tArea = new( nodeOSP, tNodeSize );
            return tArea.CheckPosIsWithin( screenPos );
        }

        public bool CheckWithinHandle( Vector2 nodePosition, float canvasScaling, Vector2 screenPos ) {
            var tNodeSize = GetHandleSize() * canvasScaling;
            Area tArea = new( nodePosition, tNodeSize );
            return tArea.CheckPosIsWithin( screenPos );
        }

        public bool CheckAreaIntersect( Vector2 nodeOSP, float canvasScaling, Area screenArea ) {
            var tNodeSize = GetSizeScaled( canvasScaling );
            Area tArea = new( nodeOSP, tNodeSize );
            return tArea.CheckAreaIntersect( screenArea );
        }
    }
}
