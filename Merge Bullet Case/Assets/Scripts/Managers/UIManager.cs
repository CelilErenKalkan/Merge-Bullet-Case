using DG.Tweening;
using Editors;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private LevelManager levelManager;
        
        public GameObject winPanel;
        public TMP_Text moneyText, gainedMoneyText;
        [SerializeField] private GameObject startButton, bulletButton;

        // Start is called before the first frame update
        private void Start()
        {
            levelManager = LevelManager.Instance;
            SetMoneyText(levelManager.dataBase.money);
            if (levelManager.dataBase.bulletSaves.Count <= 0)
                startButton.SetActive(false);
        }
        
        
        private void OnEnable()
        {
            Actions.LevelEnd += OpenWinPanel;
        }
        
        private void OnDisable()
        {
            Actions.LevelEnd -= OpenWinPanel;
        }

        public void StartGame()
        {
            startButton.SetActive(false);
            bulletButton.SetActive(false);
            SetMoneyText(levelManager.dataBase.money);
            Actions.LevelStart?.Invoke();
        }
        
        public void RestartGame()
        {
            Actions.ButtonTapped?.Invoke();
            SceneManager.LoadScene(0);
        }

        private void OpenWinPanel()
        {
            levelManager.dataBase.money += levelManager.collectedMoney;
            gainedMoneyText.text = "$" + levelManager.collectedMoney;
            levelManager.SaveSystem();
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, 0.3f);
        }

        public void AddBullet()
        {
            Actions.ButtonTapped?.Invoke();
            BulletEditor.AddBullet();
            SetMoneyText(levelManager.dataBase.money);
            startButton.SetActive(true);
        }

        private void SetMoneyText(int money)
        {
            moneyText.text = "$" + money;
        }
    }
}
