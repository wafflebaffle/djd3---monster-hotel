using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    [SerializeField] private string animationEnterName = "Expand";
    [SerializeField] private string animationExitName = "Shrink";
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData a)
    {
        _anim.SetTrigger(animationEnterName);
    }

    public void OnPointerExit(PointerEventData a)
    {
        _anim.SetTrigger(animationExitName);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _anim.SetTrigger(animationExitName);
    }
}
