using UnityEngine;
using UnityEngine.InputSystem;

namespace Kang
{
    public class GameManager : MonoBehaviour
    {
        Controls inputs;
        //public float timeScale = 1f;

        private void Awake()
        {
            inputs = new Controls();

            inputs.Player.Enable();
            inputs.Player.Jump.performed += Jump_performed;
            Application.targetFrameRate = 120;
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            AgentManager.Instance.StartEpisode();
        }
       /* private void OnValidate()
        {
            Time.timeScale = timeScale;
        }*/
    }
}
