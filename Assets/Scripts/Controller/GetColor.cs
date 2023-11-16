using UnityEngine;
public class GetColor : MonoBehaviour
{
    public enum Cor { Red, Blue, Yellow }
    public static Cor corAtual;
    public static string nomeCorAtual;
    public static void SortearEObterNomeCor()
    {
        // Sorteia uma cor
        corAtual = GetRandomEnum<Cor>();
        // Obtém o nome da cor usando o switch
        nomeCorAtual = NomedaCor(corAtual);
    }
    private static T GetRandomEnum<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        T randomValue = (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return randomValue;
    }
    public static string NomedaCor(Cor cor)
    {
        switch (cor)
        {
            case Cor.Blue:
                return "Blue";
            case Cor.Red:
                return "Red";
            case Cor.Yellow:
                return "Yellow";
            default:
                return "Default";
        }
    }
}
