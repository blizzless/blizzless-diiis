using DiIiS_NA.REST.Data.Forms;
using DiIiS_NA.REST.Extensions;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace DiIiS_NA.REST.Manager
{
    public class SessionManager : Singleton<SessionManager>
    {
        SessionManager()
        {
            _formInputs = new FormInputs();
        }

        public bool Initialize()
        {
            int _port = RestConfig.Instance.Port;
            if (_port < 0 || _port > 0xFFFF)
            {
                _port = 8081;
            }

            string configuredAddress = RestConfig.Instance.IP;
            IPAddress address;
            if (!IPAddress.TryParse(configuredAddress, out address))
            {
                return false;
            }
            _externalAddress = new IPEndPoint(address, _port);

            configuredAddress = RestConfig.Instance.IP;
            if (!IPAddress.TryParse(configuredAddress, out address))
            {
                return false;
            }

            _localAddress = new IPEndPoint(address, _port);

            _formInputs.Type = "LOGIN_FORM";

            var input = new FormInput();
            input.Id = "account_name";
            input.Type = "text";
            input.Label = "E-mail";
            input.MaxLength = 320;
            _formInputs.Inputs.Add(input);

            input = new FormInput();
            input.Id = "password";
            input.Type = "password";
            input.Label = "Password";
            input.MaxLength = 16;
            _formInputs.Inputs.Add(input);

            input = new FormInput();
            input.Id = "log_in_submit";
            input.Type = "submit";
            input.Label = "Log In";
            _formInputs.Inputs.Add(input);

            _certificate = new X509Certificate2("BNetServer.pfx");

            return true;
        }

        public IPEndPoint GetAddressForClient(IPAddress address)
        {
            if (IPAddress.IsLoopback(address))
                return _localAddress;

            return _externalAddress;
        }

        public FormInputs GetFormInput()
        {
            return _formInputs;
        }

        public X509Certificate2 GetCertificate()
        {
            return _certificate;
        }

        FormInputs _formInputs;
        IPEndPoint _externalAddress;
        IPEndPoint _localAddress;
        X509Certificate2 _certificate;
    }

    public enum BanMode
    {
        Ip = 0,
        Account = 1
    }
}
