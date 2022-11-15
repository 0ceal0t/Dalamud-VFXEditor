using static VfxEditor.AvfxFormat2.Enums;

namespace VfxEditor.AvfxFormat2 {
    public struct UiCurveEditorState {
        public UiCurveEditorPointState[] Points;
    }

    public struct UiCurveEditorPointState {
        public KeyType Type;
        public int Time;
        public float X;
        public float Y;
        public float Z;
    }
}
