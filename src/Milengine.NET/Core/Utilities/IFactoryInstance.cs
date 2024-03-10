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