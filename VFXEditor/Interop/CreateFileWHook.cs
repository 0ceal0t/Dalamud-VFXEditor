using Dalamud.Hooking;
using Penumbra.String.Classes;
using Penumbra.String.Functions;
using Penumbra.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VfxEditor.Interop {
    public unsafe class CreateFileWHook : IDisposable {
        public const int RequiredSize = 28;

        // The prefix is not valid for any actual path, so should never run into false-positives.
        private const char Prefix = ( char )( ( byte )'P' | ( ( '?' & 0x00FF ) << 8 ) );
        private const int BufferSize = Utf8GamePath.MaxGamePathLength;

        [DllImport( "kernel32.dll" )]
        private static extern nint LoadLibrary( string dllName );

        [DllImport( "kernel32.dll" )]
        private static extern nint GetProcAddress( nint hModule, string procName );

        private delegate nint CreateFileWDelegate( char* fileName, uint access, uint shareMode, nint security, uint creation, uint flags, nint template );

        private readonly Hook<CreateFileWDelegate> _createFileWHook;

        /// <summary> Some storage to skip repeated allocations. </summary>
        private readonly ThreadLocal<nint> _fileNameStorage = new( SetupStorage, true );

        public CreateFileWHook() {
            var userApi = LoadLibrary( "kernel32.dll" );
            var createFileAddress = GetProcAddress( userApi, "CreateFileW" );
            _createFileWHook = Hook<CreateFileWDelegate>.FromAddress( createFileAddress, CreateFileWDetour );
        }

        /// <remarks> Long paths in windows need to start with "\\?\", so we keep this static in the pointers. </remarks>
        private static nint SetupStorage() {
            var ptr = ( char* )Marshal.AllocHGlobal( 2 * BufferSize );
            ptr[0] = '\\';
            ptr[1] = '\\';
            ptr[2] = '?';
            ptr[3] = '\\';
            ptr[4] = '\0';
            return ( nint )ptr;
        }

        public void Enable()
            => _createFileWHook.Enable();

        public void Disable()
            => _createFileWHook.Disable();

        public void Dispose() {
            _createFileWHook.Dispose();
            foreach( var ptr in _fileNameStorage.Values ) {
                Marshal.FreeHGlobal( ptr );
            }
        }

        private nint CreateFileWDetour( char* fileName, uint access, uint shareMode, nint security, uint creation, uint flags, nint template ) {
            // Translate data if prefix fits.
            if( CheckPtr( fileName, out var name ) ) {
                // Use static storage.
                var ptr = WriteFileName( name );
                return _createFileWHook.Original( ptr, access, shareMode, security, creation, flags, template );
            }

            return _createFileWHook.Original( fileName, access, shareMode, security, creation, flags, template );
        }


        /// <remarks>Write the UTF8-encoded byte string as UTF16 into the static buffers,
        /// replacing any forward-slashes with back-slashes and adding a terminating null-wchar_t.</remarks>
        private char* WriteFileName( ReadOnlySpan<byte> actualName ) {
            var span = new Span<char>( ( char* )_fileNameStorage.Value + 4, BufferSize - 4 );
            var written = Encoding.UTF8.GetChars( actualName, span );
            for( var i = 0; i < written; ++i ) {
                if( span[i] == '/' ) {
                    span[i] = '\\';
                }
            }

            span[written] = '\0';

            return ( char* )_fileNameStorage.Value;
        }


        public static void WritePtr( char* buffer, byte* address, int length ) {
            // Set the prefix, which is not valid for any actual path.
            buffer[0] = Prefix;

            var ptr = ( byte* )buffer;
            var v = ( ulong )address;
            var l = ( uint )length;

            // Since the game calls wstrcpy without a length, we need to ensure
            // that there is no wchar_t (i.e. 2 bytes) of 0-values before the end.
            // Fill everything with 0xFF and use every second byte.
            MemoryUtility.MemSet( ptr + 2, 0xFF, 23 );

            // Write the byte pointer.
            ptr[2] = ( byte )( v >> 0 );
            ptr[4] = ( byte )( v >> 8 );
            ptr[6] = ( byte )( v >> 16 );
            ptr[8] = ( byte )( v >> 24 );
            ptr[10] = ( byte )( v >> 32 );
            ptr[12] = ( byte )( v >> 40 );
            ptr[14] = ( byte )( v >> 48 );
            ptr[16] = ( byte )( v >> 56 );

            // Write the length.
            ptr[18] = ( byte )( l >> 0 );
            ptr[20] = ( byte )( l >> 8 );
            ptr[22] = ( byte )( l >> 16 );
            ptr[24] = ( byte )( l >> 24 );

            ptr[RequiredSize - 2] = 0;
            ptr[RequiredSize - 1] = 0;
        }

        private static bool CheckPtr( char* buffer, out ReadOnlySpan<byte> fileName ) {
            if( buffer[0] is not Prefix ) {
                fileName = ReadOnlySpan<byte>.Empty;
                return false;
            }

            var ptr = ( byte* )buffer;

            // Read the byte pointer.
            var address = 0ul;
            address |= ( ulong )ptr[2] << 0;
            address |= ( ulong )ptr[4] << 8;
            address |= ( ulong )ptr[6] << 16;
            address |= ( ulong )ptr[8] << 24;
            address |= ( ulong )ptr[10] << 32;
            address |= ( ulong )ptr[12] << 40;
            address |= ( ulong )ptr[14] << 48;
            address |= ( ulong )ptr[16] << 56;

            // Read the length.
            var length = 0u;
            length |= ( uint )ptr[18] << 0;
            length |= ( uint )ptr[20] << 8;
            length |= ( uint )ptr[22] << 16;
            length |= ( uint )ptr[24] << 24;

            fileName = new ReadOnlySpan<byte>( ( void* )address, ( int )length );
            return true;
        }
    }
}
