//Blizzless Project 2022 
using DiIiS_NA.REST.Data.Forms;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Runtime.Serialization;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
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
