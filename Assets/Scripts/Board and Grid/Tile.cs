
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Tile : MonoBehaviour
{
	public int x,y;

	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	private bool matchFound = false;

	void Awake()
	{
		render = GetComponent<SpriteRenderer>();
	}

	private void Select()
	{
		isSelected = true;
		render.color = BoardManager.instance.selectedColor;
		List<GameObject> adjacents = GetAllAdjacentTiles();
		foreach (GameObject ob in adjacents)
		{
			if (ob != null)
            {
				ob.GetComponent<SpriteRenderer>().material.DOColor(BoardManager.instance.adjacentColor, 2f);
            }

		}

		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect()
	{
		isSelected = false;
		render.color = Color.white;
		BoardManager.instance.RemoveColor();
		previousSelected = null;
	}

	void OnMouseDown()
	{
		if (GameManager.instance.gameOver == true) return;

		if (render.sprite == null || BoardManager.instance.IsShifting)
		{
			return;
		}

		if (isSelected)
		{
			Deselect();
		}
		else
		{
			if (previousSelected == null)
			{
				Select();
			}

			else
			{
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
				{
					SwapSprite(previousSelected.render);
					//Swap(previousSelected, this.GetComponent<Tile>());

					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
					GUIManager.instance.MoveCounter--;

				}
				else
				{
					previousSelected.GetComponent<Tile>().Deselect();
					Select();
				}
			}
		}


	}

	public void SwapSprite(SpriteRenderer previousSelectedRender)
	{
		if (render.sprite == previousSelectedRender.sprite)
		{
			return;
		}

		//efecto de movimiento
		GameObject newTile = Instantiate(
			BoardManager.instance.tileMover,
			previousSelectedRender.transform.position,
			previousSelectedRender.transform.rotation);
		newTile.transform.DOMove(render.transform.position,0.2f).OnComplete(() => { Destroy(newTile); });

		Sprite tempSprite = previousSelectedRender.sprite;
		previousSelectedRender.sprite = render.sprite;
		render.sprite = tempSprite;

		SFXManager.instance.PlaySFX(Clip.Swap);
	}

	//intento fallido porque pilla las casillas por el raycast y aun no estan colocadas
	public void Swap(Tile origen, Tile destino) {
		Tile tmp = destino;

		BoardManager.instance.tiles[destino.x, destino.y] = BoardManager.instance.tiles[origen.x, origen.y];
		BoardManager.instance.tiles[origen.x, origen.y] = BoardManager.instance.tiles[tmp.x, tmp.y];

		origen.transform.DOMove(destino.transform.position, 1f);
		destino.transform.DOMove(origen.transform.position, 1f);
	}

	private GameObject GetAdjacent(Vector2 castDir)
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		if (hit.collider != null)
		{
			return hit.collider.gameObject;
		}
		return null;
	}

	private List<GameObject> GetAllAdjacentTiles()
	{
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++)
		{
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
		{
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
		}
		return matchingTiles;
	}

    private void ClearMatch(Vector2[] paths)
	{
		List<GameObject> matchingTiles = new List<GameObject>();
		for (int i = 0; i < paths.Length; i++)
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2)
		{
			for (int i = 0; i < matchingTiles.Count; i++)
			{
				//movimiento
				GameObject origen = matchingTiles[i].gameObject;
				GameObject newTile = Instantiate(origen, origen.transform.position, origen.transform.rotation);
				//newTile.transform.DOShakeScale(0.4f,0.5f,5).OnComplete(() => { Destroy(newTile); });
				newTile.transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f),0.4f).OnComplete(() => { Destroy(newTile); });

				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;

			}
			matchFound = true;
		}
	}

	public void ClearAllMatches()
	{
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound)
		{
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.instance.FindNullTiles());
			StartCoroutine(BoardManager.instance.FindNullTiles());
			GUIManager.instance.MoveCounter += GUIManager.instance.moveCounterBonus;
			SFXManager.instance.PlaySFX(Clip.Clear);
		}
	}

	public IEnumerator Sleep(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

}