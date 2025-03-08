using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coins = 1000;
    public int level = 1;
    public int xp = 0;

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.SetInt("Level", level);
        PlayerPrefs.SetInt("XP", xp);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        coins = PlayerPrefs.GetInt("Coins", 1000);
        level = PlayerPrefs.GetInt("Level", 1);
        xp = PlayerPrefs.GetInt("XP", 0);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveProgress();
    }

    public void AddXP(int amount)
    {
        xp += amount;
        if (xp >= 100) // Exemplo: sobe de nível a cada 100 XP
        {
            level++;
            xp = 0;
        }
        SaveProgress();
    }
}
