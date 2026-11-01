using System.IO;
using VfxEditor.Data;
using VfxEditor.FileManager;
using VfxEditor.Utils;

namespace VfxEditor.Formats.ScdFormat {
    public class ScdManagerGroup : FileManagerGroup<ScdManager, ScdDocument, ScdFile, WorkspaceMetaBasic> {
        public static string ConvertWav => Path.Combine( Plugin.Configuration.WriteLocation, "temp_out.wav" ).Replace( '\\', '/' );
        public static string ConvertOgg => Path.Combine( Plugin.Configuration.WriteLocation, "temp_out.ogg" ).Replace( '\\', '/' );
        public static string ConvertHca => Path.Combine( Plugin.Configuration.WriteLocation, "temp_out.hca" ).Replace( '\\', '/' );

        public ScdManagerGroup() : base( "Scd Editor", "Scd" ) { }

        protected override ScdManager GetNewManager() => new( this );
    }
}
