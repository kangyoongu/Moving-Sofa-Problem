using UnityEngine;
using UnityEngine.InputSystem;

namespace Kang
{
    public class GameManager : MonoBehaviour
    {
        Controls inputs;

        private void Awake()
        {
            inputs = new Controls();

            inputs.Player.Enable();
            inputs.Player.Jump.performed += Jump_performed;
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            AgentManager.Instance.StartEpisode();
        }
    }
}
