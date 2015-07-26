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
	private bool menu = false;

	void Start ()
	{
		//SCREEN ORIENTATION
		Screen.orientation = ScreenOrientation.LandscapeLeft;

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
			mBundleInstance = Instantiate (bundle.mainAsset) as GameObject;
			mBundleInstance.transform.localPosition = new Vector3 (0, 0, 0);
			if (AssetName == "CValencia") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -1);
			}
			if (AssetName == "TSerrano") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.6F);
			}
			if (AssetName == "BPublica") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.8F);
			}
			if (AssetName == "TQuart") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.5F);
			}
			if (AssetName == "Lonja") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.7F);
			}
			if (AssetName == "PGeneralitat") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.65F);
			}
			if (AssetName == "Miguelete") {
				mainCamera.transform.position = new Vector3 (0, 0.3F, -0.5F);
			}
			if (AssetName == "MUVIM") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -1);
			}
			if (AssetName == "MColon") {
				mainCamera.transform.position = new Vector3 (0, 0.2F, -0.6F);
			}
			mainCamera.transform.LookAt (mBundleInstance.transform.position);
		}
	}
	// END DOWNLOAD ASSETS


	void rotate (string pos)
	{
		if (pos == "up")
			mainCamera.transform.RotateAround (mBundleInstance.transform.position, Vector3.right, Time.deltaTime * speedRotation);
		if (pos == "down")
			mainCamera.transform.RotateAround (mBundleInstance.transform.position, Vector3.left, Time.deltaTime * speedRotation);
		if (pos == "left")
			mBundleInstance.transform.Rotate (Vector3.up * Time.deltaTime * speedRotation);
		if (pos == "right") {
			mBundleInstance.transform.Rotate (Vector3.down * Time.deltaTime * speedRotation);
		}
		mainCamera.transform.LookAt (mBundleInstance.transform.position);
	}


	void touchAndroid ()
	{
		//ESCAPE BUTTOn & BACK BUTTON
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.LoadLevel ("main_scene");
		}

		{
			//#if UNITY_ANDROID
			if (Input.touchCount > 0) {
				Touch touch = Input.touches [0];
				switch (touch.phase) {
					
				case TouchPhase.Began:
					startPos = touch.position;

					break;
				case TouchPhase.Stationary:
					menu = true;
					break;

				case TouchPhase.Moved:
					float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
					
					if (swipeDistHorizontal > minSwipeDistX && touch.position.y > Screen.height / 5) {
						
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

	void OnGUI ()
	{
		if (menu) {
			GUI.Box (new Rect (0, Screen.height - Screen.height / 5, Screen.width, Screen.height), "Camera Controls");
			GUILayout.BeginHorizontal ();

			if (GUI.Button (new Rect (0, Screen.height - Screen.height / 6, Screen.width / 5, Screen.height / 6), "UP")) {
				rotate ("up");
			}
			if (GUI.Button (new Rect (Screen.width / 5, Screen.height - Screen.height / 6, Screen.width / 5, Screen.height / 6), "DOWN")) {
				rotate ("down");
			}
			if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height - Screen.height / 6, Screen.width / 5, Screen.height / 6), "CLOSE")) {
				menu = false;
			}
			if (GUI.Button (new Rect (Screen.width * 3 / 5, Screen.height - Screen.height / 6, Screen.width / 5, Screen.height / 6), "ZOOM IN")) {
				mainCamera.fieldOfView -= 5.0f; 
			}
			if (GUI.Button (new Rect (Screen.width * 4 / 5, Screen.height - Screen.height / 6, Screen.width / 5, Screen.height / 6), "ZOOM OUT")) {
				mainCamera.fieldOfView += 5.0f;
			}
			GUILayout.EndHorizontal ();
		}

	}
}