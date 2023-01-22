//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.GameSystem;
//Blizzless Project 2022 
using DiIiS_NA.GameServer.GSSystem.ObjectsSystem;
//Blizzless Project 2022 
using System;

namespace DiIiS_NA.GameServer.GSSystem.TickerSystem
{
	/// <summary>
	/// A stepped tick timer that can fire a completion action on timeout.
	/// </summary>
	public class TickTimer : IUpdateable
	{
		/// <summary>
		/// Exact tick value to timeout.
		/// </summary>
		public int TimeoutTick { get; set; } //public "set" for extending timeout

		/// <summary>
		/// The game timer belongs to.
		/// </summary>
		public Game Game { get; private set; }

		/// <summary>
		/// The completition action to be called on timeout.
		/// </summary>
		public Action<int> CompletionAction { get; private set; }

		/// <summary>
		/// Creates a new tick timer that can fire a completition action timeout.
		/// </summary>
		/// <param name="game">The game timer belongs to</param>
		/// <param name="timeoutTick">Exact tick value to timeout</param>
		/// <param name="completionCallback">The completition action to be called on timeout</param>
		public TickTimer(Game game, int timeoutTick, Action<int> completionCallback = null)
		{
			// Some code that was calculating movement ticks was rounding the tick difference to 0 for really small
			// movements sometimes and thus would cause this exception. Enforcing every timer created to not 
			// already be timed out doesn't seem necessary and having to worry about it just complicates things. /mdz
			//if (timeoutTick <= game.TickCounter)
			//	throw new ArgumentOutOfRangeException("timeoutTick", string.Format("timeoutTick value {0} can not be equal or less then timer's belonging game's current TickCounter value {1}.", timeoutTick, game.TickCounter));

			Game = game;
			TimeoutTick = timeoutTick;
			CompletionAction = completionCallback;
		}

		/// <summary>
		/// Returns true if the timer is timed-out.
		/// </summary>
		public bool TimedOut
		{
			get { return Game == null || !Game.Working || Game.TickCounter >= TimeoutTick; }
		}

		/// <summary>
		/// Returns true if timer is still running.
		/// </summary>
		public bool Running
		{
			get { return !TimedOut; }
		}

		/// <summary>
		/// Updates the timer.
		/// </summary>
		/// <param name="tickCounter">The current tick-counter.</param>
		public virtual void Update(int tickCounter)
		{
			if (TimeoutTick == -1) // means timer is already fired there.
				return;

			if (!TimedOut) // if we haven't timed-out yet, return.
				return;

			if (CompletionAction != null) // if a completition action exists.
				CompletionAction(tickCounter); //call it once the timer time-outs.

			Stop();
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		public void Stop()
		{
			TimeoutTick = -1;
		}

		/// <summary>
		/// Creates a new tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="ticks">Relative tick amount taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		/// <returns><see cref="SteppedTickTimer"/></returns>
		public static TickTimer WaitTicks(Game game, int ticks, Action<int> completionCallback = null)
		{
			return new RelativeTickTimer(game, ticks, completionCallback);
		}

		/// <summary>
		/// Creates a new seconds based tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="seconds">Seconds taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		/// <returns><see cref="SteppedTickTimer"/></returns>
		public static TickTimer WaitSeconds(Game game, float seconds, Action<int> completionCallback = null)
		{
			return new SecondsTickTimer(game, seconds, completionCallback);
		}

		/// <summary>
		/// Creates a new mili-seconds based tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="miliSeconds">MiliSeconds taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		/// <returns><see cref="SteppedTickTimer"/></returns>
		public static TickTimer WaitMiliSeconds(Game game, float miliSeconds, Action<int> completionCallback = null)
		{
			return new MiliSecondsTickTimer(game, miliSeconds, completionCallback);
		}
	}

	/// <summary>
	/// Relative tick timer.
	/// </summary>
	public class RelativeTickTimer : TickTimer
	{
		/// <summary>
		/// Creates a new relative tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="ticks">Relative tick amount taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		public RelativeTickTimer(Game game, int ticks, Action<int> completionCallback = null)
			: base(game, game.TickCounter + ticks, completionCallback)
		{ }
	}

	/// <summary>
	/// Seconds based tick timer.
	/// </summary>
	public class SecondsTickTimer : RelativeTickTimer
	{
		/// <summary>
		/// Creates a new seconds based tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="seconds">Seconds taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		/// <returns><see cref="SteppedTickTimer"/></returns>
		public SecondsTickTimer(Game game, float seconds, Action<int> completionCallback = null)
			: base(game, (int)(1000f / game.UpdateFrequency * game.TickRate * seconds), completionCallback)
		{ }
	}

	/// <summary>
	/// Mili-seconds based tick timer.
	/// </summary>
	public class MiliSecondsTickTimer : RelativeTickTimer
	{
		/// <summary>
		/// Creates a new mili-seconds based tick timer.
		/// </summary>
		/// <param name="game">The game timer belongs to.</param>
		/// <param name="miliSeconds">MiliSeconds taken to timeout.</param>
		/// <param name="completionCallback">The completition action to be called on timeout.</param>
		/// <returns><see cref="SteppedTickTimer"/></returns>
		public MiliSecondsTickTimer(Game game, float miliSeconds, Action<int> completionCallback = null)
			: base(game, (int)((1000f / game.UpdateFrequency * game.TickRate) / 1000f * miliSeconds), completionCallback)
		{ }
	}
}
