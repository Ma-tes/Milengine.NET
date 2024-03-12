using System.Runtime.CompilerServices;

namespace Milengine.NET.Core.Utilities.InlineOptimalizations.Buffers.InlineParameterBuffer;

public interface IFixedParameter<TInlineParameter, T> : IFactoryInstance<TInlineParameter, T>
    where TInlineParameter : IInlineIndexParameter
{
}

public readonly struct InlineValueParameter_One<T> : IFactoryInstance<InlineParameter_One<T>, T>
{
    public readonly InlineParameter_One<T> InlineArray = new();

    public InlineValueParameter_One(T argumentOne)
    { 
        InlineArray[0] = argumentOne;
    }

    public static InlineParameter_One<T> CreateInstance(T argumentOne) =>
        new InlineValueParameter_One<T>(argumentOne).InlineArray;
}

public readonly struct InlineValueParameter_Two<T> : IFactoryInstance<InlineParameter_Two<T>, T, T>
{
    public readonly InlineParameter_Two<T> InlineArray = new();

    public InlineValueParameter_Two(T argumentOne, T argumentTwo)
    { 
        InlineArray[0] = argumentOne;
        InlineArray[1] = argumentTwo;
    }

    public static InlineParameter_Two<T> CreateInstance(T argumentOne, T argumentTwo) =>
        new InlineValueParameter_Two<T>(argumentOne, argumentTwo).InlineArray;
}

public readonly struct InlineValueParameter_Three<T> : IFactoryInstance<InlineParameter_Three<T>, T, T, T>
{
    public readonly InlineParameter_Three<T> InlineArray = new();

    public InlineValueParameter_Three(T argumentOne, T argumentTwo, T argumentThree)
    { 
        InlineArray[0] = argumentOne;
        InlineArray[1] = argumentTwo;
        InlineArray[2] = argumentThree;
    }

    public static InlineParameter_Three<T> CreateInstance(T argumentOne, T argumentTwo, T argumentThree) =>
        new InlineValueParameter_Three<T>(argumentOne, argumentTwo, argumentThree).InlineArray; 
}

public readonly struct InlineValueParameter_Four<T> : IFactoryInstance<InlineParameter_Four<T>, T, T, T, T>
{
    public readonly InlineParameter_Four<T> InlineArray = new();

    public InlineValueParameter_Four(T argumentOne, T argumentTwo, T argumentThree, T argumentFour)
    { 
        InlineArray[0] = argumentOne;
        InlineArray[1] = argumentTwo;
        InlineArray[2] = argumentThree;
        InlineArray[3] = argumentFour;
    }

    public static InlineParameter_Four<T> CreateInstance(T argumentOne, T argumentTwo, T argumentThree, T argumentFour) =>
        new InlineValueParameter_Four<T>(argumentOne, argumentTwo, argumentThree, argumentFour).InlineArray; 
}

public readonly struct InlineValueParameter_Eight<T> : IFactoryInstance<InlineParameter_Eight<T>, T, T, T, T, T, T, T, T>
{
    public readonly InlineParameter_Eight<T> InlineArray = new();

    public InlineValueParameter_Eight(T argumentOne, T argumentTwo, T argumentThree, T argumentFour,
        T argumentFive, T argumentSix, T argumentSeven, T argumentEight)
    { 
        InlineArray[0] = argumentOne;
        InlineArray[1] = argumentTwo;
        InlineArray[2] = argumentThree;
        InlineArray[3] = argumentFour;
        InlineArray[4] = argumentFive;
        InlineArray[5] = argumentSix;
        InlineArray[6] = argumentSeven;
        InlineArray[7] = argumentEight;
    }

    public static InlineParameter_Eight<T> CreateInstance(T argumentOne, T argumentTwo, T argumentThree, T argumentFour,
        T argumentFive, T argumentSix, T argumentSeven, T argumentEight) =>
        new InlineValueParameter_Eight<T>(argumentOne, argumentTwo, argumentThree, argumentFour,
            argumentFive, argumentSix, argumentSeven, argumentEight).InlineArray; 
}

public readonly struct InlineValueParameter_Nine<T> : IFactoryInstance<InlineParameter_Nine<T>, T, T, T, T, T, T, T, T, T>
{
    public readonly InlineParameter_Nine<T> InlineArray = new();

    public InlineValueParameter_Nine(T argumentOne, T argumentTwo, T argumentThree, T argumentFour,
        T argumentFive, T argumentSix, T argumentSeven, T argumentEight, T argumentNine)
    { 
        InlineArray[0] = argumentOne;
        InlineArray[1] = argumentTwo;
        InlineArray[2] = argumentThree;
        InlineArray[3] = argumentFour;
        InlineArray[4] = argumentFive;
        InlineArray[5] = argumentSix;
        InlineArray[6] = argumentSeven;
        InlineArray[7] = argumentEight;
        InlineArray[8] = argumentNine;
    }

    public static InlineParameter_Nine<T> CreateInstance(T argumentOne, T argumentTwo, T argumentThree, T argumentFour,
        T argumentFive, T argumentSix, T argumentSeven, T argumentEight, T argumentNine) =>
        new InlineValueParameter_Nine<T>(argumentOne, argumentTwo, argumentThree, argumentFour,
            argumentFive, argumentSix, argumentSeven, argumentEight, argumentNine).InlineArray; 
}

public interface IInlineIndexParameter
{
    public static abstract int Length { get; }
}

#pragma warning disable IDE0044
#pragma warning disable IDE0051

[InlineArray(1)]
public struct InlineParameter_One : IInlineIndexParameter
{
     private object _value; public static int Length { get; } = 1;
     public readonly object GetNonDirectInlineParameter(int index) => this[index];
}
[InlineArray(1)]
public struct InlineParameter_One<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 1;
}

[InlineArray(2)]
public struct InlineParameter_Two : IInlineIndexParameter
{
    private object _value; public static int Length { get; } = 2;
}

[InlineArray(2)]
public struct InlineParameter_Two<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 2;
}

[InlineArray(3)]
public struct InlineParameter_Three : IInlineIndexParameter
{
    private object _value; public static int Length { get; } = 3;
}
[InlineArray(3)]
public struct InlineParameter_Three<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 3;
}

[InlineArray(4)]
public struct InlineParameter_Four : IInlineIndexParameter
{
    private object _value; public static int Length { get; } = 4;
}
[InlineArray(4)]
public struct InlineParameter_Four<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 4;
}

[InlineArray(8)]
public struct InlineParameter_Eight : IInlineIndexParameter
{
    private object _value; public static int Length { get; } = 8;
}
[InlineArray(8)]
public struct InlineParameter_Eight<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 8;
}

[InlineArray(9)]
public struct InlineParameter_Nine : IInlineIndexParameter
{
    private object _value; public static int Length { get; } = 9;
}
[InlineArray(9)]
public struct InlineParameter_Nine<T> : IInlineIndexParameter
{
    private T _value; public static int Length { get; } = 9;
}

#pragma warning restore IDE0044
#pragma warning restore IDE0051