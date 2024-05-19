namespace VfxEditor.Ui.NodeGraphViewer.Content {
    public class NodeContent {
        public const string nodeContentType = "nodeContent";
        public virtual string _contentType { get; } = NodeContent.nodeContentType;
        private string header = "";
        private string description = "";

        public NodeContent() { }
        public NodeContent( string header ) { this.header = header; }
        public NodeContent( string header, string description ) { this.header = header; this.description = description; }

        public string GetHeader() => this.header;
        public void _setHeader( string header ) => this.header = header;
        public string GetDescription() => this.description;
        public void SetDescription( string description ) => this.description = description;
    }
}
