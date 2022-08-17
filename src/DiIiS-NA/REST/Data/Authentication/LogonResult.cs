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
    public class LogonResult
    {
        [DataMember(Name = "authentication_state")]
        public string AuthenticationState { get; set; }

        [DataMember(Name = "login_ticket")]
        public string LoginTicket { get; set; }

        [DataMember(Name = "error_code")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "error_message")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "support_error_code")]
        public string SupportErrorCode { get; set; }

        [DataMember(Name = "authenticator_form")]
        public FormInputs AuthenticatorForm { get; set; } = new FormInputs();
    }

    public enum AuthenticationState
    {
        NONE = 0,
        LOGIN = 1,
        LEGAL = 2,
        AUTHENTICATOR = 3,
        DONE = 4,
    }
}
