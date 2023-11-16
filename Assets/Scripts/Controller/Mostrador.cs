using UnityEngine;
using System.Collections;

public class Index : MonoBehaviour
{
	public GameObject Mostrador;
	    public Index mostrador;

	void Awake()
    {
        mostrador.CreateMostrador();
    }
	public void CreateMostrador()
	{
		//nome
		Mostrador = new GameObject("Mostrador");
		//tamanho
		Mostrador.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
		//posição
		Mostrador.transform.position = new Vector3(15f, 5f, 0f);
		//cria o renderizador
		SpriteRenderer spriteRenderer = Mostrador.AddComponent<SpriteRenderer>();
		//carrega sprite
		Sprite bubbleSprite = Resources.Load<Sprite>("Sprites/Bubbles/RedBubbleSprite");
		if (bubbleSprite != null)
		{
			spriteRenderer.sprite = bubbleSprite;
		}
		else
		{
			Debug.LogError("Falha ao carregar a Sprite. Verifique o caminho e o nome do arquivo.");
		}
	}
}