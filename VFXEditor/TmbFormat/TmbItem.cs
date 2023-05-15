using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public abstract class TmbItem {
        public readonly bool PapEmbedded;

        public abstract string Magic { get; }
        public abstract int Size { get; }
        public abstract int ExtraSize { get; }

        public CommandManager Command => PapEmbedded ? CommandManager.Pap : CommandManager.Tmb;

        public TmbItem( bool papEmbedded ) {
            PapEmbedded = papEmbedded;
        }

        public TmbItem( TmbReader reader, bool papEmbedded ) : this( papEmbedded ) {
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

        public TmbItemWithId( bool papEmbedded ) : base( papEmbedded ) {
            Id = 0;
        }

        protected TmbItemWithId( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) { }

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

        public TmbItemWithTime( bool papEmbedded ) : base( papEmbedded ) { }

        public TmbItemWithTime( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) { }

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
