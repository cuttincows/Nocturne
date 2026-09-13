using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    // Ported from FlashPunk's Ease.as class, with Boomerang functionality added
    // Likely based off of Robert Penner's functions http://www.robertpenner.com/easing/

public delegate float EaseDelegate(float t, bool boomerang = false);

public static class Ease
{
    public enum EaseType
    { QUAD, BOUCE, CUBE, QUART, QUINT, SINE, CIRC, EXPO, BACK }

    public enum DelegateType
    { IN, OUT, IN_OUT }

    public static EaseDelegate QuadIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t * t;
    };

    public static EaseDelegate QuadOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return -t * (t - 2);
    };

    public static EaseDelegate QuadInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t <= .5 ? t * t * 2 : 1 - (--t) * t * 2;
    };

    public static EaseDelegate BounceIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        t = 1 - t;
        if (t < B1) return 1.0f - 7.5625f * t * t;
        if (t < B2) return 1.0f - (7.5625f * (t - B3) * (t - B3) + 0.75f);
        if (t < B4) return 1.0f - (7.5625f * (t - B5) * (t - B5) + 0.9375f);
        return 1.0f - (7.5625f * (t - B6) * (t - B6) + .984375f);
    };

    internal static object GetDelegate(object delType, object easeType)
    {
        throw new NotImplementedException();
    }

    /** Bounce out. */
    public static EaseDelegate BounceOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        if (t < B1) return 7.5625f * t * t;
        if (t < B2) return 7.5625f * (t - B3) * (t - B3) + 0.75f;
        if (t < B4) return 7.5625f * (t - B5) * (t - B5) + 0.9375f;
        return 7.5625f * (t - B6) * (t - B6) + 0.984375f;
    };

    /** Bounce in and out. */
    public static EaseDelegate BounceInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        if (t < .5)
        {
            t = 1 - t * 2;
            if (t < B1) return (1.0f - 7.5625f * t * t) / 2;
            if (t < B2) return (1.0f - (7.5625f * (t - B3) * (t - B3) + 0.75f)) / 2.0f;
            if (t < B4) return (1.0f - (7.5625f * (t - B5) * (t - B5) + 0.9375f)) / 2.0f;
            return (1.0f - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2.0f;
        }
        t = t * 2 - 1;
        if (t < B1) return (7.5625f * t * t) / 2.0f + 0.5f;
        if (t < B2) return (7.5625f * (t - B3) * (t - B3) + 0.75f) / 2.0f + 0.5f;
        if (t < B4) return (7.5625f * (t - B5) * (t - B5) + 0.9375f) / 2.0f + 0.5f;
        return (7.5625f * (t - B6) * (t - B6) + 0.984375f) / 2.0f + 0.5f;
    };

    public static EaseDelegate CubeIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t * t * t;
    };
        
    public static EaseDelegate CubeOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return 1 + (--t) * t * t;
    };

    public static EaseDelegate CubeInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t <= 0.5f ? t * t * t * 4.0f : 1.0f + (--t) * t * t * 4.0f;
    };

    public static EaseDelegate QuartIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t * t * t * t;
    };

    public static EaseDelegate QuartOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return 1.0f - (t -= 1.0f) * t * t * t;
    };

    public static EaseDelegate QuartInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t <= 0.5f ? t * t * t * t * 8.0f : (1.0f - (t = t * 2.0f - 2.0f) * t * t * t) / 2.0f + 0.5f;
    };
        
    public static EaseDelegate QuintIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t * t * t * t * t;
    };
        
    public static EaseDelegate QuintOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return (t = t - 1) * t * t * t * t + 1;
    };
        
    public static EaseDelegate QuintInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return ((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2;
    };

    public static EaseDelegate SineIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return -((float)Math.Cos(PI2 * t)) + 1;
    };
        
    public static EaseDelegate SineOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return (float)Math.Sin(PI2 * t);
    };
        
    public static EaseDelegate SineInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return ((float)-Math.Cos(PI * t)) / 2 + .5f;
    };
        
    public static EaseDelegate CircIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
		return -((float)Math.Sqrt(1 - t* t) - 1);
    };
        
    public static EaseDelegate CircOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
		return (float)Math.Sqrt(1 - (t - 1) * (t - 1));
	};

    public static EaseDelegate CircInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
		return t <= .5 ? ((float)Math.Sqrt(1 - t* t * 4) - 1) / -2 : ((float)Math.Sqrt(1 - (t* 2 - 2) * (t* 2 - 2)) + 1) / 2;
	};
        
    public static EaseDelegate ExpoIn = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return (float)Math.Pow(2, 10 * (t - 1));
    };
        
    public static EaseDelegate ExpoOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return (float)-Math.Pow(2, -10 * t) + 1;
    };
        
    public static EaseDelegate ExpoInOut = delegate (float t, bool boomerang)
    {
        if (boomerang) t = HandleBoomerang(t);
        return t < .5 ? (float)Math.Pow(2, 10 * (t * 2 - 1)) / 2 : ((float)-Math.Pow(2, -10 * (t * 2 - 1)) + 2) / 2;
    };

    public static EaseDelegate backIn = delegate (float t, bool boomerang)
    {
        return t * t * (2.70158f * t - 1.70158f);
    };
        
    public static EaseDelegate backOut = delegate (float t, bool boomerang)
    {
        return 1 - (--t) * (t) * (-2.70158f * t - 1.70158f);
    };
        
    public static EaseDelegate backInOut = delegate (float t, bool boomerang)
    {
        t *= 2;
        if (t < 1) return t * t * (2.70158f * t - 1.70158f) / 2f;
        t--;
        return (1 - (--t) * (t) * (-2.70158f * t - 1.70158f)) / 2f + .5f;
    };

    private static float HandleBoomerang(float t)
    {
        return t = ((t < 0.5) ? t : 1 - t) * 2;
    }

    private static float PI = (float)Math.PI;
    private static float PI2 = (float)Math.PI / 2;
    private static float B1 = 1.0f / 2.75f;
    private static float B2 = 2.0f / 2.75f;
    private static float B3 = 1.5f / 2.75f;
    private static float B4 = 2.5f / 2.75f;
    private static float B5 = 2.25f / 2.75f;
    private static float B6 = 2.625f / 2.75f;
        
    public static EaseDelegate[] InDelegates =
    { QuadIn, BounceIn, CubeIn, QuartIn, QuintIn, SineIn, CircIn, ExpoIn, backIn };

    public static EaseDelegate[] OutDelegates =
    { QuadOut, BounceOut, CubeOut, QuartOut, QuintOut, SineOut, CircOut, ExpoOut, backOut };

    public static EaseDelegate[] InOutDelegates =
    { QuadInOut, BounceInOut, CubeInOut, QuartInOut, QuintInOut, SineInOut, CircInOut, ExpoInOut, backInOut };

    public static EaseDelegate[][] DelegateTypeArrs =
    { InDelegates, OutDelegates, InOutDelegates };

    public static EaseDelegate GetDelegate(DelegateType delegateType, EaseType easeType)
    {
        return DelegateTypeArrs[(int)delegateType][(int)easeType];
    }

}