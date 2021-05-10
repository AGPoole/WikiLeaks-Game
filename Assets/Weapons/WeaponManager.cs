using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    static WeaponManager s_xInstance;

    IWeapon m_xSelected;
    [SerializeField]
    List<GameObject> m_xWeapons;

    Dictionary<KeyCode, int> m_xKeyValuePairs;
    // Start is called before the first frame update
    void Start()
    {
        s_xInstance = this;

        //Register Keycodes to match each function to call
        const int alphaStart = 49;
        const int alphaEnd = 57;
        m_xKeyValuePairs = new Dictionary<KeyCode, int>();
        for (int i = alphaStart; i <= alphaEnd; i++)
        {
            m_xKeyValuePairs.Add((KeyCode)i, i-alphaStart);
        }
    }

    void Update()
    {
        //Loop through the Dictionary and check if the Registered Keycode is pressed
        foreach (KeyValuePair<KeyCode, int> entry in m_xKeyValuePairs)
        {
            //Check if the keycode is pressed
            if (Input.GetKeyDown(entry.Key) && m_xWeapons.Count>entry.Value && m_xWeapons[entry.Value]!=null)
            {
                SetSelectedWeapon(m_xWeapons[entry.Value]);
            }
        }
    }

    public static WeaponManager GetWeaponManager()
    {
        return s_xInstance;
    }

    public IWeapon GetSelectedWeapon()
    {
        return m_xSelected;
    }

    public void SetSelectedWeapon(GameObject xWeapon)
    {
        if (!m_xWeapons.Contains(xWeapon))
        {
            Debug.LogError("Attempting to select a weapon you do not own");
        }
        m_xSelected = xWeapon.GetComponent<IWeapon>();
    }

    public GameObject AddWeapon(GameObject xWeapon, SystemBase xOwner=null)
    {
        GameObject xNew = Instantiate(xWeapon, transform);
        m_xWeapons.Add(xNew);
        xNew.GetComponent<IWeapon>().SetOwner(xOwner);
        return xNew;
    }

    public void RemoveAndDestroyWeapon(GameObject xWeapon)
    {
        if (m_xSelected == xWeapon.GetComponent<IWeapon>())
        {
            m_xSelected = null;
        }
        m_xWeapons.Remove(xWeapon);
        Destroy(xWeapon);
    }
}
