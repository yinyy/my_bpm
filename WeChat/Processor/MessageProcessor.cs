using WeChat.Utils;
using WeChat.Model;

namespace WeChat.Processor
{
    public interface MessageProcessor
    {
        void process(ReceivedMessageModel message);
    }
}
