struct FixedPoint64
{
    uint low;
    int high;

    static FixedPoint64 FromInt(int value)
    {
        FixedPoint64 result;
        result.low = asuint(value);
        result.high = value < 0 ? -1 : 0;
        return result;
    }

    static FixedPoint64 FromFloat(float value)
    {
        FixedPoint64 result;
        result.low = asuint(fmod(value, 1.0) * exp2(32));
        result.high = trunc(value);
        return result;
    }

    int AsInt()
    {
        return high;
    }

    float AsFloat()
    {
        return high + asfloat(low) / exp2(32);
    }
    
    int countLeadingZeros(int x)
    {
        uint ux = asuint(x);
        ux |= (ux >> 1);
        ux |= (ux >> 2);
        ux |= (ux >> 4);
        ux |= (ux >> 8);
        ux |= (ux >> 16);
        return 32 - countbits(ux);
    }
    
    bool GreaterThanOrEqual(FixedPoint64 a, FixedPoint64 b)
    {
        if (a.high > b.high)
            return true;
        else if (a.high < b.high)
            return false;
        else
            return a.low >= b.low;
    }

    static FixedPoint64 Add(FixedPoint64 a, FixedPoint64 b)
    {
        FixedPoint64 result;
        result.low = a.low + b.low;
        result.high = a.high + b.high + (result.low < a.low ? 1 : 0);
        return result;
    }

    static FixedPoint64 Subtract(FixedPoint64 a, FixedPoint64 b)
    {
        FixedPoint64 result;
        result.low = a.low - b.low;
        result.high = a.high - b.high - (result.low > a.low ? 1 : 0);
        return result;
    }

    static FixedPoint64 Multiply(FixedPoint64 a, FixedPoint64 b)
    {
        uint al = a.low & 0xFFFF;
        uint ah = a.low >> 16;
        uint bl = b.low & 0xFFFF;
        uint bh = b.low >> 16;

        uint lowlow = al * bl;
        uint lowhigh1 = al * bh;
        uint lowhigh2 = ah * bl;
        uint lowhigh = lowhigh1 + lowhigh2 + (lowlow >> 16);
        uint highlow = ah * bh + (lowhigh >> 16);

        FixedPoint64 result;
        result.low = (lowhigh << 16) | (lowlow & 0xFFFF);
        result.high = int(a.high * b.high + highlow + (a.high * b.low + a.low * b.high));
        return result;
    }

    static FixedPoint64 Divide(FixedPoint64 a, FixedPoint64 b)
    {
        bool negativeResult = false;

        if (a.high < 0)
        {
            negativeResult = !negativeResult;
            a.high = -a.high - (a.low != 0 ? 1 : 0);
            a.low = -a.low;
        }

        if (b.high < 0)
        {
            negativeResult = !negativeResult;
            b.high = -b.high - (b.low != 0 ? 1 : 0);
            b.low = -b.low;
        }

        int shiftCount = countLeadingZeros(b.high) - countLeadingZeros(a.high);
        
        if (shiftCount < 0)
            shiftCount = 0;

        b.high <<= shiftCount;

        for (int i = shiftCount; i >= 0; i--)
            if (GreaterThanOrEqual(a, b))
                a -= b;

        if (shiftCount > 31)
            shiftCount -= 32;
        else
            b.high >>= shiftCount;

        if (negativeResult)
            return FromInt(-a.AsInt());
        else
            return FromInt(a.AsInt());
    }
};