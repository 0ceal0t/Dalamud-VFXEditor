using System;
using System.Collections.Generic;
using System.IO;
using static VfxEditor.AvfxFormat.Enums;

namespace VfxEditor.AvfxFormat {
    public class AvfxLife : AvfxOptional {
        public bool Enabled = true;
        // Life is kinda strange, can either be -1 (4 bytes = ffffffff) or Val + ValR + RanT

        public readonly AvfxFloat Value = new( "Value", "Val" );
        public readonly AvfxFloat ValRandom = new( "Random Value", "ValR" );
        public readonly AvfxEnum<RandomType> ValRandomType = new( "Random Type", "Type" );

        private readonly List<AvfxBase> Parsed;
        private readonly List<IAvfxUiBase> Display;

        public AvfxLife() : base( "Life" ) {
            Value.SetValue( -1 );
            Parsed = new() {
                Value,
                ValRandom,
                ValRandomType
            };

            Display = new() {
                Value,
                ValRandom,
                ValRandomType
            };
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            if( size > 4 ) {
                Enabled = true;
                ReadNested( reader, Parsed, size );
                return;
            }
            Enabled = false;
        }

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Parsed, assigned );

        protected override void WriteContents( BinaryWriter writer ) {
            if( !Enabled ) {
                writer.Write( -1 );
                return;
            }
            WriteNested( writer, Parsed );
        }

        public override void DrawAssigned( string parentId ) {
            var id = parentId + "/Life";
            AssignedCopyPaste( this, GetDefaultText() );
            IAvfxUiBase.DrawList( Display, id );
        }

        public override void DrawUnassigned( string id ) {
            AssignedCopyPaste( this, GetDefaultText() );
            DrawAddButtonRecurse( this, GetDefaultText(), id );
        }

        public override string GetDefaultText() => "Life";
    }
}
