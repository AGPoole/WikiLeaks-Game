using System;
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

    // class to store a distribution with different probabilities for different things
    public class Distribution<T>
    {
        List<(T, float)> m_xProbabilities;
        public Distribution(List<T> xOutcomes, Func<T, int, float> xPDf){
            m_xProbabilities = new List<(T, float)>();
            float fTotalProb = 0;
            for(int i = 0; i < xOutcomes.Count; i++)
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
            if (fTotalProb == 0)
            {
                Debug.LogError("All probabilities are 0. This distribution will not work");
            }
            else
            {
                for (int i = 0; i < m_xProbabilities.Count; i++)
                {
                    m_xProbabilities[i] = (m_xProbabilities[i].Item1, m_xProbabilities[i].Item2 / fTotalProb);
                }
            }
        }

        public T Sample()
        {
            float fValue = UnityEngine.Random.Range(0f, 1f);
            float fRunningTotal = 0;
            for(int i=0; i<m_xProbabilities.Count; i++)
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
}
