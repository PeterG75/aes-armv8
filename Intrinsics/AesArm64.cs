using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm.Arm64;

namespace Intrinsics
{
	public class AesArm64
	{
		private const int BlockSize = 16;
		private const int Rounds = 10;

		private readonly byte[] enc;
		private readonly byte[] dec;

		public AesArm64()
		{
			enc = new byte[(Rounds + 1) * BlockSize]
			{
				0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c,
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

			dec = new byte[(Rounds + 1) * BlockSize]
			{
				0xd0, 0x14, 0xf9, 0xa8, 0xc9, 0xee, 0x25, 0x89, 0xe1, 0x3f, 0x0c, 0xc8, 0xb6, 0x63, 0x0c, 0xa6,
				0x0c, 0x7b, 0x5a, 0x63, 0x13, 0x19, 0xea, 0xfe, 0xb0, 0x39, 0x88, 0x90, 0x66, 0x4c, 0xfb, 0xb4,
				0xdf, 0x7d, 0x92, 0x5a, 0x1f, 0x62, 0xb0, 0x9d, 0xa3, 0x20, 0x62, 0x6e, 0xd6, 0x75, 0x73, 0x24,
				0x12, 0xc0, 0x76, 0x47, 0xc0, 0x1f, 0x22, 0xc7, 0xbc, 0x42, 0xd2, 0xf3, 0x75, 0x55, 0x11, 0x4a,
				0x6e, 0xfc, 0xd8, 0x76, 0xd2, 0xdf, 0x54, 0x80, 0x7c, 0x5d, 0xf0, 0x34, 0xc9, 0x17, 0xc3, 0xb9,
				0x6e, 0xa3, 0x0a, 0xfc, 0xbc, 0x23, 0x8c, 0xf6, 0xae, 0x82, 0xa4, 0xb4, 0xb5, 0x4a, 0x33, 0x8d,
				0x90, 0x88, 0x44, 0x13, 0xd2, 0x80, 0x86, 0x0a, 0x12, 0xa1, 0x28, 0x42, 0x1b, 0xc8, 0x97, 0x39,
				0x7c, 0x1f, 0x13, 0xf7, 0x42, 0x08, 0xc2, 0x19, 0xc0, 0x21, 0xae, 0x48, 0x09, 0x69, 0xbf, 0x7b,
				0xcc, 0x75, 0x05, 0xeb, 0x3e, 0x17, 0xd1, 0xee, 0x82, 0x29, 0x6c, 0x51, 0xc9, 0x48, 0x11, 0x33,
				0x2b, 0x37, 0x08, 0xa7, 0xf2, 0x62, 0xd4, 0x05, 0xbc, 0x3e, 0xbd, 0xbf, 0x4b, 0x61, 0x7d, 0x62,
				0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c
			};
		}

		public void Encrypt(byte[] input, byte[] output)
		{
			int position = 0;
			int left = input.Length;

			var key0 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[0 * BlockSize]);
			var key1 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[1 * BlockSize]);
			var key2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[2 * BlockSize]);
			var key3 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[3 * BlockSize]);
			var key4 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[4 * BlockSize]);
			var key5 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[5 * BlockSize]);
			var key6 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[6 * BlockSize]);
			var key7 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[7 * BlockSize]);
			var key8 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[8 * BlockSize]);
			var key9 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[9 * BlockSize]);
			var key10 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[10 * BlockSize]);

			while (left >= BlockSize)
			{
				var block = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position]);

				block = Aes.Encrypt(block, key0);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key1);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key2);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key3);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key4);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key5);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key6);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key7);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key8);
				block = Aes.MixColumns(block);

				block = Aes.Encrypt(block, key9);
				block = Simd.Xor(block, key10);

				Unsafe.WriteUnaligned(ref output[position], block);

				position += BlockSize;
				left -= BlockSize;
			}
		}

		public void EncryptPipelined(byte[] input, byte[] output)
		{
			int position = 0;
			int left = input.Length;

			var key0 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[0 * BlockSize]);
			var key1 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[1 * BlockSize]);
			var key2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[2 * BlockSize]);
			var key3 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[3 * BlockSize]);
			var key4 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[4 * BlockSize]);
			var key5 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[5 * BlockSize]);
			var key6 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[6 * BlockSize]);
			var key7 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[7 * BlockSize]);
			var key8 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[8 * BlockSize]);
			var key9 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[9 * BlockSize]);
			var key10 = Unsafe.ReadUnaligned<Vector128<byte>>(ref enc[10 * BlockSize]);

			while (left >= 4 * BlockSize)
			{
				var block0 = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position + 0 * BlockSize]);
				var block1 = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position + 1 * BlockSize]);
				var block2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position + 2 * BlockSize]);
				var block3 = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position + 3 * BlockSize]);

				// Round 1
				block0 = Aes.Encrypt(block0, key0);
				block1 = Aes.Encrypt(block1, key0);
				block2 = Aes.Encrypt(block2, key0);
				block3 = Aes.Encrypt(block3, key0);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 2
				block0 = Aes.Encrypt(block0, key1);
				block1 = Aes.Encrypt(block1, key1);
				block2 = Aes.Encrypt(block2, key1);
				block3 = Aes.Encrypt(block3, key1);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 3
				block0 = Aes.Encrypt(block0, key2);
				block1 = Aes.Encrypt(block1, key2);
				block2 = Aes.Encrypt(block2, key2);
				block3 = Aes.Encrypt(block3, key2);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 4
				block0 = Aes.Encrypt(block0, key3);
				block1 = Aes.Encrypt(block1, key3);
				block2 = Aes.Encrypt(block2, key3);
				block3 = Aes.Encrypt(block3, key3);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 5
				block0 = Aes.Encrypt(block0, key4);
				block1 = Aes.Encrypt(block1, key4);
				block2 = Aes.Encrypt(block2, key4);
				block3 = Aes.Encrypt(block3, key4);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 6
				block0 = Aes.Encrypt(block0, key5);
				block1 = Aes.Encrypt(block1, key5);
				block2 = Aes.Encrypt(block2, key5);
				block3 = Aes.Encrypt(block3, key5);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 7
				block0 = Aes.Encrypt(block0, key6);
				block1 = Aes.Encrypt(block1, key6);
				block2 = Aes.Encrypt(block2, key6);
				block3 = Aes.Encrypt(block3, key6);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 8
				block0 = Aes.Encrypt(block0, key7);
				block1 = Aes.Encrypt(block1, key7);
				block2 = Aes.Encrypt(block2, key7);
				block3 = Aes.Encrypt(block3, key7);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 9
				block0 = Aes.Encrypt(block0, key8);
				block1 = Aes.Encrypt(block1, key8);
				block2 = Aes.Encrypt(block2, key8);
				block3 = Aes.Encrypt(block3, key8);

				block0 = Aes.MixColumns(block0);
				block1 = Aes.MixColumns(block1);
				block2 = Aes.MixColumns(block2);
				block3 = Aes.MixColumns(block3);

				// Round 10
				block0 = Aes.Encrypt(block0, key9);
				block1 = Aes.Encrypt(block1, key9);
				block2 = Aes.Encrypt(block2, key9);
				block3 = Aes.Encrypt(block3, key9);

				block0 = Simd.Xor(block0, key10);
				block1 = Simd.Xor(block1, key10);
				block2 = Simd.Xor(block2, key10);
				block3 = Simd.Xor(block3, key10);

				Unsafe.WriteUnaligned(ref output[position + 0 * BlockSize], block0);
				Unsafe.WriteUnaligned(ref output[position + 1 * BlockSize], block1);
				Unsafe.WriteUnaligned(ref output[position + 2 * BlockSize], block2);
				Unsafe.WriteUnaligned(ref output[position + 3 * BlockSize], block3);

				position += 4 * BlockSize;
				left -= 4 * BlockSize;
			}
		}

		public void Decrypt(byte[] input, byte[] output)
		{
			int position = 0;
			int left = input.Length;

			var key0 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[0 * BlockSize]);
			var key1 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[1 * BlockSize]);
			var key2 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[2 * BlockSize]);
			var key3 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[3 * BlockSize]);
			var key4 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[4 * BlockSize]);
			var key5 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[5 * BlockSize]);
			var key6 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[6 * BlockSize]);
			var key7 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[7 * BlockSize]);
			var key8 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[8 * BlockSize]);
			var key9 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[9 * BlockSize]);
			var key10 = Unsafe.ReadUnaligned<Vector128<byte>>(ref dec[10 * BlockSize]);

			while (left >= BlockSize)
			{
				var block = Unsafe.ReadUnaligned<Vector128<byte>>(ref input[position]);

				block = Aes.Decrypt(block, key0);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key1);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key2);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key3);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key4);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key5);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key6);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key7);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key8);
				block = Aes.InverseMixColumns(block);

				block = Aes.Decrypt(block, key9);
				block = Simd.Xor(block, key10);

				Unsafe.WriteUnaligned(ref output[position], block);

				position += BlockSize;
				left -= BlockSize;
			}
		}
	}
}
