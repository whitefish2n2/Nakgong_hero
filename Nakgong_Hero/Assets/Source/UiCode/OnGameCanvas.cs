using UnityEngine;

namespace Source.UiCode
{
    public class OnGameCanvas : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
