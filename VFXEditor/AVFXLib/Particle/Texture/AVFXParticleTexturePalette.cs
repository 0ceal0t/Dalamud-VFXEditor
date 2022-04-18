using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFXEditor.AVFXLib.Curve;

namespace VFXEditor.AVFXLib.Particle {
    public class AVFXParticleTexturePalette : AVFXBase {
        public readonly AVFXBool Enabled = new( "bEna" );
        public readonly AVFXEnum<TextureFilterType> TextureFilter = new( "TFT" );
        public readonly AVFXEnum<TextureBorderType> TextureBorder = new( "TBT" );
        public readonly AVFXInt TextureIdx = new( "TxNo" );
        public readonly AVFXCurve Offset = new( "POff" );

        private readonly List<AVFXBase> Children;

        public AVFXParticleTexturePalette() : base( "TP" ) {
            Children = new List<AVFXBase> {
                Enabled,
                TextureFilter,
                TextureBorder,
                TextureIdx,
                Offset
            };
            TextureIdx.SetValue( -1 );
        }

        public override void ReadContents( BinaryReader reader, int size ) => ReadNested( reader, Children, size );

        protected override void RecurseChildrenAssigned( bool assigned ) => RecurseAssigned( Children, assigned );

        protected override void WriteContents( BinaryWriter writer ) => WriteNested( writer, Children );
    }
}
