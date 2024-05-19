using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace VfxEditor.Ui.NodeGraphViewer {
    public class InputPayload {
        private static DateTime kLastMouseClicked = DateTime.MinValue;
        private static DateTime kLastKeyClicked = DateTime.MinValue;
        private static DateTime kLastWheelWheeled = DateTime.MinValue;
        private static Vector2? kLastMouseDragDelta = null;
        private const double kMouseClickValidityThreshold = 150;
        private const double kKeyClickValidityThreshold = 250;
        private static float kDelayBetweenMouseWheelCapture = 100;
        public static bool kWasLmbDragged = false;
        private static List<ImGuiKey> kKeysToCheck = new() { ImGuiKey.Delete, ImGuiKey.C, ImGuiKey.V };
        private static Dictionary<ImGuiKey, bool> kKeysDown = new();
        private static double DeltaLastMouseClick() => ( DateTime.Now - kLastMouseClicked ).TotalMilliseconds;
        private static double DeltaLastKeyClick() => ( DateTime.Now - kLastKeyClicked ).TotalMilliseconds;
        public static bool CheckMouseClickValidity() {
            if( DeltaLastMouseClick() > kMouseClickValidityThreshold ) {
                kLastMouseClicked = DateTime.Now;
                return true;
            }
            return false;
        }
        public static bool CheckKeyClickValidity() {
            if( DeltaLastKeyClick() > kKeyClickValidityThreshold ) {
                kLastKeyClicked = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool mIsHovered = false;
        public bool mIsMouseRmb = false;
        public bool mIsMouseLmb = false;
        public bool mIsMouseMid = false;
        public bool mIsMouseRmbDown = false;
        public bool mIsMouseLmbDown = false;
        public bool mIsKeyShift = false;
        public bool mIsKeyAlt = false;
        public bool mIsKeyCtrl = false;
        public bool mIsKeyDel = false;
        public bool mIsKeyC = false;
        public bool mIsKeyV = false;
        public Vector2 mMousePos = Vector2.Zero;
        private Vector2? mFirstMouseLeftHoldPos = null;
        public bool mIsALmbDragRelease = false;
        public bool mIsMouseDragLeft = false;
        public bool mIsMouseDragRight = false;
        public Vector2? mLmbDragDelta = null;
        public float mMouseWheelValue = 0;

        /// <summary>
        /// ExtraKeyboardInputs:            in case something is blocking ImGui keyboard non-mod input capture,
        ///                                 use this to submit inputs.
        /// </summary>
        public void CaptureInput( bool pCaptureMouseWheel = false, bool pCaptureMouseDrag = false, HashSet<ImGuiKey> pExtraKeyboardInputs = null ) {
            var io = ImGui.GetIO();
            if( io.KeyShift ) mIsKeyShift = true;
            if( io.KeyAlt ) mIsKeyAlt = true;
            if( io.KeyCtrl ) mIsKeyCtrl = true;
            if( io.MouseReleased[0] ) mIsMouseLmb = true;
            if( io.MouseReleased[1] ) mIsMouseRmb = true;
            if( io.MouseReleased[2] ) mIsMouseMid = true;
            if( io.MouseDown[0] ) mIsMouseLmbDown = true;
            if( io.MouseDown[1] ) mIsMouseRmbDown = true;
            this.mMousePos = io.MousePos;
            this.mIsALmbDragRelease = kWasLmbDragged;

            if( pCaptureMouseDrag ) {
                this.CaptureMouseDragDelta();
            }
            if( pCaptureMouseWheel ) this.CaptureMouseWheel();
            if( !this.mIsMouseLmbDown && !this.mIsMouseLmb ) {
                kWasLmbDragged = false;
                this.mIsALmbDragRelease = kWasLmbDragged;
            }

            // extra-inputs
            if( pExtraKeyboardInputs != null ) {
                foreach( var imKey in kKeysToCheck ) {
                    // Try get state: Down
                    if( pExtraKeyboardInputs.Contains( imKey ) ) {
                        if( !kKeysDown.TryAdd( imKey, true ) )
                            kKeysDown[imKey] = true;
                    }
                    else {
                        // Try get state: Released
                        if( kKeysDown.TryGetValue( imKey, out var isKeyDown ) && isKeyDown ) {
                            switch( imKey ) {
                                case ImGuiKey.Delete: mIsKeyDel = true; break;
                                case ImGuiKey.C: mIsKeyC = true; break;
                                case ImGuiKey.V: mIsKeyV = true; break;
                            }
                            kKeysDown[imKey] = false;
                        }
                    }
                }
            }
            if( ImGui.IsKeyPressed( ImGuiKey.Delete ) ) mIsKeyDel = true;
        }
        /// <summary> https://git.anna.lgbt/ascclemens/QuestMap/src/commit/2030f8374eb65a64947b2bc37f35fc53ff3723f4/QuestMap/PluginUi.cs#L857 </summary>
        private Vector2? CaptureMouseDragDeltaInternal() {
            // Get first left hold.
            if( this.mIsMouseLmbDown && this.mFirstMouseLeftHoldPos == null ) { this.mFirstMouseLeftHoldPos = mMousePos; }
            if( this.mIsMouseLmb ) { this.mFirstMouseLeftHoldPos = null; }

            Vector2? tRes = null;
            this.mIsMouseDragLeft = ImGui.IsMouseDragging( ImGuiMouseButton.Left );
            this.mIsMouseDragRight = ImGui.IsMouseDragging( ImGuiMouseButton.Right );
            if( this.mIsMouseDragLeft ) {
                if( kLastMouseDragDelta == null ) {
                    kLastMouseDragDelta = ImGui.GetMouseDragDelta();
                    // Imgui's MouseDelta does not recognize tiny drag under certain threshold (prob to distinguish click vs drag)
                    // So this is a compensation which adds an extra distance equal to that threshold if the node is being dragged.
                    tRes = kLastMouseDragDelta + ( this.mMousePos - this.mFirstMouseLeftHoldPos );
                }
                else {
                    var d = ImGui.GetMouseDragDelta();
                    if( this.mIsMouseDragLeft ) {
                        tRes = ( d - kLastMouseDragDelta );
                        tRes = tRes.Value + tRes.Value * 0;   // dragging loss is around 16% without compensation
                    }
                    kLastMouseDragDelta = d;
                }
            }
            else {
                kLastMouseDragDelta = null;
            }


            // distinguishing between a release from click or drag
            if( this.mIsMouseLmbDown ) {
                if( tRes.HasValue ) kWasLmbDragged = true;
            }
            else if( !this.mIsMouseLmb ) {
                kWasLmbDragged = false;
                this.mIsALmbDragRelease = kWasLmbDragged;
            }
            return tRes;
        }
        public void CaptureMouseDragDelta() {
            this.mLmbDragDelta = this.CaptureMouseDragDeltaInternal();
        }
        private float CaptureMouseWheelInternal() {
            var tRes = ImGui.GetIO().MouseWheel;
            if( tRes != 0
                && ( DateTime.Now - kLastWheelWheeled ).TotalMilliseconds < kDelayBetweenMouseWheelCapture ) {
                kLastWheelWheeled = DateTime.Now;
            }
            return ImGui.GetIO().MouseWheel;
        }
        public void CaptureMouseWheel() {
            this.mMouseWheelValue = this.CaptureMouseWheelInternal();
        }
    }
}
