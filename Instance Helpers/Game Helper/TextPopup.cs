using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Darkan.GameHelper
{
    public class TextPopup : MonoBehaviour
    {
        TextMeshPro _textMesh;
        Sequence _popupSequence;
        Tweener _yPositionTweener;
        Tweener _fadeInTweener;
        Tweener _fadeOutTweener;

        void Awake()
        {
            _textMesh = GetComponent<TextMeshPro>();
            _textMesh.alpha = 0;
            _popupSequence = DOTween.Sequence();
            _yPositionTweener = transform.DOMoveY(transform.position.y + 1, 1).SetAutoKill(false).Pause();
            _fadeInTweener = DOTween.To(() => _textMesh.alpha, x => _textMesh.alpha = x, 1, .35f).SetAutoKill(false).Pause();
            _fadeOutTweener = DOTween.To(() => _textMesh.alpha, x => _textMesh.alpha = x, 0, .35f)
                .SetAutoKill(false)
                .Pause();

            _popupSequence.Append(_yPositionTweener)
                .Insert(0, _fadeInTweener)
                .Insert(.65f, _fadeOutTweener)
                .SetAutoKill(false)
                .Pause()
                .SetEase(Ease.OutSine);
        }

        public void Popup(string text, Color color, Stack<TextPopup> pool, float duration = 1)
        {
            _textMesh.text = text;
            _textMesh.color = color;
            _textMesh.alpha = 0;

            _popupSequence.timeScale = 1 / duration;
            _popupSequence.OnComplete(() => { pool.Push(this); gameObject.SetActive(false); });

            _popupSequence.Restart();
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
