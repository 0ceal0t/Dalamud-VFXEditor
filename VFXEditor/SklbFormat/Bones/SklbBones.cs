using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using System;
using System.Runtime.InteropServices;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class SklbBones {
        public readonly string Path;
        public readonly hkResource* Resource;
        public readonly hkRootLevelContainer* Container;
        public readonly hkaSkeleton* Skeleton;

        public SklbBones( string loadPath ) {
            Path = loadPath;

            try {
                var path = Marshal.StringToHGlobalAnsi( Path );

                var loadOptions = stackalloc hkSerializeUtil.LoadOptions[1];
                loadOptions->TypeInfoRegistry = hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry();
                loadOptions->ClassNameRegistry = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry();
                loadOptions->Flags = new hkFlags<hkSerializeUtil.LoadOptionBits, int> {
                    Storage = ( int )hkSerializeUtil.LoadOptionBits.Default
                };

                Resource = hkSerializeUtil.LoadFromFile( ( byte* )path, null, loadOptions );

                if( Resource == null ) {
                    PluginLog.Error( $"Could not read file: {Path}" );
                }

                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    Container = ( hkRootLevelContainer* )Resource->GetContentsPointer( n1, hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry() );
                    var animationName = @"hkaAnimationContainer"u8;
                    fixed( byte* n2 = animationName ) {
                        var anim = ( hkaAnimationContainer* )Container->findObjectByName( n2, null );
                        Skeleton = anim->Skeletons[0].ptr;
                        OnLoad();
                    }
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not read file: {Path}" );
            }
        }

        protected virtual void OnLoad() {
        }

        public void Update() {
            try {
                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    var result = stackalloc hkResult[1];

                    var className = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry()->GetClassByName( n1 );

                    var path = Marshal.StringToHGlobalAnsi( Path );
                    var oStream = new hkOstream();
                    oStream.Ctor( ( byte* )path );

                    var saveOptions = new hkSerializeUtil.SaveOptions {
                        Flags = new hkFlags<hkSerializeUtil.SaveOptionBits, int> {
                            Storage = ( int )hkSerializeUtil.SaveOptionBits.Default
                        }
                    };

                    hkSerializeUtil.Save( result, Container, className, oStream.StreamWriter.ptr, saveOptions );

                    oStream.Dtor();
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not export to: {Path}" );
            }
        }

        public void Dispose() {
            if( Resource == null ) return;
            ( ( hkReferencedObject* )Resource )->RemoveReference();
        }
    }
}
