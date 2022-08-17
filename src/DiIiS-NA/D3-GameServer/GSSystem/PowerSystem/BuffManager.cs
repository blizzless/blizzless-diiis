//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ActorSystem;
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem
{
	public class BuffManager
	{
		private Dictionary<Actor, List<Buff>> _buffs = new Dictionary<Actor, List<Buff>>();

		public void Update()
		{
			// make copy of keys as the dictionary will be modified during update/cleaning
			Actor[] keys = _buffs.Keys.ToArray();

			// update buffs and mark finished ones as removed
			foreach (Actor target in keys)
				_RemoveBuffsIf(target, buff => buff.Update());

			// clean up removed buffs
			foreach (Actor target in keys)
			{
				if (target == null) continue;
				_buffs[target].RemoveAll(buff => buff == null);
				if (_buffs[target].Count == 0)
					_buffs.Remove(target);
			}
		}

		public bool AddBuff(Actor user, Actor target, Buff buff)
		{
			if (user.World == null || target.World == null) return false;
			if (target.Dead) return false;

			buff.User = user;
			buff.Target = target;
			buff.World = target.World;

			// try to load in power sno from class attribute first, then try parent class (if there is one)
			Type buffType = buff.GetType();
			int powerSNO = ImplementsPowerSNO.GetPowerSNOForClass(buffType);
			if (powerSNO != -1)
			{
				buff.PowerSNO = powerSNO;
			}
			else if (buffType.IsNested)
			{
				powerSNO = ImplementsPowerSNO.GetPowerSNOForClass(buffType.DeclaringType);
				if (powerSNO != -1)
					buff.PowerSNO = powerSNO;
			}

			buff.Init();

			return _AddBuff(buff);
		}
		public void RemoveStackFromBuff(Actor target, PowerBuff buff)
		{
			if (target.World == null) return;
			if (target.Dead) return;
			if (!buff.IsCountingStacks) return;

			if (buff.StackCount <= 1)
			{
				_RemoveBuffsIf(target, Buff => Buff == buff);
				return;
			}
			buff.DeStack(buff);
		}

		public void CopyBuff(Actor user, Actor target, Buff buff, int Stacks)
		{
			if (user.World == null || target.World == null) return;

			buff.User = user;
			buff.Target = target;
			buff.World = target.World;
			buff.Removed = false;

			for (int i = 0; i < Stacks; i++)
				_AddBuff(buff);
		}

		public void RemoveBuffs(Actor target, Type buffClass)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => buff.GetType() == buffClass);
		}

		public void RemoveBuffs(Actor target, int powerSNO)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => buff.PowerSNO == powerSNO);
		}

		public void RemoveBuff(Actor target, Buff buff)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, Buff => Buff == buff);
		}

		public void RemoveAllBuffs(Actor target, bool removeCooldowns = true)
		{
			if (!_buffs.ContainsKey(target)) return;

			_RemoveBuffsIf(target, buff => (removeCooldowns ? true : buff.PowerSNO != 30176));
		}

		public T GetFirstBuff<T>(Actor target) where T : Buff
		{
			if (!_buffs.ContainsKey(target)) return null;

			Buff buff = _buffs[target].FirstOrDefault(b => b != null && b.GetType() == typeof(T));
			if (buff != null)
				return (T)buff;
			else
				return null;
		}

		public List<T> GetBuffs<T>(Actor target) where T : Buff
		{
			if (!_buffs.ContainsKey(target)) return new List<T>();

			List<Buff> buffs = _buffs[target].Where(b => b != null && b.GetType() == typeof(T)).ToList();
			if (buffs != null)
				return buffs.Cast<T>().ToList();
			else
				return new List<T>();
		}

		public Dictionary<Buff, int> GetAllBuffs(Actor target)
		{
			var buffs = new Dictionary<Buff, int>();
			if (!_buffs.ContainsKey(target)) return buffs;
			foreach (var buff in _buffs[target].Where(b => b != null))
			{
				buffs.Add(buff, (buff is PowerBuff ? (buff as PowerBuff).StackCount : 1));
			}
			return buffs;
		}

		public bool HasBuff<T>(Actor target) where T : Buff
		{
			return GetFirstBuff<T>(target) != null;
		}

		public void SendTargetPayload(Actor target, Payloads.Payload payload)
		{
			if (_buffs.ContainsKey(target))
			{
				List<Buff> buffs = _buffs[target];
				int buffCount = buffs.Count;
				for (int i = 0; i < buffCount; ++i)
				{
					if (buffs[i] != null)
						try
						{
							buffs[i].OnPayload(payload);
						}
						catch { }
				}
			}
		}

		private bool _AddBuff(Buff buff)
		{
			// look up or create a buff list for the target, then add/stack the buff according to its class type.

			// the logic is a bit more complex that it seems necessary because we ensure the buff appears in the
			// active buff list before calling Apply(), if Apply() fails we undo adding it. This allows buffs to
			// recursively add/stack more of their own buff type without worrying about overwriting existing buffs.
			if (_buffs.ContainsKey(buff.Target))
			{
				Type buffType = buff.GetType();
				Buff existingBuff = _buffs[buff.Target].FirstOrDefault(b => b != null && b.GetType() == buffType);
				if (existingBuff != null)
				{
					if (existingBuff.Stack(buff))
						return true;
					// buff is non-stacking, just add normally
				}

				_buffs[buff.Target].Add(buff);
				if (buff.Apply())
				{
					return true;
				}
				else
				{
					_buffs[buff.Target].Remove(buff);
					return false;
				}
			}
			else
			{
				var keyBuffs = new List<Buff>();
				keyBuffs.Add(buff);
				_buffs[buff.Target] = keyBuffs;
				if (buff.Apply())
				{
					return true;
				}
				else
				{
					_buffs.Remove(buff.Target);
					return false;
				}
			}
		}

		private void _RemoveBuffsIf(Actor target, Func<Buff, bool> pred)
		{
			if (target == null) return;
			List<Buff> buffs = _buffs[target];
			int buffCount = buffs.Count;
			for (int i = 0; i < buffCount; ++i)
			{
				if (buffs[i] != null)
				{
					if (pred(buffs[i]))
					{
						if (buffs[i] != null)
						{
							buffs[i].Remove();
							buffs[i] = null;
						}
					}
				}
			}
		}
	}
}
