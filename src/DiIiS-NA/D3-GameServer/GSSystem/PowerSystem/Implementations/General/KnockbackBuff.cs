using DiIiS_NA.GameServer.Core.Types.Math;
using DiIiS_NA.GameServer.Core.Types.TagMap;
using DiIiS_NA.GameServer.GSSystem.TickerSystem;
using DiIiS_NA.GameServer.MessageSystem.Message.Definitions.ACD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.GameServer.GSSystem.PowerSystem.Implementations
{
	[ImplementsPowerSNO(70432)]  // Knockback.pow
	public class KnockbackBuff : Buff
	{

		public TickTimer ArrivalTime { get { if (_mover != null) return _mover.ArrivalTime; else return null; } }

		private float _magnitude;
		private float _height;
		private float _gravity;
		private ActorMover _mover;

		public KnockbackBuff(float magnitude, float arcHeight = 3.0f, float arcGravity = -0.03f)
		{
			_magnitude = magnitude;
			_height = arcHeight;
			_gravity = arcGravity;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			Vector3D destination = PowerMath.TranslateDirection2D(User.Position, Target.Position,
																   _magnitude < 0f ? User.Position : Target.Position,
																   (float)Math.Sqrt(Math.Abs(_magnitude)));

			if (!World.CheckLocationForFlag(destination, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				return false;

			_mover = new ActorMover(Target);
			_mover.MoveArc(destination, _height, _gravity, new ACDTranslateArcMessage
			{
				Field3 = 0x2006, // wtf?
				FlyingAnimationTagID = AnimationSetKeys.KnockBack.ID,
				LandingAnimationTagID = AnimationSetKeys.KnockBackLand.ID,
				PowerSNO = PowerSNO
			});

			return true;
		}

		public override bool Update()
		{
			if (_mover == null) return false;
			return _mover.Update();
		}

		public override bool Stack(Buff buff)
		{
			// not sure how knockbacks would be combined, so just swallow all knockback stacks for now
			// updated stacked buff with mover so arrival time can be read for would-be-stacked buff.
			((KnockbackBuff)buff)._mover = _mover;
			return true;
		}
	}

	public class DirectedKnockbackBuff : Buff
	{
		public TickTimer ArrivalTime { get { return _mover.ArrivalTime; } }

		private float _magnitude;
		private float _height;
		private float _gravity;
		private ActorMover _mover;
		private Vector3D _source;

		public DirectedKnockbackBuff(Vector3D source, float magnitude, float arcHeight = 3.0f, float arcGravity = -0.03f)
		{
			_source = source;
			_magnitude = magnitude;
			_height = arcHeight;
			_gravity = arcGravity;
		}

		public override bool Apply()
		{
			if (!base.Apply())
				return false;

			Vector3D destination = PowerMath.TranslateDirection2D(_source, Target.Position,
																   _magnitude < 0f ? _source : Target.Position,
																   (float)Math.Sqrt(Math.Abs(_magnitude)));

			if (!World.CheckLocationForFlag(destination, DiIiS_NA.Core.MPQ.FileFormats.Scene.NavCellFlags.AllowWalk))
				return false;

			_mover = new ActorMover(Target);
			_mover.MoveArc(destination, _height, _gravity, new ACDTranslateArcMessage
			{
				Field3 = 0x2006, // wtf?
				FlyingAnimationTagID = AnimationSetKeys.KnockBack.ID,
				LandingAnimationTagID = AnimationSetKeys.KnockBackLand.ID,
				PowerSNO = PowerSNO
			});

			return true;
		}

		public override bool Update()
		{
			if (_mover == null) return false;
			return _mover.Update();
		}

		public override bool Stack(Buff buff)
		{
			// not sure how knockbacks would be combined, so just swallow all knockback stacks for now
			// updated stacked buff with mover so arrival time can be read for would-be-stacked buff.
			((DirectedKnockbackBuff)buff)._mover = _mover;
			return true;
		}
	}
}
