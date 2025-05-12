using FFXIVClientStructs.Havok.Animation;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Object;
using FFXIVClientStructs.Havok.Common.Base.System.IO.OStream;
using FFXIVClientStructs.Havok.Common.Base.Types;
using FFXIVClientStructs.Havok.Common.Serialize.Resource;
using FFXIVClientStructs.Havok.Common.Serialize.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Havok {
    public unsafe class HavokData {
        public readonly string Path;
        public hkResource* Resource { get; private set; }
        public hkRootLevelContainer* Container { get; private set; }
        public hkaAnimationContainer* AnimationContainer { get; private set; }

        public HavokData( string havokPath, bool init ) {
            Path = havokPath;

            if( init ) Init();
            else Dalamud.Framework.RunOnFrameworkThread( Init );
        }

        public virtual void Init() {
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
                    Dalamud.Error( $"Could not read file: {Path}" );
                    return;
                }

                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    Container = ( hkRootLevelContainer* )Resource->GetContentsPointer( n1, hkBuiltinTypeRegistry.Instance()->GetTypeInfoRegistry() );
                    var animationName = @"hkaAnimationContainer"u8;
                    fixed( byte* n2 = animationName ) {
                        AnimationContainer = ( hkaAnimationContainer* )Container->findObjectByName( n2, null );
                        OnHavokLoad();
                    }
                }

                Marshal.FreeHGlobal( path );
            }
            catch( Exception e ) {
                Dalamud.Error( e, $"Could not read file: {Path}" );
            }
        }

        protected virtual void OnHavokLoad() { }

        protected void WriteHavok() {
            try {
                var rootLevelName = @"hkRootLevelContainer"u8;
                fixed( byte* n1 = rootLevelName ) {
                    var result = stackalloc hkResult[1];

                    var className = hkBuiltinTypeRegistry.Instance()->GetClassNameRegistry()->GetClassByName( n1 );

                    Dalamud.Log( $"Writing Havok to {Path}" );

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
                Dalamud.Error( e, $"Could not export to: {Path}" );
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

        public static hkArray<T> CreateArray<T>( HashSet<nint> handles, List<T> data ) where T : unmanaged =>
            CreateArray( handles, 0x80000000, data, Marshal.SizeOf<T>() );

        public static hkArray<T> CreateArray<T>( HashSet<nint> handles, hkArray<T> currentArray, List<T> data ) where T : unmanaged =>
            CreateArray( handles, ( uint )currentArray.Flags, data, Marshal.SizeOf<T>() );

        public static hkArray<T> CreateArray<T>( HashSet<nint> handles, uint f, List<T> data, int size ) where T : unmanaged {
            var count = data == null ? 0 : data.Count;
            var flags = f | ( uint )count;

            nint arr = 0;

            if( data != null ) {
                arr = Marshal.AllocHGlobal( size * data.Count + 1 );
                handles.Add( arr );
                var _arr = ( T* )arr;

                for( var i = 0; i < data.Count; i++ ) _arr[i] = data[i];
            }

            var ret = new hkArray<T>() {
                CapacityAndFlags = ( int )flags,
                Length = count,
                Data = ( T* )arr
            };
            return ret;
        }
    }
}
