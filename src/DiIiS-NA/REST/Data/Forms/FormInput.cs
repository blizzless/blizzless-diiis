using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Data.Forms
{
    [DataContract]
    public class FormInput
    {
        [DataMember(Name = "input_id")]
        public string Id { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "max_length")]
        public int MaxLength { get; set; }
    }
}
