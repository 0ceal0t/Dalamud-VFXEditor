using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.AVFXLib;

namespace VfxEditor.AvfxFormat.Vfx {
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
