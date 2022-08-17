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

namespace DiIiS_NA.REST.Data.Forms
{
    [DataContract]
    public class FormInputs
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "prompt")]
        public string Prompt { get; set; }

        [DataMember(Name = "inputs")]
        public List<FormInput> Inputs { get; set; } = new List<FormInput>();
    }
}
