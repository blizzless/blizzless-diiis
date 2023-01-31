//Blizzless Project 2022
using System;
using System.Collections.Generic;
using System.Text;

namespace DiIiS_NA.REST
{
    public sealed class Config : Core.Config.Config
    {
        public string IP { get { return this.GetString("IP", "127.0.0.1"); } set { this.Set("IP", value); } }
        public bool Public { get { return this.GetBoolean("Public", false); } set { this.Set("Public", value); } }
        public string PublicIP { get { return this.GetString("PublicIP", "0.0.0.0"); } set { this.Set("PublicIP", value); } }
        public int Port { get { return this.GetInt("PORT", 8081); } set { this.Set("PORT", value); } } //8081
        private static readonly Config _instance = new Config();
        public static Config Instance { get { return _instance; } }
        private Config() : base("REST") { }
    }
}
