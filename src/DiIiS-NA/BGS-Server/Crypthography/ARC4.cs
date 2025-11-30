namespace DiIiS_NA.LoginServer.Crypthography
{
	public class ARC4
	{
		private readonly byte[] _state;
		private byte x, y;

		public ARC4(byte[] key)
		{
			_state = new byte[256];
			x = y = 0;
			KeySetup(key);
		}

		public int Process(byte[] buffer, int start, int count)
		{
			return InternalTransformBlock(buffer, start, count, buffer, start);
		}

		private void KeySetup(byte[] key)
		{
			byte index1 = 0;
			byte index2 = 0;

			for (int counter = 0; counter < 256; counter++)
			{
				_state[counter] = (byte)counter;
			}
			x = 0;
			y = 0;
			for (int counter = 0; counter < 256; counter++)
			{
				index2 = (byte)(key[index1] + _state[counter] + index2);
				// swap byte
				(_state[counter], _state[index2]) = (_state[index2], _state[counter]);
                index1 = (byte)((index1 + 1) % key.Length);
			}
		}

		private int InternalTransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			for (int counter = 0; counter < inputCount; counter++)
			{
				x = (byte)(x + 1);
				y = (byte)(_state[x] + y);
				// swap byte
				(_state[x], _state[y]) = (_state[y], _state[x]);

                byte xorIndex = (byte)(_state[x] + _state[y]);
				outputBuffer[outputOffset + counter] = (byte)(inputBuffer[inputOffset + counter] ^ _state[xorIndex]);
			}
			return inputCount;
		}
	}
}
