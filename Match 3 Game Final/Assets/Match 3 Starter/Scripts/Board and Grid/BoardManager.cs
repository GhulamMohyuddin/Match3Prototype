/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
    public Camera mainCamera;
	public List<Sprite> characters = new List<Sprite>();
	public GameObject tile;
	public int xSize, ySize;
    public bool isBoardCreated;

    private GameObject[,] tiles;
    private float scaleTile;

	public bool IsShifting { get; set; }

	void Start () {
		instance = GetComponent<BoardManager>();

        Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        scaleTile = tile.transform.localScale.x;

        Vector3 bottomLeft = new Vector3(0, 0, 0);
        Vector3 blPosition = Camera.main.ViewportToWorldPoint(bottomLeft);
        transform.position = blPosition;

        Vector3 bottomRight = new Vector3(1, 0, 0);
        Vector3 brPosition = Camera.main.ViewportToWorldPoint(bottomRight);


        float xPos = (Mathf.Abs(blPosition.x) + Mathf.Abs(brPosition.x))/xSize;
        scaleTile = (xPos / offset.x) * scaleTile;

        isBoardCreated = false;
        CreateBoard(xPos, offset.y);

    }

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[ySize]; // Add this line
		Sprite previousBelow = null; // Add this line

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                newTile.transform.localScale = new Vector3(scaleTile, scaleTile, scaleTile);
				tiles[x, y] = newTile;
                Tile tileObject = newTile.GetComponent<Tile>();
                tileObject.xPos = x;
                tileObject.yPos = y;
                
				newTile.transform.parent = transform; // Add this line

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

        transform.position = new Vector3(transform.position.x + tiles[0, 0].GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y + tiles[0, 0].GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);
        Invoke("invokeBoardCreation", 2);
    }

    public void SetTile(int x, int y,Tile tileObject)
    {
        tiles[x, y] = tileObject.gameObject;
    }
    void invokeBoardCreation()
    {
        isBoardCreated = true;
    }
    public void PlayAttackerAnimation()
    {
        AnimationManager.instance.PlayAttackerAnimation();
    }

    public void PlayOpponentAnimation()
    {
        AnimationManager.instance.PlayOpponentAnimation();
    }


    public IEnumerator FindNullTiles() {
		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}
	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) {
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < ySize; y++) {
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null) {
				nullCount++;
			}
			renders.Add(render);
		}

        for (int i = 0; i < nullCount; i++) {
			yield return new WaitForSeconds(shiftDelay);

            if (ySize - yStart == 1)
            {
                renders[0].sprite = GetNewSprite(x, ySize - 1); ;
            }
            else
            {
                for (int k = 0; k < renders.Count - 1; k++)
                {
                    renders[k].sprite = renders[k + 1].sprite;
                    renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
                }
            }


		}
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y) {
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(characters);

		if (x > 0) {
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xSize - 1) {
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0) {
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}
       

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}

}
