using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LaundryCleaning.Scheduler.Config
{
    public static class YamlConfigLoader
    {
        public static SchedulerConfig Load(string path)
        {
            var yaml = File.ReadAllText(path);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<SchedulerConfig>(yaml);
        }
    }
}
