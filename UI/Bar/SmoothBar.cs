namespace Darkan.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SmoothBar : MonoBehaviour
    {
        [SerializeField] Image _fill;
        [SerializeField] Image _fillBetween;
        [Min(0.1f)] public float SmoothSpeed = 1;

        public Image Fill => _fill;
        public Image FillBetween => _fillBetween;

        float _targetValue;
        bool _swapped;

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
        }

        /// <summary>
        /// Sets bar hard by proportion (value between 0 and 1)
        /// </summary>
        public void SetProportion(float value)
        {
            float value01 = Mathf.Clamp01(value);

            _fillBetween.fillAmount = value01;
            _fill.fillAmount = value01;
            _targetValue = value01;
        }

        void SwapImages()
        {
            (_fill, _fillBetween) = (_fillBetween, _fill);
            _swapped = !_swapped;
        }
    }
}
