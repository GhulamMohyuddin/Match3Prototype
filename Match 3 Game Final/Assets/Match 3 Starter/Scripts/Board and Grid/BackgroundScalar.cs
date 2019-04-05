using UnityEngine;
using System.Collections;

public class BackgroundScalar : MonoBehaviour {

	[SerializeField] bool onlyX;
	[SerializeField] bool onlyY;
	[SerializeField] bool maintainRatio;

	void Start () 
	{
		UpdateAndScaleBG ();	
	}

	public void UpdateAndScaleBG (string path = null)
	{
		SpriteRenderer sr=GetComponent<SpriteRenderer>();

		if(path!= null)
			sr.sprite = Resources.Load (path, typeof(Sprite)) as Sprite;

		if(sr==null) return;

		transform.localScale=new Vector3(1,1,1);

		float width =  sr.bounds.size.x;
		float height = sr.bounds.size.y;

		float worldScreenHeight= Camera.main.orthographicSize*2f;
		float worldScreenWidth=worldScreenHeight/Screen.height*Screen.width;

		Vector3 xWidth = transform.localScale;
		Vector3 yHeight = transform.localScale;

		if (onlyX) 
		{
			xWidth.x = worldScreenWidth / width;

			if (maintainRatio)
				yHeight.y = xWidth.x; 	
		}
		else if (onlyY) 
		{
			yHeight.y = worldScreenHeight / height;

			if (maintainRatio)
				yHeight.y = xWidth.x; 	
		}
		else 
		{
			xWidth.x = worldScreenWidth / width;
			yHeight.y = worldScreenHeight / height;
		}

		transform.localScale = new Vector3 (xWidth.x, yHeight.y, transform.localScale.z);
	}

}
