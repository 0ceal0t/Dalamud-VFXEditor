using System;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.AVFXLib;

namespace VfxEditor.Data {
    public class UndoManager {
        private struct UndoRedoStruct {
            public Action Undo;
            public Action Redo;
        }

        private static readonly int MAX_UNDO = 5;
        private static int NumUndo = 0;
        private static readonly List<UndoRedoStruct> UndoRedo = new();

        public static void Reset() {
            NumUndo = 0;
            UndoRedo.Clear();
        }

        public static void AddUndo() {
            NumUndo = 0; // reset
            // Add actions
            // Remove from tail if over maximum
        }

        public static void Undo() {
            // numUndo++
        }

        public static void Redo() {
            // If have numUndo--
        }

        public static void Dispose() {
            UndoRedo.Clear();
        }
    }
}
