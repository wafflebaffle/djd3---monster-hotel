using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string triggerEnterName = "Expand";
    [SerializeField] private string triggerExitName = "Shrink";
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData a)
    {
        _anim.SetTrigger(triggerEnterName);
    }

    public void OnPointerExit(PointerEventData a)
    {
        _anim.SetTrigger(triggerExitName);
    }
}
