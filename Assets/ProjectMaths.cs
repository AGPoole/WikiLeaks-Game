using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectMaths
{
    public static int Mod(int i1, int i2)
    {
        return (i1 % i2 + i2) % i2;
    }

    // C# does not have in-built min/max for ints
    public static int Min(int i1, int i2)
    {
        return i1 < i2 ? i1 : i2;
    }
    public static int Max(int i1, int i2)
    {
        return i1 > i2 ? i1 : i2;
    }
}
