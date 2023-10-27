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
        float _lastduration;

        public event Action<TextPopup> OnReturnToPool;

        void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();

            _yPositionTweener = transform.DOMoveY(0, 2)
            .SetAutoKill(false)
            .Pause()
            .SetEase(Ease.OutCubic);
            _fadeInTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 1, .35f)
            .SetAutoKill(false)
            .Pause()
            .SetEase(Ease.InCubic);
            _fadeOutTweener = DOTween.To(() => 1f, x => _textMesh.alpha = x, 0, .35f)
            .SetAutoKill(false)
            .Pause()
            .OnComplete(() => OnReturnToPool(this));
        }

        public void PlayPopup(string text, Color color, float distance = 1, float duration = 1, float fadeTime = .35f)
        {
            _textMesh.text = text;
            _textMesh.color = color;

            if (_lastDistance != distance || _lastduration != duration)
                _yPositionTweener.ChangeEndValue(new Vector3(0, distance, 0), duration);
            _yPositionTweener.Restart();
            _fadeInTweener.Restart();
            _fadeOutTweener.Restart(true, duration - .35f);

            _lastDistance = distance;
            _lastduration = duration;
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
