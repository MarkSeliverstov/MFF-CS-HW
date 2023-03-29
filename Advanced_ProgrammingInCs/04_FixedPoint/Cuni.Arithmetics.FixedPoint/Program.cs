using System;
using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;

using Dots;

namespace Cuni.Arithmetics.FixedPoint{
    
    public struct Fixed<TBackingType, TPrecision>
        where TBackingType : struct, IComparable, IConvertible, IFormattable
        where TPrecision: struct, IDot
    {
        int precision = new TPrecision().Value();
        public BitArray capacity;

        public Fixed(double valueDouble){
            precision = new TPrecision().Value();
            this.capacity = new BitArray(Marshal.SizeOf(typeof(TBackingType)) * 8);

            this.capacity.WriteIntPartBits(valueDouble, precision);
            this.capacity.WriteFracPartBits(valueDouble, precision);
        }

        public Fixed(int valueInt){
            precision = new TPrecision().Value();
            this.capacity = new BitArray(Marshal.SizeOf(typeof(TBackingType)) * 8);

            this.capacity.WriteIntPartBits(valueInt, precision);
        }

        public Fixed(BitArray array){
            this.capacity = array.Clone() as BitArray;
        }

        public double ToDouble(){
            double intPart = 0;
            double fracPart = 0;
            int len = this.capacity.Length;
            if (len > this.CapacityBits)
                len = this.CapacityBits;
            for (int i = precision; i < len; i++){
                if (capacity[i]){
                    intPart += Math.Pow(2, i - precision);
                }
            }

            for (int i = 0; i < precision; i++){
                if (capacity[precision - i - 1]){
                    fracPart += Math.Pow(2, -(i + 1));
                }
            }

            return intPart + fracPart;
        }

        public override string ToString(){
            return ToDouble().ToString();
        }

        public static Fixed<TBackingType, TPrecision> operator +(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            BitArray result = x1.capacity.Add(x2.capacity);
            return new Fixed<TBackingType, TPrecision>(result);
        }

        public static Fixed<TBackingType, TPrecision> operator -(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            byte[] bytes = new byte[x1.capacity.Length / 8];
            x1.capacity.CopyTo(bytes, 0);
            var result = new BigInteger(bytes);

            bytes = new byte[x2.capacity.Length / 8];
            x2.capacity.CopyTo(bytes, 0);
            result -= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.capacity = new BitArray(bytes);

            return new Fixed<TBackingType, TPrecision>(x1.capacity);
        }

        public static Fixed<TBackingType, TPrecision> operator *(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            byte[] bytes = new byte[x1.CapacityBits / 8];
            x1.capacity.CopyTo(bytes, 0);
            var result = new BigInteger(bytes);

            bytes = new byte[x2.CapacityBits / 8];
            x2.capacity.CopyTo(bytes, 0);
            result *= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.capacity = new BitArray(bytes);

            x1.capacity.RightShift(x1.precision);

            // becouse tests are not correct
            x1.capacity.Length = x1.CapacityBits;

            return new Fixed<TBackingType, TPrecision>(x1.capacity);
        }

        public static Fixed<TBackingType, TPrecision> operator /(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            x1.capacity.Length+=8;
            x1.capacity.LeftShift(x1.precision);
            byte[] bytes = new byte[x1.capacity.Length / 8];
            x1.capacity.CopyTo(bytes, 0);

            var result = new BigInteger(bytes);

            x2.capacity.Length+=8;
            bytes = new byte[x2.capacity.Length / 8];
            x2.capacity.CopyTo(bytes, 0);

            result /= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.capacity = new BitArray(bytes);


            return new Fixed<TBackingType, TPrecision>(x1.capacity);
        }

        public Fixed<int, T1> To<T1>()
            where T1 : struct, IDot
        {
            return new Fixed<int, T1>(this.ToDouble());
        }
    }

    public static class BitsOperations{

        public static void WriteIntPartBits(this BitArray bits, double valueDouble, int countFracBit){
            BigInteger intPart = (BigInteger)valueDouble;
            BigInteger one = 1;
            for (int i = countFracBit; i < bits.Length; i++){
                if ((intPart & (one << (i - countFracBit))) != 0){
                    bits[i] = true;
                }
            }
        }

        public static void WriteFracPartBits(this BitArray bits, double valueDouble, int countFracBit){
            double fracPart = valueDouble - (long)valueDouble;
            double index = 0.5;
            for (int i = countFracBit - 1; i >= 0; i--){
                if (fracPart >= index){
                    bits[i] = true;
                    fracPart -= index;
                }
                index /= 2;
            }
        }

        public static BitArray Add(this BitArray bits, BitArray bits2){
            bool carry = false;
            int maxLen = Math.Max(bits.Length, bits2.Length);
            if (bits.Length < maxLen){
                bits.Length = maxLen;
            }
            if (bits2.Length < maxLen){
                bits2.Length = maxLen;
            }
            for (int i = 0; i < bits.Length; i++){
                if (bits[i] & bits2[i]){
                    if (carry){
                        bits[i] = true;
                    }
                    else{
                        bits[i] = false;
                        carry = true;
                    }
                }
                else if (bits[i] | bits2[i]){
                    bits[i] = !carry;
                }
                else{
                    if (carry){
                        bits[i] = true;
                        carry = false;
                    }
                    else{
                        bits[i] = false;
                    }
                }
            }
            return bits;
        }
    }

    public static class Extands{
        public static T SumAll<T>(this List<T> array)
            where T : struct, INumber<T>
        {
            T result = new T();
            for (int i = 0; i < array.Count(); i++){
                result = result + array[i];
            }
            return result;
        }

        public static Fixed<T, F> SumAll<T, F>(this List<Fixed<T, F>> array)
            where T : struct, IComparable, IConvertible, IFormattable
            where F : struct, IDot
        {
            Fixed<T, F> result = new Fixed<T, F>(0);
            for (int i = 0; i < array.Count(); i++){
                result = result + array[i];
            }
            return result;
        }
    }
}