using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PerkUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    UnityEngine.UI.Text m_xNameText;
    [SerializeField]
    UnityEngine.UI.Text m_xDescriptionText;
    [SerializeField]
    GameObject m_xDescriptionContainer;
    [SerializeField]
    UnityEngine.UI.Image m_xImage;

    PerkBase m_xPerk;

    public static GameObject CreatePerkUI(GameObject xPerkGameObject, SystemBase xOwner, Transform xParent)
    {
        GameObject xUIGameObject =Instantiate(Manager.GetManager().GetUIPrefab(), xParent);
        xUIGameObject.GetComponent<PerkUI>().SetUpPerk(xPerkGameObject, xOwner);
        return xUIGameObject;
    }

    void SetUpPerk(GameObject xPerkObject, SystemBase xOwner)
    {
        if (m_xPerk != null)
        {
            Destroy(xPerkObject);
        }
        m_xPerk = Instantiate(xPerkObject, transform).GetComponent<PerkBase>();

        m_xPerk.SetSystemOwner(xOwner);
        var axButtons = m_xPerk.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (var xButton in axButtons)
        {
            var xNav = xButton.navigation;
            xNav.mode = Navigation.Mode.None;
            xButton.navigation = xNav;
        }
        m_xDescriptionText.text = m_xPerk.GetDescription();
        m_xImage.sprite = m_xPerk.GetIcon();
        m_xNameText.text = m_xPerk.GetName();
        m_xPerk.SetUI(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_xDescriptionContainer.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_xDescriptionContainer.SetActive(false);
    }

    public void UpdateActive(bool bHacked)
    {
        gameObject.SetActive(!m_xPerk.GetRequiresHack() || bHacked);
    }

    public void SetSprite(Sprite xSprite)
    {
        m_xImage.sprite = xSprite;
    }

    public PerkBase GetPerk()
    {
        return m_xPerk;
    }

    public void OnClick()
    {
        m_xPerk.OnClick();
    }
}
