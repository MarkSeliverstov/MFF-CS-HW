namespace Dots
{
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
}