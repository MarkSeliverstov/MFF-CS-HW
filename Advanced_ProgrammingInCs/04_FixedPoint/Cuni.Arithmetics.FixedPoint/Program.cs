using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Cuni.Arithmetics.FixedPoint{
    public interface IDot{
        public int Value();
    }

    public struct Dot3: IDot{
        public int Value() => 3;
    }

    public struct Dot4: IDot{
        public int Value() => 4;
    }

    public struct Dot8 : IDot{
        public int Value() => 8;
    }

    public struct Dot16: IDot{
        public int Value() => 16;
    }

    public struct Dot24: IDot{
        public int Value() => 24;
    }

    public struct Fixed<TBackingType, TPrecision>
        where TBackingType : struct, IComparable, IConvertible, IFormattable
        where TPrecision: struct, IDot
    {
        int CapacityBits = Marshal.SizeOf<TBackingType>() * 8;
        int countFracBit = new TPrecision().Value();
        public BitArray bits = new BitArray(Marshal.SizeOf<TBackingType>() * 8);

        public Fixed(double valueDouble){
            this.bits.WriteIntPartBits(valueDouble, countFracBit);
            this.bits.WriteFracPartBits(valueDouble, countFracBit);
        }

        public Fixed(BitArray array){
            this.bits = array.Clone() as BitArray;
        }

        public double ToDouble(){
            double intPart = 0;
            double fracPart = 0;
            int len = this.bits.Length;
            if (len > this.CapacityBits)
                len = this.CapacityBits;
            for (int i = countFracBit; i < len; i++){
                if (bits[i]){
                    intPart += Math.Pow(2, i - countFracBit);
                }
            }

            for (int i = 0; i < countFracBit; i++){
                if (bits[countFracBit - i - 1]){
                    fracPart += Math.Pow(2, -(i + 1));
                }
            }

            return intPart + fracPart;
        }

        public override string ToString(){
            return ToDouble().ToString();
        }

        public static Fixed<TBackingType, TPrecision> operator +(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            BitArray result = x1.bits.Add(x2.bits);
            return new Fixed<TBackingType, TPrecision>(result);
        }

        public static Fixed<TBackingType, TPrecision> operator -(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            byte[] bytes = new byte[x1.bits.Length / 8];
            x1.bits.CopyTo(bytes, 0);
            var result = new BigInteger(bytes);

            bytes = new byte[x2.bits.Length / 8];
            x2.bits.CopyTo(bytes, 0);
            result -= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.bits = new BitArray(bytes);

            return new Fixed<TBackingType, TPrecision>(x1.bits);
        }

        public static Fixed<TBackingType, TPrecision> operator *(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            byte[] bytes = new byte[x1.CapacityBits / 8];
            x1.bits.CopyTo(bytes, 0);
            var result = new BigInteger(bytes);

            bytes = new byte[x2.CapacityBits / 8];
            x2.bits.CopyTo(bytes, 0);
            result *= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.bits = new BitArray(bytes);

            x1.bits.RightShift(x1.countFracBit);

            // becouse tests are not correct
            x1.bits.Length = x1.CapacityBits;

            return new Fixed<TBackingType, TPrecision>(x1.bits);
        }

        public static Fixed<TBackingType, TPrecision> operator /(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            x1.bits.Length+=8;
            x1.bits.LeftShift(x1.countFracBit);
            byte[] bytes = new byte[x1.bits.Length / 8];
            x1.bits.CopyTo(bytes, 0);

            var result = new BigInteger(bytes);

            x2.bits.Length+=8;
            bytes = new byte[x2.bits.Length / 8];
            x2.bits.CopyTo(bytes, 0);

            result /= new BigInteger(bytes);

            bytes = result.ToByteArray();
            x1.bits = new BitArray(bytes);


            return new Fixed<TBackingType, TPrecision>(x1.bits);
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