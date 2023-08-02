using DG.Tweening;
using Gameplay;
using TMPro;
using UnityEngine;
using Utils;
using Grid = Utils.Grid;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        private GameManager gameManager;
        private LevelManager levelManager;
        
        public GameObject winPanel;
        public TMP_Text moneyText;

        // Start is called before the first frame update
        private void Start()
        {
            gameManager = GameManager.Instance;
            levelManager = LevelManager.Instance;
        }

        public void StartGame()
        {
            levelManager.StartLevel();
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
            levelManager.AddBullet();
            //SetMoneyText(-levelManager.levelEditor.bulletPrice);
        }
    }
}
