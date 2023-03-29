using System.Numerics;

using Dots;

namespace Cuni.Arithmetics.FixedPoint{
    
    public struct Fixed<TBackingType, TPrecision> : IAdditionOperators<Fixed<TBackingType, TPrecision>, Fixed<TBackingType, TPrecision>, Fixed<TBackingType, TPrecision>>
        where TBackingType : IBinaryInteger<TBackingType>
        where TPrecision: struct, IDot
    {
        TBackingType back;
        TPrecision precision;

        public Fixed(double valueDouble = 0){
            precision = new TPrecision();
            back = TBackingType.CreateSaturating(0);

            BigInteger IntPart = BigInteger.CreateSaturating(valueDouble);
            BigInteger mask = BigInteger.CreateSaturating((new BigInteger(1) << (back.GetByteCount() * 8 + 1)) - 1);

            IntPart &= mask;

            back = TBackingType.CreateSaturating(IntPart);
            back <<= precision.Value();
            
            if ((long)valueDouble != valueDouble){
                back |= TBackingType.CreateSaturating((long)((valueDouble - (long)valueDouble) * (1 << precision.Value())));
            }
        }

        public double ToDouble(){
            TBackingType IntPart = back >> precision.Value();
            TBackingType FracPart = back & TBackingType.CreateSaturating((1 << precision.Value()) - 1);
            double result = Convert.ToDouble(IntPart) + Convert.ToDouble(FracPart) / (1 << precision.Value());
            return result;
        }

        public override string ToString(){
            return ToDouble().ToString();
        }

        public static Fixed<TBackingType, TPrecision> operator +(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            Fixed<TBackingType, TPrecision> result = new Fixed<TBackingType, TPrecision>();
            result.back = x1.back + x2.back;
            return result;
        }

        public static Fixed<TBackingType, TPrecision> operator -(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            Fixed<TBackingType, TPrecision> result = new Fixed<TBackingType, TPrecision>();
            result.back = x1.back - x2.back;
            return result;
        }

        public static Fixed<TBackingType, TPrecision> operator *(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            Fixed<TBackingType, TPrecision> result = new Fixed<TBackingType, TPrecision>();
            BigInteger temp = BigInteger.CreateSaturating(x1.back) * BigInteger.CreateSaturating(x2.back);
            BigInteger mask = BigInteger.CreateSaturating((new BigInteger(1) << x2.back.GetByteCount()*8+1) - 1);
            temp >>= x2.precision.Value();
            temp &= mask;
            result.back = TBackingType.CreateSaturating(temp);
            return result;
        }

        public static Fixed<TBackingType, TPrecision> operator /(Fixed<TBackingType, TPrecision> x1, Fixed<TBackingType, TPrecision> x2){
            Fixed<TBackingType, TPrecision> result = new Fixed<TBackingType, TPrecision>();
            BigInteger temp = BigInteger.CreateSaturating(x1.back) << x2.precision.Value();
            temp /= BigInteger.CreateSaturating(x2.back);
            result.back = TBackingType.CreateSaturating(temp);
            return result;
        }

        public Fixed<TBackingType, TPrecisionTo> To<TPrecisionTo>() where TPrecisionTo : struct, IDot
        {
            Fixed<TBackingType, TPrecisionTo> result = new Fixed<TBackingType, TPrecisionTo>();
            var difference = result.precision.Value() - this.precision.Value();
            result.back = (difference > 0) ? this.back << difference : this.back >> difference * -1;
            return result;
        }
    }

    public static class Extands{
        public static T SumAll<T>(this List<T> array) where T : IAdditionOperators<T,T,T>, new(){
            T result = new T();
            foreach (var item in array){
                result += item;
            }
            return result;
        }
    }
}