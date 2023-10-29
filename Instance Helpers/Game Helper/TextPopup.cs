using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace Darkan.GameHelper
{
    public class TextPopup : MonoBehaviour, IPoolable<TextPopup>
    {
        TextMeshPro _textMesh;
        Tweener _yPositionTweener;
        Tweener _fadeInTweener;
        Tweener _fadeOutTweener;

        float _lastDistance;
        float _lastDuration;
        float _lastFadeTime;
        Vector3 _lastWorldpos;
        new Transform transform;

        static readonly Vector3 ROTATION = new(0, 180, 0);

        public event Action<TextPopup> OnReturnToPool;

        void Awake()
        {
            transform = GetComponent<Transform>();
            _textMesh = GetComponent<TextMeshPro>();

            _yPositionTweener = DOTween.To(() => transform.position, x => transform.position = x, Vector3.zero, .35f)
                 .SetAutoKill(false)
                 .Pause()
                 .SetEase(Ease.OutCubic);
            _fadeOutTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 0, .35f)
                .SetAutoKill(false)
                .Pause()
                .OnComplete(() => OnReturnToPool(this));
            _fadeInTweener = DOTween.To(() => 0f, x => _textMesh.alpha = x, 1, .35f)
                .SetAutoKill(false)
                .Pause()
                .SetEase(Ease.InCubic);
        }

        void Update()
        {
            transform.LookAt(GameHelper.MainCamera.transform.position);
            transform.Rotate(ROTATION);
        }

        public void PlayPopup(string text, Color color, Vector3 worldPos, int fontSize = 8, float distance = .3f, float duration = 1.5f, float fadeTime = .35f)
        {
            transform.position = worldPos;
            _textMesh.text = text;
            _textMesh.color = color;
            _textMesh.fontSize = fontSize;

            if (_lastDistance != distance || _lastDuration != duration || _lastWorldpos != worldPos)
                _yPositionTweener.ChangeValues(worldPos, worldPos + new Vector3(0, distance, 0), duration);

            if (_lastFadeTime != fadeTime)
            {
                _fadeOutTweener.ChangeValues(1f, 0f, fadeTime);
                _fadeInTweener.ChangeValues(0f, 1f, fadeTime);
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
            _yPositionTweener.Kill();
            _fadeInTweener.Kill();
            _fadeOutTweener.Kill();
        }
    }
}
