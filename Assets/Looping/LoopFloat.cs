using UnityEngine;
using System.Collections;

struct LoopFloat
{
    float value;
    float min, max;
    float delta => max - min;

    public LoopFloat(float value, float min, float max)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        CycleValue(ref this);
    }

    public void SetValue(float value)
    {
        this.value = value;
        CycleValue(ref this);
    }

    private static void CycleValue(ref LoopFloat a)
    {
        a.value %= a.delta;
        if (a.value < a.min) a.value += a.delta;
    }

    public static LoopFloat operator +(LoopFloat a, LoopFloat b)
    {
        a.value += b.value;
        CycleValue(ref a);
        return a;
    }

    public static LoopFloat operator -(LoopFloat a, LoopFloat b)
    {
        a.value -= b.value;
        CycleValue(ref a);
        return a;
    }

    public static LoopFloat operator +(LoopFloat a, float b)
    {
        a.value += b;
        CycleValue(ref a);
        return a;
    }

    public static LoopFloat operator -(LoopFloat a, float b)
    {
        a.value -= b;
        CycleValue(ref a);
        return a;
    }

    public static LoopFloat operator ++(LoopFloat a)
    {
        a.value++;
        CycleValue(ref a);
        return a;
    }

    public static LoopFloat operator --(LoopFloat a)
    {
        a.value--;
        CycleValue(ref a);
        return a;
    }

    public static bool operator >(LoopFloat a, LoopFloat b)
    {
        return a.value > b.value;
    }

    public static bool operator <(LoopFloat a, LoopFloat b)
    {
        return a.value < b.value;
    }

    public static bool operator <=(LoopFloat a, LoopFloat b)
    {
        return a.value <= b.value;
    }

    public static bool operator >=(LoopFloat a, LoopFloat b)
    {
        return a.value >= b.value;
    }

    public static bool operator ==(LoopFloat a, LoopFloat b)
    {
        return a.value == b.value;
    }

    public static bool operator !=(LoopFloat a, LoopFloat b)
    {
        return a.value != b.value;
    }

    public static implicit operator float(LoopFloat a) => a.value;

    public override string ToString()
    {
        return string.Format("{0} ,({1} , {2})", value, min, max);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is LoopFloat)) return false;
        return (LoopFloat)obj == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public float Delta(LoopFloat b)
    {
        return Delta(b.value);
    }

    public float Delta(float b)
    {
        float v1 = b - value;
        float v2 = v1 - delta;
        if (Mathf.Abs(v1) < Mathf.Abs(v2)) return v1;
        else return v2;
    }
}
