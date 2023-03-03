using System;

namespace GamePhysics
{
    public struct Meter{
        public double Value { get; set; }
        public override string ToString() => $"{Value}m";
        public Meter(double value) => Value = value;

        public static Meter operator +(Meter a, Meter b) => new Meter(a.Value + b.Value);
        public static Speed operator /(Meter distance, Second sec) => new Speed(distance.Value / sec.Value);
    }

    public struct Speed{
        public double Value { get; set; }
        public override string ToString() => $"{Value}m/s";
        public Speed(double value) => Value = value;

        public static Speed operator *(Speed speed, double factor) => new Speed(speed.Value * factor);
    }

    public struct Second{
        public double Value { get; set; }
        public override string ToString() => $"{Value}s";
        public Second(double value) => Value = value;
        //public static implicit operator int(Second sec) => (int)sec.Value;
    }

    public static class GamePhysicsExtensions
    {
        public static Meter Meters(this double value) => new Meter(value);
        public static Meter Meters(this int value) => new Meter(value);
        
        public static Second Seconds(this double value) => new Second(value);
        public static Second Seconds(this int value) => new Second(value);

        public static Speed MeterPerSeconds(this double value) => new Speed(value);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var originalDistance = 1.5.Meters();
            var deltaDistance = 2.Meters();
            var distance = originalDistance + deltaDistance;
            Console.WriteLine($"Moving {deltaDistance} after {originalDistance} travelled equals total distance of {distance}");

            var time = 3.Seconds();
            var speed = distance / time;
            Console.WriteLine($"Distance of {distance} travelled in {time} equals speed of {speed}");

            speed *= 2;
            Console.WriteLine($"Doubled speed: {speed}");

            speed = 3.5.MeterPerSeconds();
            Console.WriteLine($"New speed: {speed}");

            // int valueSec = (int)time;
            // !!! Uncommenting following line must produce Error: Operator '*=' cannot be applied to operands of type 'MeterPerSecond' and 'Meter'
            // speed *= distance;
            // !!! Uncommenting following line must produce Error: Operator '+=' cannot be applied to operands of type 'Meter' and 'Second'
            // distance += time;
        }
    }
}
