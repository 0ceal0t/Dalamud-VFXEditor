using System;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxEnum<T> : AvfxLiteral<ParsedEnum<T>, T> where T : Enum {
        public Func<ICommand> Command {
            get => Parsed.ExtraCommand;
            set {
                Parsed.ExtraCommand = value;
            }
        }

        public AvfxEnum( string name, string avfxName, T value, Func<ICommand> command = null ) : base( avfxName, new( name, value, command ) ) { }

        public AvfxEnum( string name, string avfxName, Func<ICommand> command = null ) : base( avfxName, new( name, command ) ) { }

        // Ignore size here
        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader );
    }
}
