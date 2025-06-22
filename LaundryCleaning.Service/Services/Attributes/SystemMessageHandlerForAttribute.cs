namespace LaundryCleaning.Service.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SystemMessageHandlerForAttribute : Attribute
    {
        public string TopicName { get; }

        public SystemMessageHandlerForAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
