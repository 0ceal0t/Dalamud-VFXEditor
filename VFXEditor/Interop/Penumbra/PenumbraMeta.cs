using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using VfxEditor.Formats.AtchFormat;

namespace VfxEditor.Interop.Penumbra {
    // Builds Penumbra's binary (version 2) meta manipulation string, as accepted by AddTemporaryModAll.
    // Penumbra rejects the entire string if any identifier fails validation or is duplicated, so entries are filtered here.
    // https://github.com/xivdev/Penumbra/blob/master/Penumbra/Api/Api/MetaApi.cs (ConvertManipsV2 / CompressMetaManipulationsV2)
    public static class PenumbraMeta {
        private const byte Version = 2;
        private static readonly byte[] Magic = Encoding.ASCII.GetBytes( "META0002" );
        private const uint AtchKey = ( 'A' << 24 ) | ( 'T' << 16 ) | ( 'C' << 8 ) | 'H';
        private const int AtchMaxBoneNameLength = AtchPenumbraSupport.MaxBoneNameLength; // + null terminator in a 35 byte buffer

        public static string FromAtch( List<(string GamePath, AtchFile File)> files ) {
            using var entries = new MemoryStream();
            using var entryWriter = new BinaryWriter( entries );
            var count = 0;
            var seen = new HashSet<(uint, ushort, ushort)>();

            foreach( var (gamePath, file) in files ) {
                if( !AtchPenumbraSupport.TryGetGenderRace( gamePath, out var genderRace ) ) {
                    if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"[PenumbraMeta] Skipping {gamePath}, Penumbra only supports .atch edits for playable races" );
                    continue;
                }

                foreach( var entry in file.Entries ) {
                    var type = AtchType( entry.Name.Value );
                    if( type == 0 ) continue;

                    for( var index = 0; index < entry.States.Count; index++ ) {
                        if( !seen.Add( (type, genderRace, ( ushort )index) ) ) continue; // duplicates invalidate everything

                        var state = entry.States[index];
                        var bone = Encoding.ASCII.GetBytes( state.Bone.Value ?? "" );
                        if( bone.Length > AtchMaxBoneNameLength ) {
                            if( Plugin.Configuration?.LogDebug == true ) Dalamud.Log( $"[PenumbraMeta] Skipping {entry.Name.Value} state {index}, bone name is too long" );
                            continue;
                        }

                        // AtchIdentifier: AtchType (uint), GenderRace (ushort), EntryIndex (ushort)
                        entryWriter.Write( type );
                        entryWriter.Write( genderRace );
                        entryWriter.Write( ( ushort )index );

                        // AtchEntry: 35 byte name buffer, name length, scale, offset xyz, rotation xyz
                        var nameBuffer = new byte[AtchMaxBoneNameLength + 1];
                        bone.CopyTo( nameBuffer, 0 );
                        entryWriter.Write( nameBuffer );
                        entryWriter.Write( ( byte )bone.Length );
                        entryWriter.Write( state.Scale.Value );
                        entryWriter.Write( state.Offset.Value.X );
                        entryWriter.Write( state.Offset.Value.Y );
                        entryWriter.Write( state.Offset.Value.Z );
                        entryWriter.Write( state.Rotation.Value.X );
                        entryWriter.Write( state.Rotation.Value.Y );
                        entryWriter.Write( state.Rotation.Value.Z );
                        count++;
                    }
                }
            }

            if( count == 0 ) return string.Empty;

            using var output = new MemoryStream();
            using( var zip = new GZipStream( output, CompressionMode.Compress, true ) )
            using( var writer = new BinaryWriter( zip ) ) {
                writer.Write( Version );
                writer.Write( Magic );
                writer.Write( AtchKey );
                writer.Write( count );
                writer.Write( entries.ToArray() );
            }
            return Convert.ToBase64String( output.ToArray() );
        }

        // "2ax" -> '2' << 16 | 'a' << 8 | 'x', matching Penumbra's AtchType values
        private static uint AtchType( string name ) {
            if( string.IsNullOrEmpty( name ) || name.Length > 4 ) return 0;
            uint ret = 0;
            foreach( var c in name ) ret = ( ret << 8 ) | ( byte )c;
            return ret;
        }
    }
}
