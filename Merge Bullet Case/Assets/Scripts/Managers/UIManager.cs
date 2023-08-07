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

        // UI elements
        public GameObject winPanel;
        public TMP_Text moneyText, gainedMoneyText;
        [SerializeField] private GameObject startButton, bulletButton;
        private bool isButtonInCooldown;

        private void Start()
        {
            // Initialize references and UI elements
            levelManager = LevelManager.Instance;
            SetMoneyText(levelManager.dataBase.money);
            
            // Hide startButton if there are no bullets
            if (levelManager.dataBase.bulletDataList.Count <= 0)
                startButton.SetActive(false);
        }

        private void OnEnable()
        {
            // Subscribe to LevelEnd event
            Actions.LevelEnd += OpenWinPanel;
        }

        private void OnDisable()
        {
            // Unsubscribe from LevelEnd event
            Actions.LevelEnd -= OpenWinPanel;
        }

        public void StartGame()
        {
            // Start the game and update UI elements
            startButton.SetActive(false);
            bulletButton.SetActive(false);
            SetMoneyText(levelManager.dataBase.money);
            Actions.LevelStart?.Invoke();
        }

        public void RestartGame()
        {
            // Restart the game
            Actions.ButtonTapped?.Invoke();
            SceneManager.LoadScene(0);
        }

        private void OpenWinPanel()
        {
            // Handle end-of-level actions and show win panel
            levelManager.dataBase.money += levelManager.collectedMoney;
            gainedMoneyText.text = "$" + levelManager.collectedMoney;
            levelManager.SaveSystem();
            winPanel.SetActive(true);
            winPanel.transform.DOScale(Vector3.one, 0.3f);
        }

        public void AddBullet()
        {
            if (isButtonInCooldown) return;
            isButtonInCooldown = true;
            // Add a bullet and update UI elements
            Actions.ButtonTapped?.Invoke();
            BulletEditor.AddBullet();
            SetMoneyText(levelManager.dataBase.money);
            startButton.SetActive(true);
            isButtonInCooldown = false;
        }

        private void SetMoneyText(int money)
        {
            // Update the money text
            moneyText.text = "$" + money;
        }
    }
}