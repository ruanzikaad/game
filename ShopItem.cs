using UnityEngine;

public class ShopItem
{
    public string Name;
    public int Cost;
    public int MinimumLevel;
    public int XpReward;  // Adiciona recompensa de XP como propriedade da classe base
    public GameObject Prefab;  // Adiciona um GameObject que será o prefab
    public Vector3 SpawnOffset;  // Define um offset para a posição de spawn do prefab

    // Atualizando o construtor para receber os parâmetros do prefab e spawn offset
    public ShopItem(string name, int cost, int minimumLevel, int xpReward, GameObject prefab, Vector3 spawnOffset)
    {
        Name = name;
        Cost = cost;
        MinimumLevel = minimumLevel;
        XpReward = xpReward;
        Prefab = prefab;  // Inicializa o prefab
        SpawnOffset = spawnOffset;  // Inicializa o spawn offset
    }

    public virtual bool Purchase(int userId)
    {
        // Implementação básica pode retornar true ou fazer verificações padrão
        return true;
    }
}
