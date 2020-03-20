using Newtonsoft.Json.Linq;

namespace WarnMe_Cherry.Interfaces
{
    internal interface IDataEntry
    {
        string DATA_ID { get; }

        void New();
        void Load(JObject data);
        JObject Save();
    }
}
