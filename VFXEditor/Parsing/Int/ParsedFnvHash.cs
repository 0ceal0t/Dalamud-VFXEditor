using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.Parsing.Int {
    public class ParsedFnvHash : ParsedSimpleBase<(string, uint)> {
        private bool Editing = false;
        private DateTime LastEditTime = DateTime.Now;
        private (string, uint) StateBeforeEdit = ("", 0);

        public string Data => Value.Item1;
        public uint Hash => Value.Item2;

        public ushort NameOffset { get; set; }

        public ParsedFnvHash( string name ) : base( name ) {
            Value = ("", 0);
        }

        public override void Read( BinaryReader reader ) {
            Value = ("", reader.ReadUInt32());
            NameOffset = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
        }

        public void Read( BinaryReader reader, string guess ) {
            var hash = reader.ReadUInt32();
            Value = (FnvUtils.Encode( guess ) == hash ? guess : "", hash);
            NameOffset = reader.ReadUInt16();
            reader.ReadUInt16(); // padding
        }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Write( BinaryWriter writer ) {
            writer.Write( Value.Item2 );
            writer.Write( NameOffset );
            writer.Write( ( ushort )0 );
        }

        public void Guess( List<string> guesses ) {
            if( !string.IsNullOrEmpty( Data ) ) return;
            foreach( var guess in guesses ) {
                if( Guess( guess ) ) return;
            }
        }

        public bool Guess( string guess ) {
            if( !string.IsNullOrEmpty( Data ) ) return false; // already assigned
            if( FnvUtils.Encode( guess ) == Hash ) {
                Value = (guess, Hash);
                return true;
            }
            return false;
        }

        public string GetText() => string.IsNullOrEmpty( Data ) ? $"0x{Hash:X8}" : Data;

        protected override void DrawBody() {
            var prevValue = Value;
            var inputValue = Value.Item1;
            if( ImGui.InputTextWithHint( Name, $"0x{Hash:X8}", ref inputValue, 255 ) ) {
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
