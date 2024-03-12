namespace Milengine.NET.Core.Utilities;

public interface IFactoryInstance<TResult, TArg1>
{
    public static abstract TResult CreateInstance(TArg1 argumentOne);
}
public interface IFactoryInstance<TResult, TArg1, TArg2>
{
    public static abstract TResult CreateInstance(TArg1 argumentOne, TArg2 argumentTwo);
}

public interface IFactoryInstance<TResult, TArg1, TArg2, TArg3>
{
    public static abstract TResult CreateInstance(TArg1 argumentOne, TArg2 argumentTwo, TArg3 argumentThree);
}

public interface IFactoryInstance<TResult, TArg1, TArg2, TArg3, TArg4>
{
    public static abstract TResult CreateInstance(TArg1 argumentOne, TArg2 argumentTwo, TArg3 argumentThree, TArg4 argumentFour);
}

public interface IFactoryInstance<TResult, TArg1, TArg2, TArg3, TArg4,
    TArg5, TArg6, TArg7, TArg8, TArg9>
{
    public static abstract TResult CreateInstance(TArg1 argumentOne, TArg2 argumentTwo, TArg3 argumentThree, TArg4 argumentFour,
        TArg5 argumentFive, TArg6 argumentSix, TArg7 argumentSeven, TArg8 argumentEight, TArg9 argumentNine);
}