using Dalamud.Logging;
using System;
using VfxEditor.FileManager;
using VfxEditor.Select.Eid;
using VfxEditor.Utils;

namespace VfxEditor.EidFormat {
    public unsafe class EidManager : FileManagerWindow<EidDocument, EidFile, WorkspaceMetaBasic> {
        public EidManager() : base( "Eid Editor", "Eid", "eid", "Eid", "Eid" ) {
            SourceSelect = new EidSelectDialog( "Eid Select [LOADED]", this, true );
            ReplaceSelect = new EidSelectDialog( "Eid Select [REPLACED]", this, false );
            
            /*
            if( Plugin.ClientState?.LocalPlayer != null ) {
                var a = ( GameObject* )Plugin.ClientState.LocalPlayer.Address;
                var b = a->DrawObject;
                var c = ( CharacterBase* )b;
                // can also get skeleton from this
                var d = c->EID;

                PluginLog.Log( $"{new IntPtr(d):X8}" );

                // goes to EIDFileResource

                // https://github.com/aers/FFXIVClientStructs/blob/80c299eb8083fd8117fed5d025106806236c0eab/FFXIVClientStructs/FFXIV/Client/System/Resource/Handle/ResourceHandle.cs

                // (FileSize1 - 16) / 64 is # of elements
                // +0xC8 is pointer to the actual data. Can read it the same as EID file, except it skips the header
                // # of bind points is also (maybe?) at +0xC0

                // https://github.com/ktisis-tools/Ktisis/blob/0ef3ac7d51b170dcc55d1f472fed75eb9f9d1b2b/Ktisis/Overlay/Skeleton.cs#L98
            }
            */
        }

        protected override EidDocument GetNewDocument() => new( this, NewWriteLocation );

        protected override EidDocument GetWorkspaceDocument( WorkspaceMetaBasic data, string localPath ) =>
            new( this, NewWriteLocation, WorkspaceUtils.ResolveWorkspacePath( data.RelativeLocation, localPath ), data.Source, data.Replace );
    }
}
