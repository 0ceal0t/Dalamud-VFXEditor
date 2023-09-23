using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing.String;
using VfxEditor.Ui.Components;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.SkpFormat.LookAt {
    public class SkpLookAtGroup : IUiItem {
        public readonly ParsedPaddedString Id = new( "Id", 32, 0x00 );
        public readonly List<SkpLookAtGroupElement> Elements = new();

        private readonly CollapsingHeaders<SkpLookAtGroupElement> ElementView;

        public SkpLookAtGroup() {
            ElementView = new( "Element", Elements, null, () => new(), () => CommandManager.Skp );
        }

        public SkpLookAtGroup( BinaryReader reader ) : this() {
            Id.Read( reader );

            var numElems = reader.ReadByte();
            for( var i = 0; i < numElems; i++ ) Elements.Add( new( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            // Sometimes SE doesn't clean up their buffers, so they are filled with junk
            Id.WriteAndPopulateIgnore( writer, SkpFile.VerifyIgnore );

            writer.Write( ( byte )Elements.Count );
            Elements.ForEach( x => x.Write( writer ) );
        }

        public void Draw() {
            Id.Draw( CommandManager.Skp );
            ElementView.Draw();
        }
    }
}
