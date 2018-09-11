// Copyright (c) Arctium.

namespace Arctium.Core.Cryptography
{
    public class Adler32
    {
        public static uint Calculate(byte[] data, uint s1 = 1, uint s2 = 0)
        {
            for (var i = 0; i < data.Length; i++)
            {
                s1 = (s1 + data[i]) % 0xFFF1;
                s2 = (s2 + s1) % 0xFFF1;
            }

            return (s2 << 16) + s1;
        }
    }

}
