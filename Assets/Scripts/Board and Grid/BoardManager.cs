
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>();
	public GameObject tile, tileMover;
	public int xSize, ySize;
	
	public Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	public Color adjacentColor = new Color(.5f, .5f, .5f, 1.0f);

	public GameObject[,] tiles;


	public bool IsShifting { get; set; }

	void Start ()
	{
		instance = GetComponent<BoardManager>();
		GameManager.instance.gameOver = false;
		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
		CreateBoard(offset.x, offset.y);
	}

    
	private void CreateBoard (float xOffset, float yOffset)
	{
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[ySize];
		Sprite previousBelow = null;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x) + (xOffset/2), startY + (yOffset * y) + (yOffset/2), 0), tile.transform.rotation);
				newTile.GetComponent<Tile>().x = x;
				newTile.GetComponent<Tile>().y = y;
				tiles[x, y] = newTile;

				newTile.transform.parent = transform;

				List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(characters);
				possibleCharacters.Remove(previousLeft[y]); 
				possibleCharacters.Remove(previousBelow);

				
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];

				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

				previousLeft[y] = newSprite;
				previousBelow = newSprite;

			}
		}
    }
	
	public IEnumerator FindNullTiles()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}

		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}

	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .3f)
	{
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++)
		{  
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{ 
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++)
		{
			GUIManager.instance.Score += GUIManager.instance.scorePerMatch;
			yield return new WaitForSeconds(shiftDelay);

			if (renders.Count == 1)
			{
				renders[0].sprite = GetNewSprite(x, ySize - 1);
			}

			for (int k = 0; k < renders.Count - 1; k++)
			{ 
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, ySize - 1);

				//movimiento
				/*
				GameObject origen = renders[k + 1].gameObject;
				GameObject destino = renders[k].gameObject;
				GameObject newTile = Instantiate(origen, origen.transform.position, origen.transform.rotation);
				newTile.transform.DOMove(destino.transform.position, 0.2f).OnComplete(() => { Destroy(newTile); });
				*/
			}
		}
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0)
		{
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xSize - 1)
		{
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}



	public void RemoveColor()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				tiles[x,y].gameObject.GetComponent<SpriteRenderer>().material.DOKill();
				tiles[x,y].gameObject.GetComponent<SpriteRenderer>().material.DOColor(Color.white, 1f);

			}
		}
	}


}