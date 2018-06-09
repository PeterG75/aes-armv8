using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm.Arm64;

namespace Intrinsics
{
	public class AesArm64
	{
		private const int BlockSize = 16;
		private const int Rounds = 10;

		private readonly byte[] key;
		private readonly byte[] subkeys;

		public AesArm64()
		{
			key = new byte[BlockSize]
			{
				0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x9, 0xcf, 0x4f, 0x3c
			};

			subkeys = new byte[Rounds * BlockSize]
			{
				0xa0, 0xfa, 0xfe, 0x17, 0x88, 0x54, 0x2c, 0xb1, 0x23, 0xa3, 0x39, 0x39, 0x2a, 0x6c, 0x76, 0x05,
				0xf2, 0xc2, 0x95, 0xf2, 0x7a, 0x96, 0xb9, 0x43, 0x59, 0x35, 0x80, 0x7a, 0x73, 0x59, 0xf6, 0x7f,
				0x3d, 0x80, 0x47, 0x7d, 0x47, 0x16, 0xfe, 0x3e, 0x1e, 0x23, 0x7e, 0x44, 0x6d, 0x7a, 0x88, 0x3b,
				0xef, 0x44, 0xa5, 0x41, 0xa8, 0x52, 0x5b, 0x7f, 0xb6, 0x71, 0x25, 0x3b, 0xdb, 0x0b, 0xad, 0x00,
				0xd4, 0xd1, 0xc6, 0xf8, 0x7c, 0x83, 0x9d, 0x87, 0xca, 0xf2, 0xb8, 0xbc, 0x11, 0xf9, 0x15, 0xbc,
				0x6d, 0x88, 0xa3, 0x7a, 0x11, 0x0b, 0x3e, 0xfd, 0xdb, 0xf9, 0x86, 0x41, 0xca, 0x00, 0x93, 0xfd,
				0x4e, 0x54, 0xf7, 0x0e, 0x5f, 0x5f, 0xc9, 0xf3, 0x84, 0xa6, 0x4f, 0xb2, 0x4e, 0xa6, 0xdc, 0x4f,
				0xea, 0xd2, 0x73, 0x21, 0xb5, 0x8d, 0xba, 0xd2, 0x31, 0x2b, 0xf5, 0x60, 0x7f, 0x8d, 0x29, 0x2f,
				0xac, 0x77, 0x66, 0xf3, 0x19, 0xfa, 0xdc, 0x21, 0x28, 0xd1, 0x29, 0x41, 0x57, 0x5c, 0x00, 0x6e,
				0xd0, 0x14, 0xf9, 0xa8, 0xc9, 0xee, 0x25, 0x89, 0xe1, 0x3f, 0x0c, 0xc8, 0xb6, 0x63, 0x0c, 0xa6
			};
		}

		public void Encrypt(byte[] input, byte[] output)
		{
			int position = 0;
			int left = input.Length;

			var key = Unsafe.ReadUnaligned<Vector128<byte>>(ref this.key[0]);
			var subkey0 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[0 * BlockSize]);
			var subkey1 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[1 * BlockSize]);
			var subkey2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[2 * BlockSize]);
			var subkey3 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[3 * BlockSize]);
			var subkey4 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[4 * BlockSize]);
			var subkey5 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[5 * BlockSize]);
			var subkey6 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[6 * BlockSize]);
			var subkey7 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[7 * BlockSize]);
			var subkey8 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[8 * BlockSize]);
			var subkey9 = Unsafe.ReadUnaligned<Vector128<byte>>(ref subkeys[9 * BlockSize]);

			while (left >= BlockSize)
			{
				var block = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position]);

				block = Aes.Encrypt(block, key);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey0);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey1);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey2);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey3);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey4);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey5);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey6);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey7);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, subkey8);
				block = Simd.Xor(block, subkey9);

				Unsafe.WriteUnaligned(ref output[position], block);

				position += BlockSize;
				left -= BlockSize;
			}
		}
	}
}
