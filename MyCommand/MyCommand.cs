using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MyCommand
{
    [Serializable]
    [DataContract]
    public class MyCommand
    {
        [DataMember]
        public string NameOfCommand { get; set; }
        [DataMember]
        public int IDProcess { get; set; }
        [DataMember]
        public string Path { get; set; }

        public MyCommand() { }

    }
}