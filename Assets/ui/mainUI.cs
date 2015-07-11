using UnityEngine;
using System.Collections;
using Vuforia;


public class mainUI : MonoBehaviour {


	private Rect box; //UI BOX RECT
	private GUIStyle mStyle; //BACKGROUND IMAGE
	//--//
	private float mAboutTitleHeight = 80.0f;
	public TextAsset m_AboutText;
	GUIStyle mAboutTitleBgStyle;
	GUIStyle mOKButtonBgStyle;
	private const float ABOUT_TEXT_MARGIN = 20.0f;
	private const float START_BUTTON_VERTICAL_MARGIN = 10.0f;
	private const string mTitle="APP Realidad Aumentada Valencia";
	private float mStartButtonAreaHeight = 80.0f;
	private GUISkin mUISkin;
	private Vector2 mScrollPosition;
	public System.Action OnStartButtonTapped;
	private bool mustDraw = true;


	private static float DeviceDependentScale
	{
		get { if ( Screen.width > Screen.height)
			return Screen.height / 480f;
			else 
				return Screen.width / 480f; 
		}
	}

	// Use this for initialization
	void Start () {

		//CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
		m_AboutText = Resources.Load("PFC_About") as TextAsset;
		mAboutTitleBgStyle = new GUIStyle();
		mOKButtonBgStyle = new GUIStyle();
		mAboutTitleBgStyle.normal.background = Resources.Load ("Images/grayTexture") as Texture2D;
		mOKButtonBgStyle.normal.background = Resources.Load ("Images/capture_button_normal_XHigh") as Texture2D;
		mAboutTitleBgStyle.font = Resources.Load("SourceSansPro-Regular_big_xhdpi") as Font;
		mOKButtonBgStyle.font = Resources.Load("SourceSansPro-Regular_big_xhdpi") as Font;
		mUISkin = Resources.Load("Images/ButtonSkinsXHDPI") as GUISkin;
		mUISkin.label.font = Resources.Load("SourceSansPro-Regular") as Font;
		mOKButtonBgStyle.normal.textColor = Color.white;
		mAboutTitleBgStyle.alignment = TextAnchor.MiddleLeft;
		mOKButtonBgStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	// Update is called once per frame
	void Update () {
		CameraDevice.Instance.SetFocusMode (CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
	}


	void OnGUI (){
		if (mustDraw) {
			draw ();
		}
	}


	//DRAW ABOUT PAGE
	void drawAbout (){

		//DRAW THE BOX
		box = new Rect(0, 0, Screen.width, Screen.height);
		mStyle = new GUIStyle();
		mStyle.normal.background = Resources.Load("Images/main_background") as Texture2D;
		GUI.Box(box, "", mStyle);
	}

	void draw(){
		float scale = 1*DeviceDependentScale;
		mAboutTitleHeight = 80.0f* scale;
		drawAbout();
		GUI.Box(new Rect(0,0,Screen.width,mAboutTitleHeight),string.Empty,mAboutTitleBgStyle);
		GUI.Box(new Rect(ABOUT_TEXT_MARGIN * DeviceDependentScale,0,Screen.width,mAboutTitleHeight),mTitle,mAboutTitleBgStyle);
		float width = Screen.width / 1.5f;
		//float height = startButtonStyle.normal.background.height * scale;
		float height = mOKButtonBgStyle.normal.background.height * scale;
		
		mStartButtonAreaHeight = height + 2*(START_BUTTON_VERTICAL_MARGIN * scale);
		float left = Screen.width/2 - width/2;
		float top = Screen.height - mStartButtonAreaHeight + START_BUTTON_VERTICAL_MARGIN * scale;
		
		GUI.skin = mUISkin;
		
		GUILayout.BeginArea(new Rect(ABOUT_TEXT_MARGIN * DeviceDependentScale,
		                             mAboutTitleHeight + 5 * DeviceDependentScale,
		                             Screen.width - (ABOUT_TEXT_MARGIN * DeviceDependentScale),
		                             Screen.height - ( mStartButtonAreaHeight) - mAboutTitleHeight - 5 * DeviceDependentScale));
		
		mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, false, false, GUILayout.Width(Screen.width - (ABOUT_TEXT_MARGIN * DeviceDependentScale)), 
		                                            GUILayout.Height (Screen.height - mStartButtonAreaHeight - mAboutTitleHeight));
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		GUILayout.Label(m_AboutText.text);
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
		
		// if button was pressed, remember to make sure this event is not interpreted as a touch event somewhere else
		if (GUI.Button(new Rect(left, top, width, height), "OK" ,mOKButtonBgStyle))
		{
			mustDraw = false;
			if(this.OnStartButtonTapped != null)
			{
				this.OnStartButtonTapped();
			}
		}
	}
}
