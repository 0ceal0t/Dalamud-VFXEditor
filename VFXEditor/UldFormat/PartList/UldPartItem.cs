using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.PartList {
    public class UldPartItem {
        public readonly ParsedUInt TextureId = new( "Texture Id" );
        private readonly ParsedUInt U = new( "U", size: 2 );
        private readonly ParsedUInt V = new( "V", size: 2 );
        private readonly ParsedUInt W = new( "W", size: 2 );
        private readonly ParsedUInt H = new( "H", size: 2 );

        public UldPartItem() { }

        public UldPartItem( BinaryReader reader ) {
            TextureId.Read( reader );
            U.Read( reader );
            V.Read( reader );
            W.Read( reader );
            H.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TextureId.Write( writer );
            U.Write( writer );
            V.Write( writer );
            W.Write( writer );
            H.Write( writer );
        }

        public void Draw( string id ) {
            TextureId.Draw( id, CommandManager.Uld );
            U.Draw( id, CommandManager.Uld );
            V.Draw( id, CommandManager.Uld );
            W.Draw( id, CommandManager.Uld );
            H.Draw( id, CommandManager.Uld );
        }
    }
}
