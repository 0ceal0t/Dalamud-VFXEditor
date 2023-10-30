using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.Formats.AvfxFormat.Curve.Editor {
    public struct AvfxCurveState {
        public AvfxCurveKeyState[] Points;
    }

    public struct AvfxCurveKeyState {
        public KeyType Type;
        public int Time;
        public float X;
        public float Y;
        public float Z;
    }
}
