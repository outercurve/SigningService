namespace Outercurve.ToolsLib.Messages
{
    public class ProgressMessage
    {
        public string Activity { get; set; }
        public int ActivityId { get; set; }
        public string Description { get; set; }
        public int PercentComplete { get; set; }
        public ProgressMessageType MessageType { get; set; }
    }

    public enum ProgressMessageType
    {
        Processing,
        Complete
    }
}
