using UnityEngine;

public class CacauSeedling : ShopItem
{
    public int ConstrucaoTempo;

    // Atualizando o construtor para incluir todos os parâmetros necessários
    public CacauSeedling(string name, int cost, int minimumLevel, int xpReward, int construcaoTempo, GameObject prefab, Vector3 spawnOffset)
        : base(name, cost, minimumLevel, xpReward, prefab, spawnOffset)
    {
        XpReward = xpReward;
        ConstrucaoTempo = construcaoTempo;
    }

    public override bool Purchase(int userId)
    {
        // Implementação específica para a Muda de Cacau
        // Pode incluir validação adicional, log de compra, atualização de banco de dados, etc.
        return base.Purchase(userId); // Chama a implementação base se necessário
    }
}
