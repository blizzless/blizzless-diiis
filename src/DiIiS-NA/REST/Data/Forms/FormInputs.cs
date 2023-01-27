using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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
