namespace Darkan.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class SmoothBarText : MonoBehaviour
    {
        [SerializeField] Image _fill;
        [SerializeField] Image _fillBetween;
        [SerializeField] TextMeshProUGUI _textMesh;
        [Min(0.1f)] public float SmoothSpeed = 1;

        public Image Fill => _fill;
        public Image FillBetween => _fillBetween;

        float _targetValue;
        bool _swapped;

        void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (_targetValue == _fill.fillAmount) return;

            _fill.fillAmount = Mathf.MoveTowards(_fill.fillAmount, _targetValue, SmoothSpeed * 0.1f * Time.deltaTime);
        }

        /// <summary>
        /// Updates the visuals of the bar with a smooth transition. value / maxValue. Clamps between 0 and 1.
        /// 0 is an empty, 1 a full Bar.
        /// </summary>
        public void SetBarSmooth(float value, float maxValue)
        {
            float ratio = value / maxValue;

            _targetValue = ratio;

            if (_targetValue < _fill.fillAmount && !_swapped)
                SwapImages();
            if (_targetValue > _fill.fillAmount && _swapped)
                SwapImages();

            _fillBetween.fillAmount = ratio;
            _textMesh.text = GetBarText(value, maxValue);
        }

        /// <summary>
        /// Updates the visuals of the bar instantly and without transition. value / maxValue. Clamps between 0 and 1.
        /// 0 is an empty, 1 a full Bar.
        /// </summary>
        public void SetBarHard(float value, float maxValue)
        {
            float ratio = value / maxValue;

            _targetValue = ratio;
            _fillBetween.fillAmount = ratio;
            _fill.fillAmount = ratio;
            _textMesh.text = GetBarText(value, maxValue);
        }

        protected virtual string GetBarText(float value, float maxValue)
        {
            return $"{value}/{maxValue}";
        }

        void SwapImages()
        {
            (_fill, _fillBetween) = (_fillBetween, _fill);
            _swapped = !_swapped;
        }
    }
}
