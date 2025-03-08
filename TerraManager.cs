using UnityEngine;

public class TerraManager : MonoBehaviour
{
    public bool estaOcupado = false; // Define se já existe algo nesta Terra

    public bool EstaDisponivel()
    {
        return !estaOcupado; // Retorna verdadeiro apenas se a terra estiver livre
    }

    public void OcupaTerreno()
    {
        estaOcupado = true; // Marca a terra como ocupada
    }
}
