using System.Collections.Generic;
using Editors;
using Gameplay;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [HideInInspector] public bool isPlayable;
        public List<Bullet> bullets = new List<Bullet>();

        private void Start()
        {
            BulletEditor.CreateBullets();
        }

        public void SetPlayable(bool isSet)
        {
            isPlayable = isSet;
        }
    }
}
