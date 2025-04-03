using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.Formats.AvfxFormat.Curve.Editor {
    public struct LineEditorState {
        public LineEditorKeyState[] Points;
    }

    public struct LineEditorKeyState {
        public KeyType Type;
        public int Time;
        public float X;
        public float Y;
        public float Z;
    }
}
