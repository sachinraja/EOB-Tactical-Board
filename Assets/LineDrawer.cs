using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LineDrawer : MonoBehaviour 
{
	List<GameObject> InstantiatedPictures = new List<GameObject>();
	public List<GameObject> characterList = new List<GameObject>();
	public Sprite characterSprite;
	public GameObject characterPrefab;

	public GameObject Map;

	public Button DefaultColorButton;
	public Slider ThicknessSlider;

	public LineFactory lineFactory;

	private Vector2 start;
	private Line drawnLine;
	private bool placed = false;

	private Color col = Color.black;
	private float width = 0.05f;

	private bool Erase;
	private bool CharacterPlacement;

    private void Awake()
    {
		DefaultColorButton.Select();
		col = DefaultColorButton.GetComponent<Image>().color;
    }

    void Update ()
	{
		//only draw lines on map
		if (Map.GetComponent<Collider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) == true)
		{
			if (Erase == false && CharacterPlacement == false)
			{
				if (Input.GetMouseButtonDown(0) && placed == false)
				{
					//start line drawing
					var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					drawnLine = lineFactory.GetLine(pos, pos, width, col);

					placed = true;
				}

				else if (Input.GetMouseButtonDown(0) && placed == true)
				{
					placed = false;

					//end line drawing
					drawnLine = null;
				}

				if (drawnLine != null)
				{
					//update the line size as the player draws
					drawnLine.end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				}
			}

			else if (Erase == true)
			{
				if (Input.GetMouseButton(0))
				{
					var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

					var activeLines = lineFactory.GetActive();

					foreach (var line in activeLines)
					{
						if (line.gameObject.GetComponent<BoxCollider2D>().OverlapPoint(pos))
						{
							line.gameObject.SetActive(false);
						}
					}

					for (int i = 0; i < InstantiatedPictures.Count; i++)
					{
						if (InstantiatedPictures[i].GetComponent<BoxCollider2D>().OverlapPoint(pos))
						{
							Destroy(InstantiatedPictures[i]);
							InstantiatedPictures.RemoveAt(i);
							
						}
					}
				}
			}

			else if (CharacterPlacement == true)
			{
				if (Input.GetMouseButtonDown(0))
				{
					var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

					GameObject c = Instantiate(characterPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
					c.GetComponent<SpriteRenderer>().sprite = characterSprite;
					InstantiatedPictures.Add(c);
				}
			}
		}


	}

	public void Clear()
	{
		//get list of active lines and remove them
		var activeLines = lineFactory.GetActive();

		foreach (var line in activeLines) 
		{
			line.gameObject.SetActive(false);
		}

		//clear pictures
		foreach (var pic in InstantiatedPictures)
        {
			Destroy(pic);
		}
		InstantiatedPictures.Clear();
	}

	public void changeColor()
    {
		Erase = false;
		CharacterPlacement = false;

		//get color of box and make that color of line
		col = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color;
	}

	public void changeThickness()
    {
		width = ThicknessSlider.value;
    }

	public void Eraser()
    {
		Erase = true;
		CharacterPlacement = false;
    }

	public void Characters()
    {
		Erase = false;
		CharacterPlacement = true;

		//save image to instantiate when the user clicks
		characterSprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
    }
}
