namespace Darkan.InstanceHelpers
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Pool;

    public class TextPopup : MonoBehaviour
    {
        public void Init(IObjectPool<TextPopup> objectPool)
        {
            _objectPool = objectPool;
        }

        static readonly Vector3 ROTATE_Y_180 = new(0, 180, 0);

        public TextPopupParams PopupParams => _currPopupParams;

        TextMeshPro _textMesh;
        Tweener _yPositionTweener;
        Tweener _fadeInTweener;
        Tweener _fadeOutTweener;
        new Transform transform;

        TextPopupParams _currPopupParams = TextPopupParams.BasicWhite;

        IObjectPool<TextPopup> _objectPool;

        void Awake()
        {
            transform = GetComponent<Transform>();
            _textMesh = GetComponent<TextMeshPro>();

            _yPositionTweener = DOTween.To(() => transform.localPosition, x => transform.localPosition = x,
                _currPopupParams.Offset + _currPopupParams.Distance, _currPopupParams.Duration)
                 .SetAutoKill(false)
                 .Pause()
                 .SetEase(Ease.OutCubic);

            _fadeInTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 1f, _currPopupParams.FadeTime)
                .SetAutoKill(false)
                .Pause()
                .SetEase(Ease.InCubic);

            _fadeOutTweener = DOTween.To(() => 1f, x => _textMesh.alpha = x, 0f, _currPopupParams.FadeTime)
                .SetAutoKill(false)
                .Pause()
                .SetDelay(_currPopupParams.Duration - _currPopupParams.FadeTime)
                .OnComplete(() => _objectPool.Release(this));
        }

        void Update()
        {
            transform.LookAt(GameHelper.MainCamera.transform.position);
            transform.Rotate(ROTATE_Y_180);
        }

        public void PlayPopup()
        {
            _yPositionTweener.Restart();
            _fadeOutTweener.Restart();
            _fadeInTweener.Restart();
        }

        public void ChangePopupParams(TextPopupParams popupParams)
        {
            _textMesh.text = popupParams.Text;
            _textMesh.color = popupParams.Color;
            _textMesh.fontSize = popupParams.FontSize;
            _textMesh.alpha = 0;

            if (_currPopupParams.FadeTime != popupParams.FadeTime)
            {
                _fadeOutTweener.ChangeValues(1f, 0f, popupParams.FadeTime);
                _fadeOutTweener.SetDelay(popupParams.Duration - popupParams.FadeTime);
                _fadeInTweener.ChangeValues(0f, 1f, popupParams.FadeTime);
            }

            if (_currPopupParams.Distance != popupParams.Distance
            || _currPopupParams.Duration != popupParams.Duration
            || _currPopupParams.Offset != popupParams.Offset)
            {
                _yPositionTweener.ChangeValues(popupParams.Offset,
                    popupParams.Offset + popupParams.Distance, popupParams.Duration);
            }

            _currPopupParams = popupParams;
        }

        void OnDestroy()
        {
            _yPositionTweener.Kill();
            _fadeInTweener.Kill();
            _fadeOutTweener.Kill();
        }
    }
}
