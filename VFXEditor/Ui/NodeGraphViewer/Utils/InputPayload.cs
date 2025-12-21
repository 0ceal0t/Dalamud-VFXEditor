using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer.Utils {
    public class InputPayload {
        private static DateTime LastMouseClicked = DateTime.MinValue;
        private static DateTime LastKeyClicked = DateTime.MinValue;
        private static DateTime LastWheelWheeled = DateTime.MinValue;
        private static Vector2? LastMouseDragDelta = null;
        private const double MouseClickValidityThreshold = 150;
        private const double KeyClickValidityThreshold = 250;
        private static readonly float DelayBetweenMouseWheelCapture = 100;
        private static bool WasLmbDragged = false;
        private static readonly List<ImGuiKey> KeysToCheck = [ImGuiKey.Delete, ImGuiKey.C, ImGuiKey.V];
        private static readonly Dictionary<ImGuiKey, bool> KeysDown = [];
        private static double DeltaLastMouseClick() => ( DateTime.Now - LastMouseClicked ).TotalMilliseconds;
        private static double DeltaLastKeyClick() => ( DateTime.Now - LastKeyClicked ).TotalMilliseconds;
        public static bool CheckMouseClickValidity() {
            if( DeltaLastMouseClick() > MouseClickValidityThreshold ) {
                LastMouseClicked = DateTime.Now;
                return true;
            }
            return false;
        }
        public static bool CheckKeyClickValidity() {
            if( DeltaLastKeyClick() > KeyClickValidityThreshold ) {
                LastKeyClicked = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool IsHovered = false;
        public bool IsMouseRmb = false;
        public bool IsMouseLmb = false;
        public bool IsMouseMid = false;
        public bool IsMouseRmbDown = false;
        public bool IsMouseLmbDown = false;
        public bool IsKeyShift = false;
        public bool IsKeyAlt = false;
        public bool IsKeyCtrl = false;
        public bool IsKeyDel = false;
        public bool IsKeyC = false;
        public bool IsKeyV = false;
        public Vector2 MousePos = Vector2.Zero;
        private Vector2? FirstMouseLeftHoldPos = null;
        public bool IsALmbDragRelease = false;
        public bool IsMouseDragLeft = false;
        public bool IsMouseDragRight = false;
        public Vector2? LmbDragDelta = null;
        public float MouseWheelValue = 0;

        public void CaptureInput( bool pCaptureMouseWheel = false, bool pCaptureMouseDrag = false, HashSet<ImGuiKey> pExtraKeyboardInputs = null ) {
            var io = ImGui.GetIO();
            if( io.KeyShift ) IsKeyShift = true;
            if( io.KeyAlt ) IsKeyAlt = true;
            if( io.KeyCtrl ) IsKeyCtrl = true;
            if( io.MouseReleased[0] ) IsMouseLmb = true;
            if( io.MouseReleased[1] ) IsMouseRmb = true;
            if( io.MouseReleased[2] ) IsMouseMid = true;
            if( io.MouseDown[0] ) IsMouseLmbDown = true;
            if( io.MouseDown[1] ) IsMouseRmbDown = true;
            MousePos = io.MousePos;
            IsALmbDragRelease = WasLmbDragged;

            if( pCaptureMouseDrag ) {
                CaptureMouseDragDelta();
            }
            if( pCaptureMouseWheel ) CaptureMouseWheel();
            if( !IsMouseLmbDown && !IsMouseLmb ) {
                WasLmbDragged = false;
                IsALmbDragRelease = WasLmbDragged;
            }

            // extra-inputs
            if( pExtraKeyboardInputs != null ) {
                foreach( var imKey in KeysToCheck ) {
                    // Try get state: Down
                    if( pExtraKeyboardInputs.Contains( imKey ) ) {
                        if( !KeysDown.TryAdd( imKey, true ) )
                            KeysDown[imKey] = true;
                    }
                    else {
                        // Try get state: Released
                        if( KeysDown.TryGetValue( imKey, out var isKeyDown ) && isKeyDown ) {
                            switch( imKey ) {
                                case ImGuiKey.Delete: IsKeyDel = true; break;
                                case ImGuiKey.C: IsKeyC = true; break;
                                case ImGuiKey.V: IsKeyV = true; break;
                            }
                            KeysDown[imKey] = false;
                        }
                    }
                }
            }
            if( ImGui.IsKeyPressed( ImGuiKey.Delete ) ) IsKeyDel = true;
        }

        private Vector2? CaptureMouseDragDeltaInternal() {
            // Get first left hold.
            if( IsMouseLmbDown && FirstMouseLeftHoldPos == null ) { FirstMouseLeftHoldPos = MousePos; }
            if( IsMouseLmb ) { FirstMouseLeftHoldPos = null; }

            Vector2? tRes = null;
            IsMouseDragLeft = ImGui.IsMouseDragging( ImGuiMouseButton.Left );
            IsMouseDragRight = ImGui.IsMouseDragging( ImGuiMouseButton.Right );
            if( IsMouseDragLeft ) {
                if( LastMouseDragDelta == null ) {
                    LastMouseDragDelta = ImGui.GetMouseDragDelta();
                    // Imgui's MouseDelta does not recognize tiny drag under certain threshold (prob to distinguish click vs drag)
                    // So this is a compensation which adds an extra distance equal to that threshold if the node is being dragged.
                    tRes = LastMouseDragDelta + ( MousePos - FirstMouseLeftHoldPos );
                }
                else {
                    var d = ImGui.GetMouseDragDelta();
                    if( IsMouseDragLeft ) {
                        tRes = d - LastMouseDragDelta;
                        tRes = tRes.Value + tRes.Value * 0;   // dragging loss is around 16% without compensation
                    }
                    LastMouseDragDelta = d;
                }
            }
            else {
                LastMouseDragDelta = null;
            }


            // distinguishing between a release from click or drag
            if( IsMouseLmbDown ) {
                if( tRes.HasValue ) WasLmbDragged = true;
            }
            else if( !IsMouseLmb ) {
                WasLmbDragged = false;
                IsALmbDragRelease = WasLmbDragged;
            }
            return tRes;
        }

        public void CaptureMouseDragDelta() {
            LmbDragDelta = CaptureMouseDragDeltaInternal();
        }

        private static float CaptureMouseWheelInternal() {
            var tRes = ImGui.GetIO().MouseWheel;
            if( tRes != 0
                && ( DateTime.Now - LastWheelWheeled ).TotalMilliseconds < DelayBetweenMouseWheelCapture ) {
                LastWheelWheeled = DateTime.Now;
            }
            return ImGui.GetIO().MouseWheel;
        }
        public void CaptureMouseWheel() {
            MouseWheelValue = CaptureMouseWheelInternal();
        }
    }
}
