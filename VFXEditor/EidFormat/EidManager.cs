using VfxEditor.FileManager;
using VfxEditor.Select.Eid;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public unsafe class EidManager : FileManagerWindow<EidDocument, EidFile, WorkspaceMetaBasic> {
        public EidManager() : base( "Eid Editor", "Eid" ) {
            SourceSelect = new EidSelectDialog( "Eid Select [LOADED]", this, true );
            ReplaceSelect = new EidSelectDialog( "Eid Select [REPLACED]", this, false );

            /*if( Plugin.ClientState?.LocalPlayer != null ) {
                var a = ( GameObject* )Plugin.ClientState.LocalPlayer.Address;
                var b = a->DrawObject;
                var c = ( CharacterBase* )b;
                // can also get skeleton from this
                var d = ( ResourceHandle* )c->EID;
                var e = Marshal.ReadIntPtr( new IntPtr( d ) + 0xc8 );

                PluginLog.Log( $"{new IntPtr( e ):X8} {d->FileSize} {d->FileSize2} {d->FileSize3}" );

                var num = ( int )Math.Floor( ( double )d->FileSize / 64 );
                var data = new byte[d->FileSize];
                Marshal.Copy( e, data, 0, data.Length );

                using var ms = new MemoryStream( data );
                using var r = new BinaryReader( ms );

                for( var i = 0; i < num; i++ ) {
                    var binder = new EidBindPoint( r );
                    PluginLog.Log( $"{binder.Name.Value} {binder.Id.Value}" );
                }

                // goes to EIDFileResource

                // https://github.com/aers/FFXIVClientStructs/blob/80c299eb8083fd8117fed5d025106806236c0eab/FFXIVClientStructs/FFXIV/Client/System/Resource/Handle/ResourceHandle.cs

                // (FileSize1 - 16) / 64 is # of elements
                // +0xC8 is pointer to the actual data. Can read it the same as EID file, except it skips the header
                // # of bind points is also (maybe?) at +0xC0

                // https://github.com/ktisis-tools/Ktisis/blob/0ef3ac7d51b170dcc55d1f472fed75eb9f9d1b2b/Ktisis/Overlay/Skeleton.cs#L98
            }*/
        }

        protected override EidDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override EidDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) => new( this, NewWriteLocation, localPath, data );
    }
}
