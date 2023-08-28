using Dalamud.Logging;
using FFXIVClientStructs.Havok;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok {
    public unsafe class HavokData {
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
                    return;
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

        protected virtual void OnLoad() { }

        protected void WriteHavok() {
            try {
                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    var result = stackalloc hkResult[1];

                    var className = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry()->GetClassByName( n1 );

                    PluginLog.Log( $"Writing Havok to {Path}" );

                    var oStream = stackalloc hkOstream[1];
                    oStream->Ctor( Path );

                    var saveOptions = new hkSerializeUtil.SaveOptions {
                        Flags = new hkFlags<hkSerializeUtil.SaveOptionBits, int> {
                            Storage = ( int )hkSerializeUtil.SaveOptionBits.Default
                        }
                    };

                    hkSerializeUtil.Save( result, Container, className, oStream->StreamWriter.ptr, saveOptions );

                    oStream->Dtor();
                }
            }
            catch( Exception e ) {
                PluginLog.Error( e, $"Could not export to: {Path}" );
            }
        }

        public virtual void RemoveReference() {
            if( Resource != null ) {
                var refResource = ( hkReferencedObject* )Resource;
                refResource->RemoveReference();
            }

            if( AnimationContainer != null ) {
                var refContainer = ( hkReferencedObject* )AnimationContainer;
                refContainer->RemoveReference();
            }
        }

        public static List<T> ToList<T>( hkArray<T> array ) where T : unmanaged {
            var ret = new List<T>();
            for( var i = 0; i < array.Length; i++ ) {
                ret.Add( array[i] );
            }
            return ret;
        }

        // ====================

        public static hkArray<T> CreateArray<T>( hkArray<T> currentArray, List<T> data, out nint handle ) where T : unmanaged =>
            CreateArray( currentArray.Flags, data, Marshal.SizeOf( typeof( T ) ), out handle );

        public static hkArray<T> CreateArray<T>( int f, List<T> data, int size, out nint handle ) where T : unmanaged {
            var flags = f | data.Count;

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
    }
}
