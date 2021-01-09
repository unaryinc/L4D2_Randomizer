/*
MIT License
Copyright (c) 2020 Unary Incorporated
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Security.Cryptography;

using Unary.L4D2_Randomizer.Abstract;

namespace Unary.L4D2_Randomizer.Systems
{
    public class Random : ISystem
    {
        public int Next(int min, int max)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min));
            if (min == max) return min;

            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[4];
                rng.GetBytes(data);

                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

                int diff = max - min;
                int mod = generatedValue % diff;
                int normalizedNumber = min + mod;

                return normalizedNumber;
            }
        }
        
        public int Next()
        {
            return Next(int.MinValue, int.MaxValue);
        }

        public float Next(float min, float max)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min));
            if (min == max) return min;

            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[4];
                rng.GetBytes(data);

                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

                float diff = max - min;
                float mod = generatedValue % diff;
                float normalizedNumber = min + mod;

                return normalizedNumber;
            }
        }
    }
}
