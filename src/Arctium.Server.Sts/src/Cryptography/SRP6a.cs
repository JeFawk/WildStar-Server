// Copyright (c) Arctium.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Arctium.Core;
using Arctium.Core.Misc;

namespace Arctium.Server.Sts.Cryptography
{
    sealed class SRP6a : IDisposable
    {
        public byte[] I { get; private set; }
        public byte[] S2 { get; private set; }
        public byte[] V { get; private set; }
        public byte[] B { get; private set; }
        public byte[] SessionKey { get; private set; }
        public byte[] ClientM { get; private set; }
        public byte[] ServerM { get; private set; }

        public byte[] S { get; }

        readonly BigInteger gBN;
        readonly BigInteger k;

        readonly byte[] N;
        readonly byte[] g;        

        SHA256 sha256;
        BigInteger A;
        BigInteger BN;
        BigInteger v;
        BigInteger b;
        BigInteger s;

        public SRP6a(string salt, string accountName, string passwordVerifier)
        {
            sha256 = new SHA256Managed();

            I = sha256.ComputeHash(Encoding.UTF8.GetBytes(accountName));

            N = new byte[]
            {
                0xE3, 0x06, 0xEB, 0xC0, 0x2F, 0x1D, 0xC6, 0x9F, 0x5B, 0x43, 0x76, 0x83, 0xFE, 0x38, 0x51, 0xFD,
                0x9A, 0xAA, 0x6E, 0x97, 0xF4, 0xCB, 0xD4, 0x2F, 0xC0, 0x6C, 0x72, 0x05, 0x3C, 0xBC, 0xED, 0x68,
                0xEC, 0x57, 0x0E, 0x66, 0x66, 0xF5, 0x29, 0xC5, 0x85, 0x18, 0xCF, 0x7B, 0x29, 0x9B, 0x55, 0x82,
                0x49, 0x5D, 0xB1, 0x69, 0xAD, 0xF4, 0x8E, 0xCE, 0xB6, 0xD6, 0x54, 0x61, 0xB4, 0xD7, 0xC7, 0x5D,
                0xD1, 0xDA, 0x89, 0x60, 0x1D, 0x5C, 0x49, 0x8E, 0xE4, 0x8B, 0xB9, 0x50, 0xE2, 0xD8, 0xD5, 0xE0,
                0xE0, 0xC6, 0x92, 0xD6, 0x13, 0x48, 0x3B, 0x38, 0xD3, 0x81, 0xEA, 0x96, 0x74, 0xDF, 0x74, 0xD6,
                0x76, 0x65, 0x25, 0x9C, 0x4C, 0x31, 0xA2, 0x9E, 0x0B, 0x3C, 0xFF, 0x75, 0x87, 0x61, 0x72, 0x60,
                0xE8, 0xC5, 0x8F, 0xFA, 0x0A, 0xF8, 0x33, 0x9C, 0xD6, 0x8D, 0xB3, 0xAD, 0xB9, 0x0A, 0xAF, 0xEE
            };

            S = salt.ToByteArray();

            g = new byte[] { 2, 0, 0, 0 };

            BN = N.ToBigInteger();
            gBN = new byte[] { 2 }.ToBigInteger();

            k = ReverseUInt32(sha256.ComputeHash(N.Combine(g))).ToBigInteger();
            v = passwordVerifier.ToByteArray().ToBigInteger();
        }

        public void CalculateX(string accountName, string password, bool calcB)
        {
            I = sha256.ComputeHash(Encoding.UTF8.GetBytes(accountName));

            var p = sha256.ComputeHash(Encoding.UTF8.GetBytes(accountName + ":" + password));
            var x = ReverseUInt32(sha256.ComputeHash(S.Combine(p)));

            CalculateV(new BigInteger(x.Combine(new byte[1])), calcB);
        }

        void CalculateV(BigInteger x, bool calcB)
        {
            v = BigInteger.ModPow(gBN, x, BN);
            V = v.ToByteArray();

            if (calcB)
                CalculateB();
        }

        public void CalculateB()
        {
            S2 = Helper.GenerateRandomKey(0x80);

            b = S2.ToBigInteger();
            B = GetBytes(((k * v + BigInteger.ModPow(gBN, b, BN)) % BN).ToByteArray(), 0x80);
        }

        public bool CalculateU(byte[] a)
        {
            A = a.ToBigInteger();

            // Don't continue if A == 0 (mod N)
            if (A.IsZero || (A % BN).IsZero)
                return false;

            var u = ReverseUInt32(sha256.ComputeHash(a.Combine(B)));

            CalculateS(new BigInteger(u.Combine(new byte[1])));

            return true;
        }

        void CalculateS(BigInteger u)
        {
            s = BigInteger.ModPow(((A * BigInteger.ModPow(v, u, BN)) % BN), b, BN);

            CalculateSessionKey();
        }

        public void CalculateSessionKey()
        {
            var sBytes = GetBytes(s.ToByteArray(), 0x80);
            var first0Position = Array.IndexOf(sBytes, (byte)0);
            var startIndex1 = sBytes.Length - 1;
            var length = 4;

            if (first0Position != -1 && first0Position < (sBytes.Length - 4))
                length = sBytes.Length - first0Position;

            var part1 = new byte[length >> 1];
            var part2 = new byte[length >> 1];

            for (int i = 0, j = startIndex1, k = startIndex1 - 1; i < part1.Length; i++, j -= 2, k -= 2)
            {
                part1[i] = sBytes[j];
                part2[i] = sBytes[k];
            }

            part1 = sha256.ComputeHash(part1);
            part2 = sha256.ComputeHash(part2);

            SessionKey = new byte[sBytes.Length / 2];

            for (var i = 0; i < part1.Length; i++)
            {
                SessionKey[i * 2] = part1[i];
                SessionKey[i * 2 + 1] = part2[i];
            }
        }

        public void CalculateClientM(byte[] a)
        {
            var NHash = sha256.ComputeHash(N);
            var gHash = sha256.ComputeHash(g);

            for (var i = 0; i < NHash.Length; i++)
                NHash[i] ^= gHash[i];

            // Concat all variables for M1 hash
            var hash = NHash.Combine(I, S, a, B, SessionKey);

            ClientM = sha256.ComputeHash(hash);
        }

        public void CalculateServerM(byte[] m1, byte[] a)
        {
            ServerM = sha256.ComputeHash(a.Combine(m1, SessionKey));
        }

        public byte[] GetBytes(byte[] data, int count)
        {
            if (data.Length <= count)
                return data;

            var bytes = new byte[count];

            Buffer.BlockCopy(data, 0, bytes, 0, count);

            return bytes;
        }

        byte[] ReverseUInt32(byte[] data)
        {
            var ret = new byte[data.Length];

            for (var i = 0; i < data.Length; i += 4)
                Buffer.BlockCopy(data, i, ret, ret.Length - (i + 4), 4);

            return ret;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    sha256.Dispose();
                }

                SessionKey = null;
                ServerM = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
