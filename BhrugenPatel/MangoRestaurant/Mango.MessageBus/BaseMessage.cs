namespace Mango.MessageBus
{
    public class BaseMessage
    {
        public int MsgId { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}