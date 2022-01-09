using UnityEngine;

public enum Gender
{
    MALE,
    FEMALE
}

public class CandidateValues : MonoBehaviour
{
    [SerializeField]
    FirstName[] m_xFirstNames;
    [SerializeField]
    LastName[] m_xLastNames;
    [SerializeField]
    [Range(0, 1)]
    float m_fMiddleInitialProb = 1f;
    [Range(0, 1)]
    float m_fSuffixProb = 0.5f;
    [SerializeField]
    Scandal[] m_xScandals;
    [SerializeField]
    float m_fScandalEffect;
    [SerializeField]
    int m_iScandalLength;
    [SerializeField]
    bool m_bCandidateMovementEnabled = true;
    [SerializeField]
    float m_fCandidateChangeBoundary = 0.05f;
    [SerializeField]
    float m_fCandidateTaxDifference = 0.15f;
    [SerializeField]
    float m_fMinTax = 0.05f;
    [SerializeField]
    float m_fMaxTax = 0.85f;

    static CandidateValues s_xStaticInstance;

    static CandidateValues GetCandidateValues()
    {
        // WSTODO: in awake, count instances to ensure there is only one
        if (s_xStaticInstance == null)
        {
            s_xStaticInstance = FindObjectOfType(typeof(CandidateValues)) as CandidateValues;
        }
        return s_xStaticInstance;
    }
    public static Name GetRandomName(Orientation eOrientation)
    {
        Name xName = GetRandomName(eOrientation, Gender.MALE);
        xName.m_xFirstName = s_xStaticInstance.m_xFirstNames[Random.Range(0, s_xStaticInstance.m_xFirstNames.Length)];
        return xName;
    }
    public static Name GetRandomName(Orientation eOrientation, Gender eGender)
    {
        Name xName = new Name();

        // TODO: change to filter
        do
        {
            xName.m_xFirstName = GetCandidateValues().m_xFirstNames[Random.Range(0, s_xStaticInstance.m_xFirstNames.Length)];
        } while (xName.m_xFirstName.m_xGender != eGender);

        do
        {
            xName.m_xLastName = s_xStaticInstance.m_xLastNames[Random.Range(0, s_xStaticInstance.m_xLastNames.Length)];
        } while (System.Array.IndexOf(xName.m_xLastName.m_xOrientations, eOrientation) == -1);

        xName.m_bShowInitial = Random.Range(0f, 1f) < s_xStaticInstance.m_fMiddleInitialProb;
        if (xName.m_bShowInitial)
        {
            xName.m_cMiddleInitial = (char)Random.Range((int)65, (int)89);
        }

        if (Random.Range(0f, 1f) < s_xStaticInstance.m_fSuffixProb)
        {
            xName.m_eSuffix = Random.Range(0f, 1f) < 0.5f ? Name.NameSuffix.Jr : Name.NameSuffix.Sr;
        }
        else
        {
            xName.m_eSuffix = Name.NameSuffix.None;
        }

        return xName;
    }

    public static Scandal GetRandomScandal()
    {
        return GetCandidateValues().m_xScandals[Random.Range(0, GetCandidateValues().m_xScandals.Length)];
    }

    public static float GetScandalEffect()
    {
        return GetCandidateValues().m_fScandalEffect;
    }

    public static int GetScandalLength()
    {
        return GetCandidateValues().m_iScandalLength;
    }

    public static float GetChangeBoundary()
    {
        return GetCandidateValues().m_fCandidateChangeBoundary;
    }

    public static float GetTaxDifference()
    {
        return GetCandidateValues().m_fCandidateTaxDifference;
    }

    public static float GetMaxTax()
    {
        return GetCandidateValues().m_fMaxTax;
    }
    public static float GetMinTax()
    {
        return GetCandidateValues().m_fMinTax;
    }

    public static bool GetCandidateMovementEnabled()
    {
        return GetCandidateValues().m_bCandidateMovementEnabled;
    }

    [System.Serializable]
    public class FirstName
    {
        public string m_xName;
        public Gender m_xGender;
    }

    [System.Serializable]
    public class LastName
    {
        public string m_xName;
        public Orientation[] m_xOrientations;
    }

    [System.Serializable]
    public class Name
    {
        public FirstName m_xFirstName;
        public LastName m_xLastName;

        public bool m_bShowInitial = false;
        public char m_cMiddleInitial;

        public enum NameSuffix
        {
            Jr,
            Sr,
            None
        }
        public NameSuffix m_eSuffix = NameSuffix.None;

        public Name()
        {
            m_xFirstName = new FirstName();
            m_xLastName = new LastName();
            m_bShowInitial = false;
            m_cMiddleInitial = 'A';
            m_eSuffix = NameSuffix.None;
        }

        public string GetString(bool bIsPresident = false)
        {
            string xSuffix = "";
            switch (m_eSuffix)
            {
                case NameSuffix.None:
                    xSuffix = "";
                    break;
                case NameSuffix.Jr:
                    xSuffix = " Jr";
                    break;
                case NameSuffix.Sr:
                    xSuffix = " Sr";
                    break;
            }
            string xPresName = string.Format("President {0}", m_xFirstName.m_xName[0]);

            return string.Format("{0} {1}{2}{3}", bIsPresident ? "Pres." : m_xFirstName.m_xName,
                m_bShowInitial ? m_cMiddleInitial + " " : "", m_xLastName.m_xName, xSuffix);
        }
    }

    [System.Serializable]
    public class Scandal
    {
        [SerializeField]
        string m_xText;

        public string GetScandalText(Name m_xName)
        {
            bool bIsMale = m_xName.m_xFirstName.m_xGender == Gender.MALE;
            return string.Format(m_xText, m_xName.m_xLastName.m_xName, bIsMale ? "he " : "she ", bIsMale ? "his " : "her ");
        }
    }
}
