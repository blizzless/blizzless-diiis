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
    public class FormInputValue
    {
        [DataMember(Name = "input_id")]
        public string Id { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
