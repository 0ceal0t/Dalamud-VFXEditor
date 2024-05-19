namespace VfxEditor.Ui.NodeGraphViewer.Content {
    public class NodeContentError : NodeContent {
        public new const string nodeContentType = "nodeContentError";
        public override string _contentType => NodeContentError.nodeContentType;
        public NodeContentError( string header, string description ) : base( header, description ) { }
    }
}
