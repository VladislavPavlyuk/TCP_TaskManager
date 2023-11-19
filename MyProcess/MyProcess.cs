using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MyProc
{
    [DataContract]
    public class MyProcess
    {
        [DataMember]
        public int ProcessId { get; set; }
        [DataMember]
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public MyProcess() { }
       

    }
}