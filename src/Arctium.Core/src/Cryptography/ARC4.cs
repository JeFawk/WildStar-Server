// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Core.Cryptography
{
    public sealed class ARC4
    {
        byte[] s;
        byte tmp, tmp2;

        public ARC4()
        {
            s = new byte[0x100];
            tmp = 0;
            tmp2 = 0;
        }

        public void PrepareKey(byte[] key)
        {
            for (int i = 0; i < 0x100; i++)
                s[i] = (byte)i;

            for (int i = 0, j = 0; i < 0x100; i++)
            {
                j = (byte)((j + s[i] + key[i % key.Length]) % 0x100);

                var tempS = s[i];

                s[i] = s[j];
                s[j] = tempS;
            }
        }

        public void ProcessBuffer(byte[] data, int length, int index = 0)
        {
            for (int i = 0; i < length; i++)
            {
                tmp = (byte)((tmp + 1) % 0x100);
                tmp2 = (byte)((tmp2 + s[tmp]) % 0x100);

                var sTemp = s[tmp];

                s[tmp] = s[tmp2];
                s[tmp2] = sTemp;

                data[i + index] = (byte)(s[(s[tmp] + s[tmp2]) % 0x100] ^ data[i + index]);
            }
        }

        public void ProcessBuffer(byte[] data) => ProcessBuffer(data, data.Length);
    }
}
