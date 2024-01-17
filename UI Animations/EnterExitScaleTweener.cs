using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Darkan.UI.Animations
{

    public class EnterExitScaleTweener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        Vector3 _scale = new(1.15f, 1.15f, 1.15f);

        [SerializeField]
        Ease _enterEase = Ease.OutElastic;

        [SerializeField]
        Ease _exitEase = Ease.InElastic;

        [SerializeField]
        float _duration = .4f;

        [SerializeField]
        float _overshoot = 1.7f;

        [SerializeField]
        float _delay = 0;

        Tweener _tweener;

        void Awake()
        {
            _tweener = transform.DOScale(_scale, _duration)
                .SetAutoKill(false)
                .Pause();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayEnterAnimation();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PlayExitAnimation();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            PlayExitAnimation();
        }

        public void OnSelect(BaseEventData eventData)
        {
            PlayEnterAnimation();
        }

        void PlayEnterAnimation()
        {
            _tweener.SetEase(_enterEase, _overshoot, _delay);
            _tweener.Restart();
        }
        void PlayExitAnimation()
        {
            _tweener.SetEase(_exitEase, _overshoot, _delay);
            _tweener.PlayBackwards();
        }

        void OnDestroy()
        {
            _tweener.Kill();
        }
    }
}
