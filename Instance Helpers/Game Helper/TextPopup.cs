namespace Darkan.GameHelper
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

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

        public event System.Action<TextPopup> OnReturnToPool;

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

        public void PlayPopup(TextPopupParams @params)
        {
            _textMesh.text = @params.Text;
            _textMesh.color = @params.Color;
            _textMesh.fontSize = @params.FontSize;

            if (_lastDistance != @params.Distance || _lastDuration != @params.Duration || _lastWorldpos != @params.WorldPos)
            {
                _yPositionTweener.ChangeValues(@params.WorldPos,
                    @params.WorldPos + new Vector3(0, @params.Distance, 0), @params.Duration);
            }

            if (_lastFadeTime != @params.FadeTime)
            {
                _fadeOutTweener.ChangeValues(1f, 0f, @params.FadeTime);
                _fadeInTweener.ChangeValues(0f, 1f, @params.FadeTime);
            }

            _yPositionTweener.Restart();
            _fadeInTweener.Restart();
            _fadeOutTweener.Restart(true, @params.Duration - @params.FadeTime);

            _lastDistance = @params.Distance;
            _lastDuration = @params.Duration;
            _lastFadeTime = @params.FadeTime;
            _lastWorldpos = @params.WorldPos;
        }

        void OnDestroy()
        {
            _yPositionTweener.Kill();
            _fadeInTweener.Kill();
            _fadeOutTweener.Kill();
        }
    }
}
