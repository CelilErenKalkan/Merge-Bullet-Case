using System.Collections.Generic;
using Editors;
using Gameplay;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        // Gameplay control
        [HideInInspector] public bool isPlayable;

        // List of bullets
        public List<Bullet> bullets = new List<Bullet>();

        private void Start()
        {
            // Create initial bullets
            BulletEditor.CreateBullets();
        }

        // Set the playability state
        public void SetPlayable(bool isSet)
        {
            isPlayable = isSet;
        }
    }
}
