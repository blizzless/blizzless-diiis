using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.LoginServer
{
	public sealed class LoginServerConfig : Core.Config.Config
	{
		public bool Enabled
		{
			get => GetBoolean(nameof(Enabled), true);
			set => Set(nameof(Enabled), value);
		}

		public string BindIP
		{
			get => GetString(nameof(BindIP), "127.0.0.1");
			set => Set(nameof(BindIP), value);
		}

		public int WebPort
		{
			get => GetInt(nameof(WebPort), 9000);
			set => Set(nameof(WebPort), value);
		}

		public int Port
		{
			get => GetInt(nameof(Port), 1119);
			set => Set(nameof(Port), value);
		}

		[Obsolete]
		public string BindIPv6
		{
			get => GetString(nameof(BindIPv6), "::1");
			set => Set(nameof(BindIPv6), value);
		}

		/// <summary>
		/// Whether Motd should be displayed on login.
		/// </summary>
		public bool MotdEnabled
		{
			get => GetBoolean(nameof(MotdEnabled), true);
			set => Set(nameof(MotdEnabled), value);
		}
		
		/// <summary>
		///	Motd text
		/// </summary>
		public string Motd
		{
			get => GetString(nameof(Motd),
				$"Welcome to Blizzless Server Build {Program.Build} - Stage: {Program.Stage} [{Program.TypeBuild}]!");
			set => Set(nameof(Motd), value);
		}

		public static readonly LoginServerConfig Instance = new();

		private LoginServerConfig() : base("Battle-Server")
		{
		}
	}
}
