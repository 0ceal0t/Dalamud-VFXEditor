namespace VfxEditor.Interop.Structs.Vfx {
    public abstract class BaseVfx {
        public string Path;

        public BaseVfx( string path ) {
            Path = path;
        }

        public abstract void Remove();

        public abstract nint GetAddress();
    }
}