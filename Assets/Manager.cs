using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager s_xInstance;

    float fNextTime = 0.0f;
    [SerializeField]
    [Range(0f, 5f)]
    float m_fTimeGap = 1f;
    [SerializeField]
    bool m_bShouldWriteTaxesToFile = true;

    //TODO: move to PLAYER class
    [SerializeField]
    UnityEngine.UI.Text m_xMoneyText;
    [SerializeField]
    float m_fPlayerMoney;

    // TODO: move to a better place
    [SerializeField]
    GameObject m_xEdgePrefab;
    [SerializeField]
    GameObject m_xLineRendererPrefab;
    [SerializeField]
    float m_fConnectionRange;

    [SerializeField]
    UnityEngine.UI.Text m_xHacksText;

    [SerializeField]
    bool m_bTechIncreaseEnabled = true;
    [SerializeField]
    int m_iTechLevel = 0;
    [SerializeField]
    int m_iTechLevelUpPoints = 0;
    [SerializeField]
    int[] m_aiTechScoreBoundaries;
    [SerializeField]
    UnityEngine.UI.Text m_xTechLevelText;

    [SerializeField]
    Country m_xCountry;

    [SerializeField]
    float m_fHexagonEdgeSize;
    [SerializeField]
    GameObject m_xPerkUIPrefab;

    public enum GridDirection : int
    {
        UP,
        UP_RIGHT,
        DOWN_RIGHT,
        DOWN,
        DOWN_LEFT,
        UP_LEFT
    }

    // Start is called before the first frame update
    //void Awake()
    //{
    //    s_xInstance = this;
    //}

    public static Manager GetManager()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType(typeof(Manager)) as Manager;
        }
        return s_xInstance;
    }

    static bool s_bIsPaused = false;
    int m_iTurnNumber = 0;

    [SerializeField]
    private float dragSpeed = 1f;
    private Vector3 dragOrigin = new Vector3(0f, 0f, 0f);
    [SerializeField]
    Vector2 xMaxCam;
    [SerializeField]
    Vector2 xMinCam;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("-"))
        {
            Camera.main.orthographicSize++;
        }
        else if (Input.GetKeyDown("="))
        {
            Camera.main.orthographicSize--;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePauseButton.GetPauseButton().TogglePause();
        }
        if (!s_bIsPaused && fNextTime < Time.time)
        {
            fNextTime = Time.time + m_fTimeGap;
            m_xCountry.OnNextTurn();
            NotificationSystem.OnNextTurn();
            SystemBase.SetUpEdges();
            if (m_bTechIncreaseEnabled)
            {
                m_iTechLevelUpPoints += 1;
                if (m_iTechLevel < m_aiTechScoreBoundaries.Length && m_iTechLevelUpPoints > m_aiTechScoreBoundaries[m_iTechLevel])
                {
                    m_iTechLevel++;
                    m_iTechLevelUpPoints = 0;
                }
            }

            m_iTurnNumber++;
            if (DebugSettings.ShouldLogTurnEnd())
            {
                UnityEngine.Debug.Log("Done");
            }
        }
        else if (s_bIsPaused)
        {
            fNextTime = Time.time + m_fTimeGap;
        }
        HandleCameraMovement();
        m_xMoneyText.text = "$" + m_fPlayerMoney.ToString("0.00");
        m_xTechLevelText.text = "Tech Level: " + GetTechLevel().ToString();
    }

    void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
        Camera.main.gameObject.transform.Translate(move, Space.World);

        Vector3 xCurrent = Camera.main.gameObject.transform.position;
        Camera.main.gameObject.transform.position = new Vector3(Mathf.Clamp(xCurrent.x, xMinCam.x, xMaxCam.x), Mathf.Clamp(xCurrent.y, xMinCam.y, xMaxCam.y), xCurrent.z);
    }

    public static void TogglePaused()
    {
        s_bIsPaused = !s_bIsPaused;
    }
    public static bool GetIsPaused()
    {
        return s_bIsPaused;
    }

    public static int GetTurnNumber()
    {
        if (s_xInstance == null)
        {
            return 0;
        }
        return s_xInstance.m_iTurnNumber;
    }

    public static bool ShouldWriteTaxesToFile()
    {
        if (s_xInstance == null)
        {
            return false;
        }
        return s_xInstance.m_bShouldWriteTaxesToFile;
    }

    [SerializeField]
    float m_fSOFT_CAP_TAX;
    [SerializeField]
    float m_fHARD_CAP_TAX;
    [SerializeField]
    float m_fSOFT_CAP_CORP;
    [SerializeField]
    float m_fHARD_CAP_CORP;

    // values at which happiness increases in different ranges
    [SerializeField]
    HappinessBound[] m_xGovernmentBounds;
    [SerializeField]
    HappinessBound[] m_xCorporateBounds;

    public static float CalculateHappiness(int iGovLevel, int iCorpLevel, bool bNormalize = true)
    {
        float fReturn = 0;

        s_xInstance = GetManager();

        if (s_xInstance.m_xGovernmentBounds.Length == 0 || s_xInstance.m_xCorporateBounds.Length == 0)
        {
            UnityEngine.Debug.LogError("Empty happiness bounds");
            return 0;
        }

        // TODO: move the verification checks and log errors to Start, so they are only performed once and this function is clearer
        int iBoundsIndex = 0;
        float fPreviousHappiness = s_xInstance.m_xGovernmentBounds[0].fHappinessAfter;
        for (int i = 1; i <= iGovLevel; i++)
        {
            if (iBoundsIndex + 1 < s_xInstance.m_xGovernmentBounds.Length
                && s_xInstance.m_xGovernmentBounds[iBoundsIndex + 1].iBound <= i)
            {
                iBoundsIndex += 1;
                if (s_xInstance.m_xGovernmentBounds[iBoundsIndex].fHappinessAfter > fPreviousHappiness)
                {
                    UnityEngine.Debug.LogError("Government happiness bounds are not decreasing");
                }
            }
            fPreviousHappiness = s_xInstance.m_xGovernmentBounds[iBoundsIndex].fHappinessAfter;
            fReturn += s_xInstance.m_xGovernmentBounds[iBoundsIndex].fHappinessAfter;
        }

        iBoundsIndex = 0;
        fPreviousHappiness = s_xInstance.m_xCorporateBounds[0].fHappinessAfter;
        for (int i = 1; i <= iCorpLevel; i++)
        {
            if (iBoundsIndex + 1 < s_xInstance.m_xCorporateBounds.Length
                && s_xInstance.m_xCorporateBounds[iBoundsIndex + 1].iBound <= i)
            {
                iBoundsIndex += 1;
                if (s_xInstance.m_xCorporateBounds[iBoundsIndex].fHappinessAfter > fPreviousHappiness)
                {
                    UnityEngine.Debug.LogError("Corporate happiness bounds are not decreasing");
                }
            }
            fPreviousHappiness = s_xInstance.m_xCorporateBounds[iBoundsIndex].fHappinessAfter;
            fReturn += s_xInstance.m_xCorporateBounds[iBoundsIndex].fHappinessAfter;
        }

        if (bNormalize)
        {
            if (s_xInstance.m_xGovernmentBounds[s_xInstance.m_xGovernmentBounds.Length - 1].fHappinessAfter != 0
                || s_xInstance.m_xCorporateBounds[s_xInstance.m_xCorporateBounds.Length - 1].fHappinessAfter != 0)
            {
                UnityEngine.Debug.LogError("Maximum happiness does not exist, so cannot calculate happiness");
            }
            int iMaxGovLevel = s_xInstance.m_xGovernmentBounds[s_xInstance.m_xGovernmentBounds.Length - 1].iBound;
            int iMaxCorpLevel = s_xInstance.m_xCorporateBounds[s_xInstance.m_xCorporateBounds.Length - 1].iBound;

            fReturn /= CalculateHappiness(iMaxGovLevel, iMaxCorpLevel, false);
        }
        return fReturn;
    }

    public void ChangeMoney(float fAddition)
    {
        m_fPlayerMoney += fAddition;
        if (m_fPlayerMoney < 0f)
        {
            m_fPlayerMoney = 0f;
        }
    }

    public float GetMoney()
    {
        return m_fPlayerMoney;
    }

    public GameObject GetEdgePrefabGameObject()
    {
        return m_xEdgePrefab;
    }
    public GameObject GetLineRendererPrefabGameObject()
    {
        return m_xLineRendererPrefab;
    }

    public float GetConnectionRange()
    {
        return m_fConnectionRange;
    }

    public int GetTechLevel()
    {
        return m_iTechLevel;
    }

    public int GetNumTechLevels()
    {
        return m_aiTechScoreBoundaries.Length;
    }

    public Vector3 GetPositionFromGridCoords(int iX, int iY)
    {
        float fHexHeight = 0.86602540378f * m_fHexagonEdgeSize * 2;
        float fHexWidth = 2f * m_fHexagonEdgeSize;
        float fOffset = ProjectMaths.Mod(iX, 2) == 0 ? 0 : -fHexHeight / 2;
        return new Vector3(iX * fHexWidth * 0.75f, (iY * fHexHeight) + fOffset, 0f);
    }

    public float GetHexagonEdgeSize()
    {
        return m_fHexagonEdgeSize;
    }

    public static SystemBase GetAdjacentSystem(int iX, int iY, GridDirection eDir)
    {
        switch (eDir)
        {
            case GridDirection.UP:
                {
                    return SystemBase.GetSystemWithCoords(iX, iY + 1);
                }
            case GridDirection.UP_LEFT:
                {
                    if (ProjectMaths.Mod(iX, 2) == 0)
                    {
                        return SystemBase.GetSystemWithCoords(iX - 1, iY + 1);
                    }
                    else
                    {
                        return SystemBase.GetSystemWithCoords(iX - 1, iY);
                    }
                }
            case GridDirection.DOWN_LEFT:
                {
                    if (ProjectMaths.Mod(iX, 2) == 0)
                    {
                        return SystemBase.GetSystemWithCoords(iX - 1, iY);
                    }
                    else
                    {
                        return SystemBase.GetSystemWithCoords(iX - 1, iY - 1);
                    }
                }
            case GridDirection.DOWN:
                {
                    return SystemBase.GetSystemWithCoords(iX, iY - 1);
                }
            case GridDirection.DOWN_RIGHT:
                {
                    if (ProjectMaths.Mod(iX, 2) == 0)
                    {
                        return SystemBase.GetSystemWithCoords(iX + 1, iY);
                    }
                    else
                    {
                        return SystemBase.GetSystemWithCoords(iX + 1, iY - 1);
                    }
                }
            case GridDirection.UP_RIGHT:
                {
                    if (ProjectMaths.Mod(iX, 2) == 0)
                    {
                        return SystemBase.GetSystemWithCoords(iX + 1, iY + 1);
                    }
                    else
                    {
                        return SystemBase.GetSystemWithCoords(iX + 1, iY);
                    }
                }
        }
        return null;
    }

    public static GridDirection GetOppositeDirection(GridDirection eDir)
    {
        switch (eDir)
        {
            case GridDirection.UP:
                return GridDirection.DOWN;
            case GridDirection.UP_RIGHT:
                return GridDirection.DOWN_LEFT;
            case GridDirection.DOWN_RIGHT:
                return GridDirection.UP_LEFT;
            case GridDirection.DOWN:
                return GridDirection.UP;
            case GridDirection.DOWN_LEFT:
                return GridDirection.UP_RIGHT;
            case GridDirection.UP_LEFT:
                return GridDirection.DOWN_RIGHT;
            default:
                UnityEngine.Debug.LogError("Extra Grid Direction case");
                return GridDirection.UP;
        }
    }

    public GameObject GetUIPrefab()
    {
        return m_xPerkUIPrefab;
    }

#if (UNITY_EDITOR)
    [ContextMenu("Correct Positions")]
    void CorrectPositions()
    {
        foreach (OrganisationBase xOrg in FindObjectsOfType<OrganisationBase>())
        {
            xOrg.CorrectPosition();
        }
        foreach (SystemBase xSys in FindObjectsOfType<SystemBase>())
        {
            xSys.CorrectPosition();
        }
    }
#endif
}

[System.Serializable]
class HappinessBound
{
    public int iBound;
    public float fHappinessAfter;
}
