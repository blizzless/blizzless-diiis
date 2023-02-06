using System;
using System.Collections.Generic;
using System.Text;

namespace DiIiS_NA.REST
{
    public sealed class RestConfig : Core.Config.Config
    {
        public string IP
        {
            get => GetString("IP", "127.0.0.1");
            set => Set("IP", value);
        }

        public bool Public
        {
            get => GetBoolean("Public", false);
            set => Set("Public", value);
        }

        public string PublicIP
        {
            get => GetString("PublicIP", "0.0.0.0");
            set => Set("PublicIP", value);
        }

        public int Port
        {
            get => GetInt("PORT", 8081);
            set => Set("PORT", value);
        } //8081

        public static RestConfig Instance { get; } = new();

        private RestConfig() : base("REST")
        {
        }
    }
}
