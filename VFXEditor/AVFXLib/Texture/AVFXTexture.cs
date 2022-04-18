using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Texture {
    public class AVFXTexture : AVFXBase {
        public const string NAME = "Tex";

        public readonly AVFXString Path = new( "Path" );

        public AVFXTexture() : base( NAME ) {
        }

        public override void ReadContents( BinaryReader reader, int size ) {
            Path.SetValue(Encoding.ASCII.GetString( reader.ReadBytes( size ) ));
        }

        protected override void RecurseChildrenAssigned( bool assigned ) { }

        protected override void WriteContents( BinaryWriter writer ) {
            writer.Write( Encoding.ASCII.GetBytes( Path.GetValue() ) );
        }
    }
}
