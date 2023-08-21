using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok {
    public unsafe abstract class HavokData {
        public readonly string Path;
        public readonly hkResource* Resource;
        public readonly hkRootLevelContainer* Container;
        public readonly hkaAnimationContainer* AnimationContainer;

        public HavokData( string havokPath ) {
            Path = havokPath;

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
                        AnimationContainer = ( hkaAnimationContainer* )Container->findObjectByName( n2, null );
                        OnLoad();
                    }
                }

                Marshal.FreeHGlobal( path );
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not read file: {Path}" );
            }
        }

        protected abstract void OnLoad();

        public static hkArray<T> CreateArray<T>( hkArray<T> currentArray, List<T> data, out nint handle ) where T : unmanaged {
            var flags = currentArray.Flags | data.Count;

            var size = Marshal.SizeOf( typeof( T ) );
            var arr = Marshal.AllocHGlobal( size * data.Count + 1 );
            var _arr = ( T* )arr;

            for( var i = 0; i < data.Count; i++ ) {
                _arr[i] = data[i];
            }

            handle = arr;

            var ret = new hkArray<T>() {
                CapacityAndFlags = flags,
                Length = data.Count,
                Data = _arr
            };
            return ret;
        }

        protected void WriteHavok() {
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
                    Marshal.FreeHGlobal( path );
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not export to: {Path}" );
            }
        }

        public virtual void RemoveReference() {
            if( Resource == null ) return;
            var refResource = ( hkReferencedObject* )Resource;
            refResource->RemoveReference();

            var refSkeleton = ( hkReferencedObject* )AnimationContainer;
            refSkeleton->RemoveReference();
        }
    }
}
