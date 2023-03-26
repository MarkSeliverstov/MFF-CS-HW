using System;
using System.Numerics;
using System.Runtime.InteropServices;

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

    public struct Fixed<T, F> : IComparable
        where T : struct, IComparable, IConvertible, IFormattable
        where F: struct, IDot
    {
        int CapacityBits = Marshal.SizeOf<T>() * 8;
        int countFracBit = new F().Value();
        public long value;

        public Fixed(double valueDouble){
            long intPart = (long)valueDouble;
            double frac = valueDouble - intPart;

            this.value = (intPart << countFracBit) + frac.GetFracBits(countFracBit);
        }

        public double ToDouble(){
            long one = 1;
            long mask = (one << countFracBit) - 1;
            if (CapacityBits == 64)
                mask = long.MaxValue;
            this.value = this.value & mask;
            long intPart = value >> countFracBit;

            double fracPart = 0;
            double index = 0.5;
            for (int i = countFracBit - 1; i >= 0; i--){
                if ((value & (1 << i)) != 0){
                    fracPart += index;
                }
                index /= 2;
            }
            return intPart + fracPart;
        }

        public static Fixed<T, F> operator +(Fixed<T, F> x1, Fixed<T, F> x2){
            x1.value += x2.value;
            return x1;
        }

        public static Fixed<T, F> operator -(Fixed<T, F> x1, Fixed<T, F> x2){
            x1.value -= x2.value;
            return x1;
        }

        public static Fixed<T, F> operator *(Fixed<T, F> x1, Fixed<T, F> x2){
            x1.value *= x2.value;
            x1.value >>= x1.countFracBit;
            return x1;
        }

        public static Fixed<T, F> operator /(Fixed<T, F> x1, Fixed<T, F> x2){
            x1.value <<= x1.countFracBit;
            x1.value /= x2.value;
            return x1;
        }

        public override string ToString(){
            return ToDouble().ToString();
        }

        public Fixed<int, T1> To<T1>()
            where T1 : struct, IDot
        {
            return new Fixed<int, T1>(this.ToDouble());
        }

        public int CompareTo(object? obj)
        {
            throw new NotImplementedException();
        }
    }

    public static class BitsOperations{
        public static long GetFracBits(this double value, int countFracBit){
            double index = 0.5;
            long result = 0;
            for (int i = countFracBit - 1; i >= 0; i--){
                if (value >= index){
                    result |= 1 << i;
                    value -= index;
                }
                index /= 2;
            }
            return result;
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