using UnityEngine;
using System.Collections;

struct LoopInt
{
    int value;
    int min, max;
    int delta => max - min;

    public LoopInt(int value, int min, int max)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        CycleValue(ref this);
    }

    public void SetValue(int value)
    {
        this.value = value;
        CycleValue(ref this);
    }

    private static void CycleValue(ref LoopInt a)
    {
        a.value %= a.delta;
        if (a.value < a.min) a.value += a.delta;
    }

    public static LoopInt operator +(LoopInt a, LoopInt b)
    {
        a.value += b.value;
        CycleValue(ref a);
        return a;
    }

    public static LoopInt operator -(LoopInt a, LoopInt b)
    {
        a.value -= b.value;
        CycleValue(ref a);
        return a;
    }

    public static LoopInt operator +(LoopInt a, int b)
    {
        a.value += b;
        CycleValue(ref a);
        return a;
    }

    public static LoopInt operator -(LoopInt a, int b)
    {
        a.value -= b;
        CycleValue(ref a);
        return a;
    }

    public static LoopInt operator ++(LoopInt a)
    {
        a.value ++;
        CycleValue(ref a);
        return a;
    }

    public static LoopInt operator --(LoopInt a)
    {
        a.value--;
        CycleValue(ref a);
        return a;
    }

    public static bool operator >(LoopInt a, LoopInt b)
    {
        return a.value > b.value;
    }

    public static bool operator <(LoopInt a, LoopInt b)
    {
        return a.value < b.value;
    }

    public static bool operator <=(LoopInt a, LoopInt b)
    {
        return a.value <= b.value;
    }

    public static bool operator >=(LoopInt a, LoopInt b)
    {
        return a.value >= b.value;
    }

    public static bool operator ==(LoopInt a, LoopInt b)
    {
        return a.value == b.value;
    }

    public static bool operator !=(LoopInt a, LoopInt b)
    {
        return a.value != b.value;
    }

    public static implicit operator int(LoopInt a) => a.value;

    public override string ToString()
    {
        return string.Format("{0} ,({1} , {2})", value, min, max);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is LoopInt)) return false;
        return (LoopInt)obj == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public int Delta(LoopInt b)
    {
        return Delta(b.value);
    }

    public int Delta(int b)
    {
        int v1 = b - value;
        int v2 = v1 - delta;
        if (Mathf.Abs(v1) < Mathf.Abs(v2)) return v1;
        else return v2;
    }
}