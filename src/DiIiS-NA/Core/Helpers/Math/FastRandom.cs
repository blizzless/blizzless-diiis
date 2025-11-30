using System;
using System.Collections.Generic;
using System.Text;
using DiIiS_NA.Core.Logging;

namespace DiIiS_NA.Core.Helpers.Math
{
	public class FastRandom
	{
		#region Static Fields

		private static readonly Logger Logger = LogManager.CreateLogger(nameof(FastRandom));
		
		private static readonly FastRandom _seedRng = new(Environment.TickCount);

		public static readonly FastRandom Instance = new FastRandom();

		#endregion

		#region Instance Fields

		const double REAL_UNIT_INT = 1.0 / ((double)int.MaxValue + 1.0);
		const double REAL_UNIT_UINT = 1.0 / ((double)uint.MaxValue + 1.0);
		const uint Y = 842502087, Z = 3579807591, W = 273326509;

		uint _x, _y, _z, _w;

		#endregion

		#region Constructors

		public FastRandom()
		{
			Reinitialise(_seedRng.NextInt());
		}

		public FastRandom(int seed)
		{
			Reinitialise(seed);
		}

		#endregion

		#region Public Methods [Reinitialisation]

		public void Reinitialise(int seed)
		{
			_x = (uint)seed;
			_y = Y;
			_z = Z;
			_w = W;

			_bitBuffer = 0;
			_bitMask = 1;
		}

		#endregion

		#region Public Methods [System.Random functionally equivalent methods]

		public int Next()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

			uint rtn = _w & 0x7FFFFFFF;
			if (rtn == 0x7FFFFFFF)
			{
				return Next();
			}
			return (int)rtn;
		}

		public int Next(int upperBound)
		{
			if (upperBound < 0)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
			}

			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			return (int)((REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * upperBound);
		}

		public int Next(int lowerBound, int upperBound)
		{
			if (lowerBound > upperBound)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
			}

			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			int range = upperBound - lowerBound;
			if (range < 0)
			{   
				return lowerBound + (int)((REAL_UNIT_UINT * (double)(_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * (double)((long)upperBound - (long)lowerBound));
			}

			return lowerBound + (int)((REAL_UNIT_INT * (double)(int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))))) * (double)range);
		}

		public double NextDouble()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			return REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))));
		}

		public double NextDouble(double lowerBound, double upperBound)
		{
			return NextDouble() * (upperBound - lowerBound) + lowerBound;
		}

		public void NextBytes(byte[] buffer)
		{
			uint x = this._x, y = this._y, z = this._z, w = this._w;
			int i = 0;
			uint t;
			for (int bound = buffer.Length - 3; i < bound;)
			{
				t = x ^ (x << 11);
				x = y; y = z; z = w;
				w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

				buffer[i++] = (byte)w;
				buffer[i++] = (byte)(w >> 8);
				buffer[i++] = (byte)(w >> 16);
				buffer[i++] = (byte)(w >> 24);
			}

			if (i < buffer.Length)
			{
				t = x ^ (x << 11);
				x = y; y = z; z = w;
				w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

				buffer[i++] = (byte)w;
				if (i < buffer.Length)
				{
					buffer[i++] = (byte)(w >> 8);
					if (i < buffer.Length)
					{
						buffer[i++] = (byte)(w >> 16);
						if (i < buffer.Length)
						{
							buffer[i] = (byte)(w >> 24);
						}
					}
				}
			}
			this._x = x; this._y = y; this._z = z; this._w = w;
		}

		#endregion

		#region Public Methods [Methods not present on System.Random]

		public uint NextUInt()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			return _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
		}

		public int NextInt()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			return (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))));
		}

		uint _bitBuffer;
		uint _bitMask;

		public bool NextBool()
		{
			if (0 == _bitMask)
			{
				uint t = _x ^ (_x << 11);
				_x = _y; _y = _z; _z = _w;
				_bitBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

				_bitMask = 0x80000000;
				return (_bitBuffer & _bitMask) == 0;
			}

			return (_bitBuffer & (_bitMask >>= 1)) == 0;
		}

		uint _byteBuffer;
		byte _byteBufferState;

		public byte NextByte()
		{
			if (0 == _byteBufferState)
			{
				uint t = _x ^ (_x << 11);
				_x = _y; _y = _z; _z = _w;
				_byteBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
				_byteBufferState = 0x4;
				return (byte)_byteBuffer; 
			}
			_byteBufferState >>= 1;
			return (byte)(_byteBuffer >>= 1);
		}

		#endregion

		/// <summary>
		/// Chance returns true if a random number between 0 and 100 is lesser than the specified value.
		/// </summary>
		/// <param name="successPercentage"></param>
		/// <returns></returns>
		public bool Chance(float successPercentage) => Next(100) < successPercentage;
	}

	public static class NumberExtensions
	{
        public static float ToFloat(this double value, int decimals = 4) => (float)System.Math.Round(value, decimals);
        public static float ToFloat(this float value, int decimals = 4) => (float)System.Math.Round(value, decimals);
		public static double ToDouble(this float value, int decimals = 4) => (double)System.Math.Round(value, decimals);
		public static double ToDouble(this double value, int decimals = 4) => (double)System.Math.Round(value, decimals);
    }
}
