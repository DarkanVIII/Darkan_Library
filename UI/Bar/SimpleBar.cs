namespace Darkan.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class SimpleBar : MonoBehaviour
    {
        [SerializeField] Image _fill;

        public Image Fill => _fill;

        float _value;
        float _maxValue;

        void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        public void SetBar(float value, float maxValue)
        {
            _value = value;
            _maxValue = maxValue;
            UpdateBar();
        }

        void UpdateBar()
        {
            _fill.fillAmount = _value / _maxValue;
        }

        public void IncreaseByDeltaTime()
        {
            _value += Time.deltaTime;
            UpdateBar();
        }

        public void DecreaseByDeltaTime()
        {
            _value -= Time.deltaTime;
            UpdateBar();
        }

        public void EmptyBar()
        {
            _fill.fillAmount = 0;
            _value = 0;
        }

        public void FullBar()
        {
            _fill.fillAmount = 1;
            _value = _maxValue;
        }
    }
}