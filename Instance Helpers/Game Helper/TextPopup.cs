using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Darkan.GameHelper
{
    public class TextPopup : MonoBehaviour, IPooled<TextPopup>
    {
        TextMeshPro _textMesh;
        Sequence _popupSequence;
        Tweener _yPositionTweener;
        Tweener _fadeInTweener;
        Tweener _fadeOutTweener;

        float _lastDistance;
        float _lastDuration;
        float _lastFadeTime;
        Vector3 _lastWorldpos;

        public event Action<TextPopup> OnReturnToPool;

        void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();

            _yPositionTweener = DOTween.To(() => transform.position, x => transform.position = x, Vector3.zero, .35f)
                 .SetAutoKill(false)
                 .Pause()
                 .SetEase(Ease.OutCubic);
            _fadeInTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 1, .35f)
                .SetAutoKill(false)
                .Pause()
                .SetEase(Ease.InCubic);
            _fadeOutTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 0, .35f)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => OnReturnToPool(this));
        }

        public void PlayPopup(string text, Color color, Vector3 worldPos, int fontSize, float distance = 1, float duration = 1, float fadeTime = .35f)
        {
            _textMesh.text = text;
            _textMesh.color = color;
            _textMesh.fontSize = fontSize;

            if (_lastDistance != distance || _lastDuration != duration || _lastWorldpos != worldPos)
                _yPositionTweener.ChangeValues(worldPos, worldPos + new Vector3(0, distance, 0), duration);

            if (_lastFadeTime != fadeTime)
            {
                _fadeInTweener.ChangeEndValue(1f, fadeTime);
                _fadeOutTweener.ChangeEndValue(0f, fadeTime);
            }

            _yPositionTweener.Restart();
            _fadeInTweener.Restart();
            _fadeOutTweener.Restart(true, duration - fadeTime);

            _lastDistance = distance;
            _lastDuration = duration;
            _lastFadeTime = fadeTime;
            _lastWorldpos = worldPos;
        }

        void OnDestroy()
        {
            _popupSequence.Kill();
            _yPositionTweener.Kill();
            _fadeInTweener.Kill();
            _fadeOutTweener.Kill();
        }
    }
}
