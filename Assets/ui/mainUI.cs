﻿using UnityEngine;
using System.Collections;
using Vuforia;
using SimpleJSON;

public class mainUI : MonoBehaviour
{

	//GLOBAL CONST START
	public static JSONNode element;
	public static string elementName;
	private static int StLoad = 0;
	//GLOBAL CONST END
	private Rect box; //UI BOX RECT
	private GUIStyle mStyle; //BACKGROUND IMAGE
	//--//
	private float mAboutTitleHeight = 80.0f;
	public TextAsset m_AboutText;
	GUIStyle mAboutTitleBgStyle;
	GUIStyle mOKButtonBgStyle;
	private const float ABOUT_TEXT_MARGIN = 20.0f;
	private const float START_BUTTON_VERTICAL_MARGIN = 10.0f;
	private const string mTitle = "APP Realidad Aumentada Valencia";
	private float mStartButtonAreaHeight = 80.0f;
	private GUISkin mUISkin;
	private Vector2 mScrollPosition;
	public System.Action OnStartButtonTapped;
	private RaycastHit hit;
	public Camera Camera;
	public int timer;
	private bool inet = true;
	private bool objectTapped = false;
	private static JSONNode J;
	private static JSONNode aux;
	private bool GUIdrawed = true;

	private static float DeviceDependentScale {
		get {
			if (Screen.width > Screen.height)
				return Screen.height / 480f;
			else 
				return Screen.width / 480f; 
		}
	}

	// Use this for initialization
	void Start ()
	{
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.orientation = ScreenOrientation.AutoRotation;
		//CALL CAMERA
		m_AboutText = Resources.Load ("PFC_About") as TextAsset;
		mAboutTitleBgStyle = new GUIStyle ();
		mOKButtonBgStyle = new GUIStyle ();
		mAboutTitleBgStyle.normal.background = Resources.Load ("Images/grayTexture") as Texture2D;
		mOKButtonBgStyle.normal.background = Resources.Load ("Images/capture_button_normal_XHigh") as Texture2D;
		mAboutTitleBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
		mOKButtonBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
		mUISkin = Resources.Load ("Images/ButtonSkinsXHDPI") as GUISkin;
		mUISkin.label.font = Resources.Load ("SourceSansPro-Regular") as Font;

		//DPI CONF
		if (Screen.dpi > 300) {
			// load and set gui style
			mUISkin = Resources.Load ("Images/ButtonSkinsXHDPI") as GUISkin;
			mAboutTitleBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
			mOKButtonBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
		} else if (Screen.dpi > 260) {
			// load and set gui style
			mUISkin = Resources.Load ("Images/ButtonSkins") as GUISkin;
			mAboutTitleBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
			mOKButtonBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
		} else if (Screen.height == 1848 && Screen.width == 1200) {
			mUISkin = Resources.Load ("Images/ButtonSkinsXHDPI") as GUISkin;
			mAboutTitleBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
			mOKButtonBgStyle.font = Resources.Load ("SourceSansPro-Regular_big_xhdpi") as Font;
		} else {
			// load and set gui style
			mUISkin = Resources.Load ("Images/ButtonSkinsSmall") as GUISkin;
			mUISkin.label.font = Resources.Load ("SourceSansPro-Regular_Small") as Font;
			mAboutTitleBgStyle.font = Resources.Load ("SourceSansPro-Regular") as Font;
			mOKButtonBgStyle.font = Resources.Load ("SourceSansPro-Regular") as Font;
		}
		//END DPI CONF

		mOKButtonBgStyle.normal.textColor = Color.white;
		mAboutTitleBgStyle.alignment = TextAnchor.MiddleLeft;
		mOKButtonBgStyle.alignment = TextAnchor.MiddleCenter;

		//CHECKING THE CONNECTION
		CheckConnection ();
		if (inet) {
			Debug.Log ("INTERNET DISPONIBLE");
			// DOWNLOAD JSON if INTERNET
			StartCoroutine (downloadJson ());
			
		} else {
			Debug.Log ("INTERNET NO DISPONIBLE");		
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
		timer -= 1;
		if (timer == 0)
			callAutoFocus ();

		if (Application.platform == RuntimePlatform.WindowsEditor && !GUIdrawed) {
			checkInputWindows ();
		} else if (!GUIdrawed) {
			checkInputMobile ();
		}

		//MENU BUTTON
		if (Input.GetKey (KeyCode.Menu)) {
			StLoad = 0;
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (!GUIdrawed) {
				StLoad = 0;
			} else {
				objectTapped = false;
				StLoad++;
			}
		}
	}
	void callAutoFocus ()
	{
		CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
	}

	void OnGUI ()
	{
		GUIdrawed = true;

		if (StLoad == 0) {
			draw (mTitle);
			GUIdrawed = true;
			return;
		} 
		if (objectTapped) {
			GUIdrawed = true;
			draw (aux ["nombre"], aux ["descCompleta"], "", aux ["modelo3d"].ToString (), "Cerrar", aux ["coords"]);
			return;
		}

		GUIdrawed = false;
	}

	void OnApplicationPause (bool pause)
	{
		if (!pause) {
			// App resumed
			callAutoFocus ();
		}
	}

	//DRAW ABOUT PAGE
	void drawAbout ()
	{

		//DRAW THE BOX
		box = new Rect (0, 0, Screen.width, Screen.height);
		mStyle = new GUIStyle ();
		mStyle.normal.background = Resources.Load ("Images/main_background") as Texture2D;
		GUI.Box (box, "", mStyle);
	}

	void draw (string title, string desc="about", string img="", string model3d="", string button="OK", string maps="")
	{
		float scale = 1 * DeviceDependentScale;
		mAboutTitleHeight = 80.0f * scale;
		drawAbout ();
		GUI.Box (new Rect (0, 0, Screen.width, mAboutTitleHeight), string.Empty, mAboutTitleBgStyle);
		GUI.Box (new Rect (ABOUT_TEXT_MARGIN * DeviceDependentScale, 0, Screen.width, mAboutTitleHeight), title, mAboutTitleBgStyle);
		float width = Screen.width / 1.5f;
		//float height = startButtonStyle.normal.background.height * scale;
		float height = mOKButtonBgStyle.normal.background.height * scale;
		
		mStartButtonAreaHeight = height + 2 * (START_BUTTON_VERTICAL_MARGIN * scale);
		float left = Screen.width / 2 - width / 2;
		float top = Screen.height - mStartButtonAreaHeight + START_BUTTON_VERTICAL_MARGIN * scale;
		
		GUI.skin = mUISkin;
		
		GUILayout.BeginArea (new Rect (ABOUT_TEXT_MARGIN * DeviceDependentScale,
		                             mAboutTitleHeight + 5 * DeviceDependentScale,
		                             Screen.width - (ABOUT_TEXT_MARGIN * DeviceDependentScale),
		                             Screen.height - (mStartButtonAreaHeight) - mAboutTitleHeight - 5 * DeviceDependentScale));
		
		mScrollPosition = GUILayout.BeginScrollView (mScrollPosition, false, false, GUILayout.Width (Screen.width - (ABOUT_TEXT_MARGIN * DeviceDependentScale)), 
		                                             GUILayout.Height (Screen.height - mStartButtonAreaHeight - mAboutTitleHeight - START_BUTTON_VERTICAL_MARGIN * DeviceDependentScale));
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();

		if (desc == "about")
			GUILayout.Label (m_AboutText.text);
		else
			GUILayout.Label (desc);
		
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		GUILayout.EndScrollView ();
		
		GUILayout.EndArea ();
		if (model3d != "") { // 3 buttons
			if (GUI.Button (new Rect (left / 2, top, width / 3, height), "3D Scene", mOKButtonBgStyle)) {
			
				element = aux; // set the global variable for acces from the other scene
				Application.LoadLevel ("vis");	//Draw new scene

			}
			// if button was pressed, remember to make sure this event is not interpreted as a touch event somewhere else
			if (GUI.Button (new Rect (left + width / 3, top, width / 3, height), button, mOKButtonBgStyle)) {
				objectTapped = false;
				//update the vector pos
				mScrollPosition = new Vector2 ();
			}
			if (maps != "")
			if (GUI.Button (new Rect (left + left / 2 + width * 2 / 3, top, width / 3, height), "MAPS", mOKButtonBgStyle)) {
				Application.OpenURL (maps);
			}
		} else if (maps != "") { // ONLY 2 BUTTONS
			if (GUI.Button (new Rect (left, top, width / 3, height), button, mOKButtonBgStyle)) {
				objectTapped = false;
				//update the vector pos
				mScrollPosition = new Vector2 ();
			}
			if (GUI.Button (new Rect (2 * left + width / 3, top, width / 3, height), "MAPS", mOKButtonBgStyle)) {
				Application.OpenURL (maps);
			}
		} else { // JUST 1 BUTTON
			if (GUI.Button (new Rect (left + width / 3, top, width / 3, height), button, mOKButtonBgStyle)) {
				objectTapped = false;
				//update the vector pos
				mScrollPosition = new Vector2 ();
				StLoad++; // no more about
			}
		}
	} // END DRAW

	void checkInputMobile ()
	{
		
		for (int i = 0; i < Input.touchCount; ++i) {
			if (Input.GetTouch (i).phase.Equals (TouchPhase.Began)) {
				Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (i).position);
				if (Physics.Raycast (ray, out hit)) {
					hit.transform.gameObject.SendMessage ("OnMouseDown");
					aux = J ["pdi"] [hit.transform.gameObject.name];
					elementName = hit.transform.gameObject.name;
					objectTapped = true;
				}
			}
//			if (Input.GetTouch (i).phase.Equals (TouchPhase.Stationary)) {
//				if (!CameraDevice.Instance.SetFlashTorchMode (true))
//					CameraDevice.Instance.SetFlashTorchMode (true);
//				else
//					CameraDevice.Instance.SetFlashTorchMode (false);
//			}
		}
	}
	
	void checkInputWindows ()
	{
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			RaycastHit raycastHit;
			Ray ray = Camera.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out raycastHit, 1000.0f) && raycastHit.collider) {
				Debug.Log (raycastHit.transform.name + " Hitted");
				aux = J ["pdi"] [raycastHit.transform.name];
				elementName = raycastHit.transform.name;
				objectTapped = true;

			}
		}
		
	}

	IEnumerator CheckConnection ()
	{
		const float timeout = 10f;
		float startTime = Time.timeSinceLevelLoad;
		Ping ping = new Ping ("google.com");
		
		while (true) {
			if (ping.isDone) {
				inet = true;
				yield break;
			}
			if (Time.timeSinceLevelLoad - startTime > timeout) {
				inet = false;
				yield break;
			}
			
			yield return new WaitForEndOfFrame ();
		}
	}
	IEnumerator downloadJson ()
	{
		string url = "http://sersanor.com/pfc/data.json";
		WWW www = new WWW (url);
		
		yield return www;
		if (www.error == null) {
			//Sucessfully loaded the JSON string
			Debug.Log ("Loaded following JSON string" + www.text);
			J = JSONNode.Parse (www.text);
		} else {
			Debug.Log ("ERROR: " + www.error);
		}
	}

}
