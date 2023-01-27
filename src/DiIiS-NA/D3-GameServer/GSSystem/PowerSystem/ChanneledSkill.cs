using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public abstract class ChanneledSkill : PowerScript
	{
		public bool IsChannelOpen = false;
		public float EffectsPerSecond = 1.0f;
		public bool WaitForSpawn = false;
		public new float WaitSeconds = 1.0f;

		public virtual void OnChannelOpen() { }
		public virtual void OnChannelClose() { }
		public virtual void OnChannelUpdated() { }
		public abstract IEnumerable<TickTimer> Main();

		private TickTimer _effectTimeout = null;
		private TickTimer _waitTimeout = null;

		public sealed override IEnumerable<TickTimer> Run()
		{
			// process channeled skill events
			if (IsChannelOpen)
			{
				if (WaitForSpawn)
				{
					if (_waitTimeout == null || _waitTimeout.TimedOut)
					{
						_waitTimeout = WaitSeconds(WaitSeconds);
						OnChannelUpdated();
					}
				}
				else
					OnChannelUpdated();
			}
			else  // first call to this skill's Run(), set channel as open
			{
				OnChannelOpen();
				IsChannelOpen = true;
			}

			// run main script if ready
			if (_effectTimeout == null || _effectTimeout.TimedOut)
			{
				_effectTimeout = WaitSeconds(EffectsPerSecond);
				foreach (TickTimer timeout in Main())
					yield return timeout;
			}
		}

		public void CloseChannel()
		{
			OnChannelClose();
			IsChannelOpen = false;
		}
	}
}
