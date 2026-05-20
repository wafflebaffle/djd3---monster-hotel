using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        _anim.Play(animationEnterName);
    }

    public void OnPointerExit(PointerEventData a)
    {
        _anim.Play(animationExitName);
    }
}
