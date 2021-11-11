using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VFXEditor.Helper;
using VFXEditor.Textools;

namespace VFXEditor.Tmb {
    public partial class TmbManager {
        // ========== PENUMBRA ==============

        public void PenumbraExport( string modFolder, bool exportTmb ) {
            if( !exportTmb ) return;
            var path = TmbReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;

            var data = CurrentFile.ToBytes();
            PenumbraHelper.WriteBytes( data, modFolder, path );
        }

        // ======== TEXTOOLS ============

        public void TextoolsExport( BinaryWriter writer, bool exportTmb, List<TTMPL_Simple> simpleParts, ref int modOffset ) {
            if( !exportTmb ) return;
            var path = TmbReplacePath;
            if( string.IsNullOrEmpty( path ) || CurrentFile == null ) return;

            var modData = TextoolsHelper.CreateType2Data( CurrentFile.ToBytes() );
            simpleParts.Add( TextoolsHelper.CreateModResource( path, modOffset, modData.Length ) );
            writer.Write( modData );
            modOffset += modData.Length;
        }
    }
}
