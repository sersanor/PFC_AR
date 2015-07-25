using UnityEngine;
using System.Collections;
using SimpleJSON;

public class visUI : MonoBehaviour
{
	
	public float minSwipeDistY;
	public float minSwipeDistX;
	public GameObject objeto;
	public float speedRotation = 1.0f;
	private Vector2 startPos;
	private GameObject mBundleInstance;
	private string AssetName;
	public Camera mainCamera;
	private JSONNode aux;

	void Start ()
	{

		aux = mainUI.element;
		if (aux != null) {
			Debug.Log ("Info Token");
			AssetName = mainUI.elementName;
			Debug.Log (AssetName);
			StartCoroutine (Download ());
		} else
			Debug.Log ("No Info");
	}

	void Update ()
	{
		touchAndroid ();
		//rotate ();
	}

	// START DOWLOAD ASSETS
	IEnumerator Download ()
	{
		string bundleURL = aux ["modelo3d"];
			
		Debug.Log (bundleURL);
		using (WWW www = WWW .LoadFromCacheOrDownload(bundleURL, 1)) {
			yield return www;
				
			if (www .error != null)
				throw new UnityException ("WWW Download had an error: " + www .error);
				
			AssetBundle bundle = www .assetBundle;
			if (AssetName == "CValencia") {
				Debug.Log ("Drawing");
				mBundleInstance = Instantiate (bundle.mainAsset) as GameObject;
				mBundleInstance.transform.localPosition = new Vector3 (0, 0, 0);
				mainCamera.transform.position = new Vector3 (0, 0.2F, -1);
				mainCamera.transform.LookAt (mBundleInstance.transform.position);

			} else {
				Debug.Log ("NOT BUNDLE");
				mBundleInstance = Instantiate (bundle.LoadAsset (AssetName)) as GameObject;
			}
		}
	}
	// END DOWNLOAD ASSETS


	void rotate (string pos)
	{
		if (pos == "up")
			mBundleInstance.transform.Rotate (Vector3.up * Time.deltaTime * speedRotation);
		if (pos == "down")
			mBundleInstance.transform.Rotate (Vector3.down * Time.deltaTime * speedRotation);
		if (pos == "left")
			mBundleInstance.transform.Rotate (Vector3.up * Time.deltaTime * speedRotation);
		if (pos == "right") {
			mBundleInstance.transform.Rotate (Vector3.down * Time.deltaTime * speedRotation);
		}
	}


	void touchAndroid ()
	{
		{
			//#if UNITY_ANDROID
			if (Input.touchCount > 0) {
				Touch touch = Input.touches [0];
				switch (touch.phase) {
					
				case TouchPhase.Began:
					startPos = touch.position;

					break;

				case TouchPhase.Moved:
					float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
					
					if (swipeDistHorizontal > minSwipeDistX) {
						
						float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
						
						if (swipeValue > 0) {//right swipe
							rotate ("right");
						} else if (swipeValue < 0) {//left swipe
							rotate ("left");
						}
					}
					break;
				}
			}
		}
	}
}