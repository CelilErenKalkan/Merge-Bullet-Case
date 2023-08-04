using Editors;
using UnityEngine;

namespace Gameplay
{
    public class StartBullets : MonoSingleton<StartBullets>
    {
        public bool isMoveForward;
        private const float ForwardSpeed = 10;

        private void Update()
        {
            MoveForward();
        }

        private void MoveForward()
        {
            if(isMoveForward)
                transform.Translate(transform.forward * ForwardSpeed * Time.deltaTime);
        }
    }
}