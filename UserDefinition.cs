using UnityEngine;
using TMPro;

namespace MyGame.Models
{
    public class UserManager : MonoBehaviour
    {
        public static UserManager Instance { get; private set; }

        public UserData CurrentUser { get; private set; }

        // Referências para os objetos TMP_Text no Unity
        public TMP_Text coinsText;
        public TMP_Text xpText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void InitializeUser(UserData userData)
        {
            CurrentUser = userData;
            UpdateDisplay(); // Atualiza o display quando o usuário é inicializado
        }

        public void UpdateCoinsAndXP(int coins, int xp)
        {
            if (CurrentUser != null)
            {
                CurrentUser.coins += coins;
                CurrentUser.xp += xp;
                UpdateDisplay(); // Atualiza os displays de moedas e XP
            }
        }

        private void UpdateDisplay()
        {
            if (coinsText != null && xpText != null)
            {
                coinsText.text = CurrentUser.coins.ToString();
                xpText.text = CurrentUser.xp.ToString();
            }
        }
    }

    [System.Serializable]
    public class UserData
    {
        public string farm_name;
        public float cash;
        public float coins;
        public int xp;
        public int level;
    }

     [System.Serializable]
    public class UserResponse
    {
        public string status;
        public string message;
        public UserData data;
        public  string level;
        public string coins;
        public string xp;
    }

}

