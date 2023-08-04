using DG.Tweening;
using Editors;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private LevelManager levelManager;
        
        public GameObject winPanel;
        public TMP_Text moneyText;
        [SerializeField] private GameObject startButton, bulletButton;

        // Start is called before the first frame update
        private void Start()
        {
            levelManager = LevelManager.Instance;
        }

        public void StartGame()
        {
            startButton.SetActive(false);
            bulletButton.SetActive(false);
            Actions.LevelStart?.Invoke();
        }

        public void OpenWinPanel(int money)
        {
            levelManager.dataBase.money += money;
            moneyText.text = "$" + money;
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, 0.3f);
        }

        public void AddBullet()
        {
            BulletEditor.AddBullet();
            SetMoneyText(-levelManager.levelEditor.bulletPrice);
        }

        private void SetMoneyText(int money)
        {
            moneyText.text = money.ToString();
        }
    }
}
