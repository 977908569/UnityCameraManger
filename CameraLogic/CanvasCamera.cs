namespace Game {
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityStandardAssets.ImageEffects;

	/// <summary>
	///		UI Canvas Camera
	/// </summary>
	public class CanvasCamera : MonoBehaviour {

		public static CanvasCamera instance;
		private void Awake()
		{
			instance = this;
		}

		/// <summary>
		///		Root transform.
		/// </summary>
		public static Transform Root {
			get {
				if (root == null) root = new GameObject("UI");
				return root.transform;
			}
		}

		/// <summary>
		///		Camera object.
		/// </summary>
		public static Camera Camera { get; private set; }

		/// <summary>
		///		Get Canvas Aspect Ratio
		/// </summary>
		public static float ScreenAspectRatio {
			get {
				return Screen.width * 1.0f / Screen.height; 
			}
		}

		/// <summary>
		///		Check if current pointer is over UI.
		/// </summary>
		public static bool IsPointerOverUI {
			get {
				return EventSystem.current.IsPointerOverGameObject();
			}
		}

		public static void ShowBlur(bool show)
		{
			if (instance == null)
				return;
			BlurOptimized blur = instance.transform.GetComponent<BlurOptimized>();
			blur.enabled = show;
		}

		/// <summary>
		///		Initialization.
		/// </summary>
		public static void Initialize()
		{
			GameObject prefab = AssetsMgr.Load<GameObject>("AssetBundle/Prefabs/Camera/CanvasCamera.prefab");
			GameObject proxy = Instantiate(prefab);
			proxy.AddComponent<CanvasCamera>();
			Camera = proxy.GetComponent<Camera>();
			//Bloom = proxy.GetComponent<UnityStandardAssets.ImageEffects.Bloom>();
			proxy.name = "CanvasCamera";
			proxy.transform.position = Vector3.zero;

			DontDestroyOnLoad(proxy);
		}

		/// <summary>
		///		Get Pointed UI GameObject.
		/// </summary>
		/// <returns>All pointed game object.</returns>
		static List<RaycastResult> ret = new List<RaycastResult>();
        public static List<RaycastResult> GetPointedObject() {
            ret.Clear();
            PointerEventData ev = new PointerEventData(EventSystem.current);
			ev.position = Input.mousePosition;
			EventSystem.current.RaycastAll(ev, ret);
			return ret;
		}

#region PRIVATE
		static GameObject root = null;
#endregion
	}
}
