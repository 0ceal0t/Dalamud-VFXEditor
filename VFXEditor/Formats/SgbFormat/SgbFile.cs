using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.FileManager;
using VfxEditor.Formats.SgbFormat.Scenes;
using VfxEditor.Ui.Components;

namespace VfxEditor.Formats.SgbFormat {
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Files/SgbFile.cs#L14
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Layer/LayerGroup.cs#L12
    // https://github.com/NotAdam/Lumina/blob/40dab50183eb7ddc28344378baccc2d63ae71d35/src/Lumina/Data/Parsing/Scene/Scene.cs#L8

    // TODO unlock disabled with warning
    // TODO CollisionAttribute, CreationParamsBase, CreationParams
    // TODO SoundEffectParam data
    // TODO Overriden stuff
    // TODO collision config in BGInstanceObject
    // TODO VFX preview
    // TODO Texture preview in light

    public class SgbFile : FileManagerFile {
        public readonly List<SgbEntry> Entries = new();
        private readonly CommandDropdown<SgbEntry> EntryDropdown;

        public SgbFile( BinaryReader reader, bool verify ) : base() {
            reader.ReadInt32(); // magic
            reader.ReadInt32(); // total file size

            var chunkCount = reader.ReadInt32();
            for( var i = 0; i < chunkCount; i++ ) { // TODO: find example with multiple chunks
                Entries.Add( new( this, reader ) );
            }

            // TODO: verify

            EntryDropdown = new( "Entry", Entries, ( SgbEntry item, int idx ) => $"Entry {idx} ({item.Id.Value})", () => new SgbEntry( this ), null, false );
        }

        public override void Write( BinaryWriter writer ) {
            // TODO
        }

        public override void Draw() {
            ImGui.Separator();
            EntryDropdown.Draw();
        }
    }
}
