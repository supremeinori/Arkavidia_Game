using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonTextHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.green;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}
