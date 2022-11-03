using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using VfxEditor;
using VfxEditor.Dialogs;
using VfxEditor.Utils;
using VfxEditor.TexTools;

namespace VFXEditor.Animation {
    public class AnimationManager {
        protected string SklHkxTemp => Path.Combine( Plugin.Configuration.WriteLocation, $"skl_temp.hkx" ).Replace( '\\', '/' );
        private string BinTemp => Path.Combine( Plugin.Configuration.WriteLocation, $"anim_out.bin" ).Replace( '\\', '/' );

        public AnimationManager() {

        }

        public void Load( string animationHxk, int animationIndex, string sklbPath ) {

        }

        public void Dispose() {
            if( File.Exists( SklHkxTemp ) ) File.Delete( SklHkxTemp );
            if( File.Exists( BinTemp ) ) File.Delete( BinTemp );
        }
    }
}
