using UnityEngine;

namespace Gameplay
{
    public class StartBullets : MonoSingleton<StartBullets>
    {
        #region Variables for Movement
        public bool isMoveForward;
        public float forwardSpeed;
        #endregion

        void Start()
        {
        
        }

        void Update()
        {
            MoveForward();
        }

        private void MoveForward()
        {
            if(isMoveForward)
                transform.Translate(transform.forward * forwardSpeed * Time.deltaTime);
        }
    }
}