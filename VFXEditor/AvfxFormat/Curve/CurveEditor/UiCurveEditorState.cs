using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
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
