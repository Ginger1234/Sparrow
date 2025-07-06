using UnityEngine;

namespace managers
{
    public class MainGameManager : MonoBehaviour
    {
        public static MainGameManager Instance { get; private set; }

        public PlayerFlyController Player;
        public Menu MainMenu;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate instance
            }
        }

        public void GameFinished(bool won){
            if (won)
            {
                MainMenu.Win();
            }
            else
            {
                MainMenu.OnDeath();
            }
        }

        public void SnapPlayer(Vector3 pos){
            Player.transform.position = pos;
        }
    }
}
