using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Data.Forms
{
    [DataContract]
    public class FormInputValue
    {
        [DataMember(Name = "input_id")]
        public string Id { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
