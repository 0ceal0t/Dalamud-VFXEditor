using Dalamud.Bindings.ImGui;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat {
    public abstract class TmbItem {
        public readonly TmbFile File;

        public abstract string Magic { get; }
        public abstract int Size { get; }
        public abstract int ExtraSize { get; }

        public CommandManager Command => File.Command;

        public TmbItem( TmbFile file ) {
            File = file;
        }

        public TmbItem( TmbFile file, TmbReader reader ) : this( file ) {
            reader.UpdateStartPosition();
            reader.ReadString( 4 ); // magic
            reader.ReadInt32(); // size
        }

        public virtual void Write( TmbWriter writer ) {
            FileUtils.WriteString( writer.Writer, Magic );
            writer.Write( Size );
        }
    }

    public abstract class TmbItemWithId : TmbItem {
        public short Id;

        public TmbItemWithId( TmbFile file ) : base( file ) {
            Id = 0;
        }

        public TmbItemWithId( TmbFile file, TmbReader reader ) : base( file, reader ) {
            Id = reader.ReadInt16();
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            writer.Write( Id );
        }
    }

    public abstract class TmbItemWithTime : TmbItemWithId {
        public ParsedShort Time = new( "Time" );

        public TmbItemWithTime( TmbFile file ) : base( file ) { }

        public TmbItemWithTime( TmbFile file, TmbReader reader ) : base( file, reader ) {
            Time.Read( reader.Reader );
        }

        public override void Write( TmbWriter writer ) {
            base.Write( writer );
            Time.Write( writer.Writer );
        }

        protected void DrawHeader() {
            Time.Draw();
            ImGui.SameLine();
            ImGui.TextDisabled( $"[ ID: {Id} ]" );
        }
    }
}
