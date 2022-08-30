using VFXEditor.Helper;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat {
    public abstract class TmbItem {
        public abstract string Magic { get; }
        public abstract int Size { get; }
        public abstract int ExtraSize { get; }

        public TmbItem() { }

        public TmbItem( TmbReader reader ) {
            reader.UpdateStartPosition();
        }

        protected virtual void ReadHeader( TmbReader reader ) {
            reader.ReadString( 4 ); // magic
            reader.ReadInt32(); // size
        }

        protected virtual void WriteHeader( TmbWriter writer ) {
            FileHelper.WriteString( writer.Writer, Magic );
            writer.Write( Size );
        }

        public abstract void Write( TmbWriter writer );
    }

    public abstract class TmbItemWithId : TmbItem {
        public short Id;

        public TmbItemWithId() : base() {
            Id = 0;
        }

        protected TmbItemWithId( TmbReader reader ) : base( reader ) { }

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
        public short Time;

        public TmbItemWithTime() : base() {
            Time = 0;
        }

        public TmbItemWithTime( TmbReader reader ) : base( reader ) { }

        protected override void ReadHeader( TmbReader reader ) {
            base.ReadHeader( reader );
            Time = reader.ReadInt16();
        }

        protected override void WriteHeader( TmbWriter writer ) {
            base.WriteHeader( writer );
            writer.Write( Time );
        }
    }
}
