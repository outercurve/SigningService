namespace Outercurve.ToolsLib.Messages
{
    public class Message
    {
         public MessageType MessageType { get; set; }
         public string Contents { get; set; }

    }

    public enum MessageType
    {
        Info,
        Warning
    }
}
