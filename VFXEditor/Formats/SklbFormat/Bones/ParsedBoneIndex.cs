using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.Havok.Animation.Rig;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Parsing;

namespace VfxEditor.SklbFormat.Bones {
    public unsafe class ParsedBoneIndex : ParsedShort {
        public ParsedBoneIndex( string name ) : base( name ) { }

        public ParsedBoneIndex( string name, int value ) : base( name, value ) { }

        public string GetText( List<SklbBone> bones ) => Value == -1 ? "[NONE]" : ( Value >= bones.Count ? "[UNKNOWN]" : bones[Value].Name.Value );

        public string GetText( hkaSkeleton* skeleton ) => Value == -1 ? "[NONE]" : ( Value >= skeleton->Bones.Length ? "[UNKNOWN]" : skeleton->Bones[Value].Name.String );

        public void Draw( List<SklbBone> bones ) => Draw( bones.Select( x => x.Name.Value ).ToList() );

        public void Draw( hkaSkeleton* skeleton ) {
            var names = new List<string>();
            for( var i = 0; i < skeleton->Bones.Length; i++ ) {
                names.Add( skeleton->Bones[i].Name.String );
            }
            Draw( names );
        }

        public void Draw( List<string> names ) {
            var text = Value == -1 ? "[NONE]" : ( Value >= names.Count ? "[UNKNOWN]" : names[Value] );

            using var _ = ImRaii.PushId( Name );
            using var combo = ImRaii.Combo( Name, text );
            if( !combo ) return;

            if( ImGui.Selectable( "[NONE]", Value == -1 ) ) {
                CommandManager.Add( new ParsedSimpleCommand<int>( this, -1 ) );
            }

            for( var i = 0; i < names.Count; i++ ) {
                using var __ = ImRaii.PushId( i );
                var selected = i == Value;

                if( ImGui.Selectable( names[i], selected ) ) {
                    CommandManager.Add( new ParsedSimpleCommand<int>( this, i ) );
                }
                if( selected ) ImGui.SetItemDefaultFocus();
            }
        }
    }
}
