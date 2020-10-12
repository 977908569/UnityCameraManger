namespace Game {
    using UnityEngine;

    /// <summary>
    ///		Background UI Camera
    /// </summary>
    public class BackgoundCamera : MonoBehaviour {

        /// <summary>
        ///		Camera object.
        /// </summary>
        public static Camera Camera { get; private set; }

        /// <summary>
        ///		Initialization.
        /// </summary>
        public static void Initialize() {
            GameObject prefab = AssetsMgr.Load<GameObject>("AssetBundle/Prefabs/Camera/BackgroundCamera.prefab");
            GameObject proxy = Instantiate(prefab);

            Camera = proxy.GetComponent<Camera>();
            proxy.name = "BackgroundCamera";

            DontDestroyOnLoad(proxy);
        }
    }
}
