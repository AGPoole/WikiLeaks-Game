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

    ActionBase m_xAction;

    public static GameObject CreatePerkUI(GameObject xActionGameObject, SystemBase xOwner, Transform xParent)
    {
        GameObject xUIGameObject =Instantiate(Manager.GetManager().GetUIPrefab(), xParent);
        xUIGameObject.GetComponent<PerkUI>().SetUpAction(xActionGameObject, xOwner);
        return xUIGameObject;
    }

    void SetUpAction(GameObject xActionObject, SystemBase xOwner)
    {
        if (m_xAction != null)
        {
            Destroy(xActionObject);
        }
        m_xAction = Instantiate(xActionObject, transform).GetComponent<ActionBase>();

        m_xAction.SetSystemOwner(xOwner);
        var axButtons = m_xAction.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (var xButton in axButtons)
        {
            var xNav = xButton.navigation;
            xNav.mode = Navigation.Mode.None;
            xButton.navigation = xNav;
        }
        m_xDescriptionText.text = m_xAction.GetDescription();
        m_xImage.sprite = m_xAction.GetIcon();
        m_xNameText.text = m_xAction.GetName();
        m_xAction.SetUI(this);
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
        gameObject.SetActive(!m_xAction.GetRequiresHack() || bHacked);
    }

    public void SetSprite(Sprite xSprite)
    {
        m_xImage.sprite = xSprite;
    }

    public ActionBase GetAction()
    {
        return m_xAction;
    }

    public void OnClick()
    {
        m_xAction.OnClick();
    }
}
