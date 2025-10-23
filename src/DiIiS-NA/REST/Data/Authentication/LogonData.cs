using DiIiS_NA.REST.Data.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Data.Authentication
{
    [DataContract]
    public class LogonData
    {
        public string this[string inputId] => Inputs.SingleOrDefault(i => i.Id == inputId)?.Value;

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "program_id")]
        public string Program { get; set; }

        [DataMember(Name = "platform_id")]
        public string Platform { get; set; }

        [DataMember(Name = "inputs")]
        public List<FormInputValue> Inputs { get; set; } = new List<FormInputValue>();
    }
}
