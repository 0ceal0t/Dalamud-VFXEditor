using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Text;
using VfxEditor.Flatbuffer;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.PbdFormat.Extended {
    public class PbdEpbdEntry : IUiItem {
        public readonly ParsedUInt Child = new( "Child" );
        public readonly ParsedUInt Parent = new( "Parent" );
        public readonly List<uint> Unknown1 = [];
        public readonly List<sbyte> Unknown2 = [];

        public PbdEpbdEntry() { }

        public PbdEpbdEntry( EpbdEntry entry ) : this() {
            Child.Value = entry.ChildRaceCode;
            Parent.Value = entry.ParentRaceCode;
            Unknown1.AddRange( entry.Unknown );
            Unknown2.AddRange( entry.Pbd );
        }

        public void Draw() {
            Child.Draw();
            Parent.Draw();
            ImGui.TextDisabled( $"{Unknown1.Count} integers | 0x{Unknown2.Count:X4} bytes" );
        }

        public EpbdEntry Export() => new() {
            ChildRaceCode = Child.Value,
            ParentRaceCode = Parent.Value,
            Unknown = Unknown1,
            Pbd = Unknown2
        };
    }
}
