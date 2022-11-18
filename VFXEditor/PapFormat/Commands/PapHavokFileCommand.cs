using System;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapHavokFileCommand : ICommand {
        private readonly Action ChangeAction;
        private readonly string FileLocation;
        private readonly string PreFileLocation;
        private readonly string PostFileLocation;

        public PapHavokFileCommand( string fileLocation, Action changeAction ) {
            FileLocation = fileLocation;
            ChangeAction = changeAction;

            var dir = Path.GetDirectoryName( fileLocation );
            var fileName = Path.GetFileName( fileLocation );
            var randomId = UiUtils.RandomString( 12 );
            PreFileLocation = Path.Combine( dir, $"{randomId}-{fileName}.bak" );
            PostFileLocation = Path.Combine( dir, $"{randomId}-{fileName}" );
            CommandManager.FilesToCleanup.Add( PreFileLocation );
            CommandManager.FilesToCleanup.Add( PostFileLocation );
            File.Copy( fileLocation, PreFileLocation, true ); // backup
        }

        public void Execute() {
            ChangeAction.Invoke();
            File.Copy( FileLocation, PostFileLocation, true );
        }

        public void Redo() {
            File.Copy( PostFileLocation, FileLocation, true );
        }

        public void Undo() {
            File.Copy( PreFileLocation, FileLocation, true );
        }
    }
}
