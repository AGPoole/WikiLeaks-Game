using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectMaths
{
    public static int Mod(int i1, int i2)
    {
        return (i1 % i2 + i2) % i2;
    }
}
