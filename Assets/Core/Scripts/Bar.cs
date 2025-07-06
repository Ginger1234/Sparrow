using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using managers;

namespace ui_bars
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private Image bar;
        [SerializeField] private float initialValue = 100f;
        [SerializeField] private float updatePeriod = 5f;
        [SerializeField] private float decreaseAmount = 0.2f;

        private float currentValue;

        public float CurrentValue
        {
            get => currentValue;
            private set
            {
                currentValue = Mathf.Clamp(value, 0f, initialValue);
                UpdateBar();
                if (currentValue <= 0f)
                {
                    HandleZeroValue();
                }
            }
        }

        private void Start()
        {
            CurrentValue = initialValue;
            StartCoroutine(DecreaseOverTime());
        }

        private IEnumerator DecreaseOverTime()
        {
            while (CurrentValue > 0)
            {
                yield return new WaitForSeconds(updatePeriod);
                CurrentValue -= decreaseAmount;
            }
        }

        public void Decrease(float damage)
        {
            CurrentValue -= damage;
        }

        public void Increase(float healing)
        {
            CurrentValue += healing;
        }

        private void UpdateBar()
        {
            bar.fillAmount = CurrentValue / initialValue;
        }

        private void HandleZeroValue()
        {
            MainGameManager.Instance.Player.Death();
            MainGameManager.Instance.GameFinished(false);
        }
    }
}
