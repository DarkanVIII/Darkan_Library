namespace Darkan.RuntimeTools
{
    using DG.Tweening;
    using System;
    using TMPro;
    using UnityEngine;

    public class TextPopup : MonoBehaviour
    {
        public void Init(Action<TextPopup> onComplete, Transform origin, Transform lookAtTarget)
        {
            _onComplete = onComplete;
            _origin = origin;
            _lookAtTarget = lookAtTarget;

            Vector3 startPosition = _origin.position;
            _lastStartPos = startPosition;
            _yPositionTweener = DOTween.To(() => startPosition, x => transform.position = x,
                 startPosition + _currPopupParams.Distance, _currPopupParams.Duration)
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
                .OnComplete(() => _onComplete.Invoke(this));
        }

        public void Init(Action<TextPopup> onComplete, Transform lookAtTarget)
        {
            _onComplete = onComplete;
            _lookAtTarget = lookAtTarget;

            Vector3 startPosition = Vector3.zero;
            _lastStartPos = startPosition;
            _yPositionTweener = DOTween.To(() => startPosition, x => transform.position = x,
                 startPosition + _currPopupParams.Distance, _currPopupParams.Duration)
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
                .OnComplete(() => _onComplete.Invoke(this));
        }

        static readonly Vector3 ROTATE_Y_180 = new(0, 180, 0);

        TextMeshPro _textMesh;
        Tweener _yPositionTweener;
        Tweener _fadeInTweener;
        Tweener _fadeOutTweener;
        new Transform transform;
        Transform _origin;
        Vector3 _lastStartPos;

        TextPopupParams _currPopupParams = TextPopupParams.BasicWhite;

        Action<TextPopup> _onComplete;
        Transform _lookAtTarget;

        void Awake()
        {
            transform = GetComponent<Transform>();
            _textMesh = GetComponent<TextMeshPro>();
        }

        void Update()
        {
            if (_lookAtTarget == null) return;

            transform.LookAt(_lookAtTarget.position);
            transform.Rotate(ROTATE_Y_180);
        }

        public void Play()
        {
            _yPositionTweener.Restart();
            _fadeOutTweener.Restart();
            _fadeInTweener.Restart();
        }

        public TextPopup ChangePopupParams(TextPopupParams popupParams)
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
            || _lastStartPos != _origin.position)
            {
                Vector3 startPos = _origin.position;
                _yPositionTweener.ChangeValues(startPos, startPos + popupParams.Distance, popupParams.Duration);
                _fadeOutTweener.SetDelay(popupParams.Duration - popupParams.FadeTime);
            }

            _currPopupParams = popupParams;
            _lastStartPos = _origin.position;

            return this;
        }

        void OnDestroy()
        {
            _yPositionTweener.Kill();
            _fadeInTweener.Kill();
            _fadeOutTweener.Kill();
        }
    }
}
