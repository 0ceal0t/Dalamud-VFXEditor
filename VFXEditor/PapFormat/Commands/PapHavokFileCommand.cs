using System;
using System.IO;
using VfxEditor.Utils;

namespace VfxEditor.PapFormat {
    public class PapHavokFileCommand : ICommand {
        private readonly PapAnimation Animation;
        private readonly Action ChangeAction;
        private readonly string FileLocation;
        private readonly string PreFileLocation;
        private readonly string PostFileLocation;

        public PapHavokFileCommand( PapAnimation animation, string fileLocation, Action changeAction ) {
            Animation = animation;
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
            ClearAnimation();
            ChangeAction.Invoke();
            File.Copy( FileLocation, PostFileLocation, true );
        }

        public void Redo() {
            ClearAnimation();
            File.Copy( PostFileLocation, FileLocation, true );
        }

        public void Undo() {
            ClearAnimation();
            File.Copy( PreFileLocation, FileLocation, true );
        }

        private void ClearAnimation() {
            Animation.Skeleton.ClearData();
            if( Plugin.DirectXManager.PapPreview.CurrentAnimation == Animation ) Plugin.DirectXManager.PapPreview.ClearAnimation();
        }
    }
}
