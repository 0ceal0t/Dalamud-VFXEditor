using ImGuiNET;
using System;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedFnvHash : ParsedSimpleBase<(string, uint)> {
        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private (string, uint) StateBeforeEdit = ("", 0);

        public string Data => Value.Item1;
        public uint Hash => Value.Item2;

        public ParsedFnvHash( string name ) : base( name ) { }

        public ParsedFnvHash( string name, (string, uint) value ) : base( name, value ) { }

        public override void Read( BinaryReader reader ) {
            Value = ("", reader.ReadUInt32());
        }

        public void Read( BinaryReader reader, string guess ) {
            var hash = reader.ReadUInt32();
            Value = (FnvUtils.Encode( guess ) == hash ? guess : "", hash);
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Write( BinaryWriter writer ) => writer.Write( Value.Item2 );

        public bool Guess( string guess ) {
            if( !string.IsNullOrEmpty( Data ) ) return false; // already assigned
            if( FnvUtils.Encode( guess ) == Hash ) {
                Value = (guess, Hash);
                return true;
            }
            return false;
        }

        protected override void DrawBody() {
            var prevValue = Value;
            var inputValue = Value.Item1;
            if( ImGui.InputTextWithHint( Name, $"0x{Value.Item2:X8}", ref inputValue, 255 ) ) {
                Value = (inputValue, FnvUtils.Encode( inputValue ));

                if( !Editing ) {
                    Editing = true;
                    StateBeforeEdit = prevValue;
                }
                LastEditTime = DateTime.Now;
            }
            else if( Editing && ( DateTime.Now - LastEditTime ).TotalMilliseconds > 200 ) {
                Editing = false;
                Update( StateBeforeEdit, Value );
            }
        }
    }
}
