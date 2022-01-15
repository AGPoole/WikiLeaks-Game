using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectMaths
{
    // Mod, but also for negative values
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

    public static int HexagonGridDistance(int iX1, int iY1, int iX2, int iY2)
    {
        // start at bottom co-ordinate
        if (iY1 > iY2)
        {
            int iTempX = iX1;
            iX1 = iX2;
            iX2 = iTempX;

            int iTempY = iY1;
            iY1 = iY2;
            iY2 = iTempY;
        }

        if (iX1 == iX2)
        {
            return Math.Abs(iY1 - iY2);
        }
        if (iY1 == iY2)
        {
            return Math.Abs(iX1 - iX2);
        }

        bool bIs1Peak = Mod(iX1, 2) == 1;
        bool bIs2Peak = Mod(iX2, 2) == 1;

        // case 1: trough to trough or peak to peak
        if (bIs1Peak == bIs2Peak)
        {
            int iXDifference = Math.Abs(iX1 - iX2);
            int iYDifference = Math.Abs(iY1 - iY2);

            if (iXDifference < 2 * iYDifference)
            {
                // Move in across, up-across increments until below the target, then move up
                return (iXDifference / 2) + iYDifference;
            }
            else
            {
                // Move in across, up-across increments, then just move right
                return iXDifference;
            }
        }

        // case 2: peak to trough
        if (bIs1Peak && !bIs2Peak)
        {
            // To do this, we reduce to case 1 by moving diagonally down in the wrong direction.
            // This takes to a trough to trough where we know the first move would be diagonally
            // up. We can take away 1 from the result to then remove this

            int iXMoveAway = iX1 < iX2 ? -1 : 1;
            return HexagonGridDistance(iX1 + iXMoveAway, iY1 - 1, iX2, iY2) - 1;
        }

        // case 3: trough to peak
        // the first move will be diagonally up. Then we have case 1
        int iXMoveToward = iX1 < iX2 ? 1 : -1;
        return HexagonGridDistance(iX1 + iXMoveToward, iY1 + 1, iX2, iY2) + 1;
    }

    // class to store a distribution with different probabilities for different things
    public class Distribution<T>
    {
        List<(T, float)> m_xProbabilities;
        public Distribution(List<T> xOutcomes, Func<T, int, float> xPDf)
        {
            m_xProbabilities = new List<(T, float)>();
            float fTotalProb = 0;


            for (int i = 0; i < xOutcomes.Count; i++)
            {
                float fNewValue = xPDf(xOutcomes[i], i);
                if (fNewValue >= 0f)
                {
                    m_xProbabilities.Add((xOutcomes[i], fNewValue));
                    fTotalProb += fNewValue;
                }
                else
                {
                    Debug.LogError("Probability relative value less than 0. The calculations will not work for this");
                }
            }
            if (fTotalProb <= 0f || Mathf.Approximately(fTotalProb, 0f))
            {
                Debug.LogError("All probabilities are 0. This distribution will not work");
            }
            else
            {
                // Normalise the results
                for (int i = 0; i < m_xProbabilities.Count; i++)
                {
                    m_xProbabilities[i] = (m_xProbabilities[i].Item1, m_xProbabilities[i].Item2 / fTotalProb);
                }
            }
        }

        public T Sample()
        {
            float fValue = RandomRange(0f, 1f);
            float fRunningTotal = 0;
            for (int i = 0; i < m_xProbabilities.Count; i++)
            {
                fRunningTotal += m_xProbabilities[i].Item2;
                if (fRunningTotal > fValue)
                {
                    return m_xProbabilities[i].Item1;
                }
            }
            return m_xProbabilities[m_xProbabilities.Count - 1].Item1;
        }
    }

    // TODO: use this across the project, make it deterministic and make the seed clone-able, for use in
    // cloned systems
    static float RandomRange(float fMin, float fMax)
    {
        return UnityEngine.Random.Range(fMin, fMax);
    }
}
