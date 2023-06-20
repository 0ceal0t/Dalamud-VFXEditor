using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public abstract class TmbItem {
        public readonly TmbFile File;
        public readonly bool PapEmbedded;

        public abstract string Magic { get; }
        public abstract int Size { get; }
        public abstract int ExtraSize { get; }

        public CommandManager Command => PapEmbedded ? CommandManager.Pap : CommandManager.Tmb;

        public TmbItem( TmbFile file ) {
            File = file;
            PapEmbedded = file.PapEmbedded;
        }

        public TmbItem( TmbFile file, TmbReader reader ) : this( file ) {
            reader.UpdateStartPosition();
        }

        protected virtual void ReadHeader( TmbReader reader ) {
            reader.ReadString( 4 ); // magic
            reader.ReadInt32(); // size
        }

        protected virtual void WriteHeader( TmbWriter writer ) {
            FileUtils.WriteString( writer.Writer, Magic );
            writer.Write( Size );
        }

        public abstract void Write( TmbWriter writer );
    }

    public abstract class TmbItemWithId : TmbItem {
        public short Id;

        public TmbItemWithId( TmbFile file ) : base( file ) {
            Id = 0;
        }

        protected TmbItemWithId( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override void ReadHeader( TmbReader reader ) {
            base.ReadHeader( reader );
            Id = reader.ReadInt16();
        }

        protected override void WriteHeader( TmbWriter writer ) {
            base.WriteHeader( writer );
            writer.Write( Id );
        }
    }

    public abstract class TmbItemWithTime : TmbItemWithId {
        public ParsedShort Time = new( "Time" );

        public TmbItemWithTime( TmbFile file ) : base( file ) { }

        public TmbItemWithTime( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override void ReadHeader( TmbReader reader ) {
            base.ReadHeader( reader );
            Time.Read( reader.Reader );
        }

        protected override void WriteHeader( TmbWriter writer ) {
            base.WriteHeader( writer );
            Time.Write( writer.Writer );
        }

        protected void DrawHeader() {
            Time.Draw( Command );
            ImGui.SameLine();
            ImGui.TextDisabled( $"[ ID: {Id} ]" );
        }
    }
}
