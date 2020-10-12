namespace Game {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using DG.Tweening;
	using UnityEngine.PostProcessing;
	using System;
	using UnityStandardAssets.ImageEffects;

	/// <summary>
	///		Model Preview Camera
	/// </summary>
	public class PreviewCamera : MonoBehaviour {

		public static void Initialize()
		{
			GameObject prefab = AssetsMgr.Load<GameObject>("AssetBundle/Prefabs/Camera/PreviewCamera.prefab");
			GameObject proxy = Instantiate(prefab);
			proxy.name = "PreviewCamera";
			DontDestroyOnLoad(proxy);
		}

		/// <summary>
		///		Show model by item instance object
		/// </summary>
		/// <param name="item">Item</param>
		/// <param name="offset">Offset from camera</param>
		public static void Show(BaseItem item, Vector3 offset)
        {
			if (item.GetItemType() == emItemType.Card)
            {
				ins.anchor.gameObject.SetActive(true);
                ins.StopAllCoroutines();
                ins.StartCoroutine(ins.DisplayCharacher(item as Card, offset) );
			}
            else if (item.GetItemType() == emItemType.Weapon)
            {
				
            }
		}

		public static void ShowByPath(string modelPath, Vector3 offset,Action complete=null)
		{
			ins.anchor.gameObject.SetActive(true);
			ins.StopAllCoroutines();
			ins.StartCoroutine(ins.DisplayCharacher(modelPath, offset,complete));
		}

		public static void HideCache( string _name )
        {
            ins.cache.Find( _name ).gameObject.SetActive( false );
        }

        public static void RemoveAllCache()
        {
            ins.DetachCache();
        }

        public static void StopCoroutine()
        {
            ++nShow;
            ins.StopAllCoroutines();
        }
		/// <summary>
		///		Show last display model
		/// </summary>
        public static void Show()
        {
            ins.anchor.gameObject.SetActive(true);
            ins.cache.gameObject.SetActive( true );
        }
		public static void PostBehaviour(bool show)
		{
			if (ins.postBehaviour != null)
			{
				ins.postBehaviour.enabled = show;
			}
		}

		public static Transform FindEffect(string path)
		{
			return ins.CurShowCacheCharactor.transform.GetChild(0).Find(path);
		}

		public static void ShowBlur(bool show)
		{
			if (ins == null)
				return;
			BlurOptimized blur = ins.previewCamera.transform.GetComponent<BlurOptimized>();
			blur.enabled = show;
		}
		/// <summary>
		///		Clear all.
		/// </summary>
		public static void Clear() {
			ins.Detach();
            ins.DetachCache();
		}

		/// <summary>
		///		Hide preview
		/// </summary>
		public static void Hide() {
			ins.anchor.gameObject.SetActive(false);
            ins.cache.gameObject.SetActive( false );
		}

		/// <summary>
		///		Toggle model rotate around Y-axis
		/// </summary>
		/// <param name="rotating">Is this feature enabled</param>
		public static void ToggleRotate(bool rotating) {
			ins.rotator.enabled = rotating;
		}

		/// <summary>
		///		Directly rotate 
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static void SetRotation(float x, float y, float z) {
			ins.previewCamera.transform.localRotation = Quaternion.Euler(x, y, z);
        }

        public static void SetScale(float x)
        {
            ins.anchor.localScale = Vector3.one * x;
        }

		/// <summary>
		///		Move camera.
		/// </summary>
		/// <param name="offset">Offset from camera</param>
		/// <param name="duration">Duration</param>
		/// <param name="onFinish">Callback on move finished</param>
		public static void Move(Vector3 offset, float duration, System.Action onFinish) {
			if (duration <= 0) {
				ins.anchor.localPosition = offset;
                if (onFinish != null) onFinish();
			} else {
				ins.anchor.DOLocalMove(offset, duration).OnComplete(() => {
					if (onFinish != null) onFinish();
				});
            }
		}

		public static void MoveCamera(Vector3 pos, float duration, System.Action onFinish)
		{
			if (duration <= 0)
			{
				ins.previewCamera.transform.localPosition = pos;
				if (onFinish != null) onFinish();
			}
			else
			{
				ins.previewCamera.transform.DOLocalMove(pos, duration).OnComplete(() =>
				{
					if (onFinish != null) onFinish();
				});
			}
		}

		void Awake() {
			ins = GetComponent<PreviewCamera>();
			previewCamera = GetComponentInChildren<Camera>();
			anchor = transform.Find("Anchor");
			rotator = anchor.GetComponent<CircleRotate>();
			postBehaviour = previewCamera.GetComponent<PostProcessingBehaviour>();
			defaultDepth = previewCamera.depth;
            cache = transform.Find( "Cache" );
        }

		IEnumerator DisplayCharacher(Card item, Vector3 offset)
        {
            ++nShow;
            int tmepShow = nShow;

            Detach();
			rotator.enabled = false;
			previewCamera.transform.localPosition = offset;
            yield return null;
            GameObject root = new GameObject( "Card" );
			CurShowCacheCharactor = root;
			anchor.transform.localPosition = Vector3.zero;
			anchor.transform.localRotation = Quaternion.identity;
			Utils.SetParent( anchor, root.transform, true );
			GameObject _modle = AssetsMgr.Instance(GamePaths.GetCharacterPath(item.BaseInfo.modelPath));
			if (_modle != null)
			{
				Utils.SetLayer(_modle, GameLayers.ShowModel);
				Utils.SetParent(root.transform, _modle.transform);
			}
        }

		IEnumerator DisplayCharacher(string path, Vector3 offset,Action complete=null)
		{
			++nShow;
			int tmepShow = nShow;

			Detach();
			rotator.enabled = false;
			previewCamera.transform.localPosition = offset;
			yield return null;
			GameObject root = new GameObject("Model");
			CurShowCacheCharactor = root;

			Utils.SetParent(anchor, root.transform, true);
			GameObject _modle = AssetsMgr.Instance(path);
			if (_modle != null)
			{
				Utils.SetLayer(_modle, GameLayers.ShowModel);
				Utils.SetParent(root.transform, _modle.transform);
			}

			if (complete != null)
			{
				complete();
			}
		}

		public static void PlayUICG(string cgName)
		{
			Animator animator = ins.previewCamera.GetComponentInParent<Animator>();
			if (animator == null)
				return;
			//
			animator.Rebind();
			animator.Play("Take");
			//GameObject ctr = AssetsMgr.DoLoad<GameObject>(GamePaths.GetCGPath(cgName));
			//if (ctr != null)
			//{
			//	Animator _ani = ctr.GetComponent<Animator>();
			//	animator.runtimeAnimatorController = _ani.runtimeAnimatorController;
			//	animator.Play("Play");
			//}
		}

		public static void PlayAnimation(string _name)
		{
			if (ins.CurShowCacheCharactor == null)
				return;
			Animator animator = ins.CurShowCacheCharactor.GetComponentInChildren<Animator>();
			if (animator == null)
				return;
			animator.Play(_name);
		}

		public static void SetBool(string name, bool value)
		{
			if (ins.CurShowCacheCharactor == null)
				return;
			Animator animator = ins.CurShowCacheCharactor.GetComponentInChildren<Animator>();
			if (animator == null)
				return;
			animator.SetBool(name, value);
		}
		public static void SetTrigger(string name)
		{
			if (ins.CurShowCacheCharactor == null)
				return;
			Animator animator = ins.CurShowCacheCharactor.GetComponentInChildren<Animator>();
			if (animator == null)
				return;
			animator.SetTrigger(name);
		}
		void Detach() {
			List<Transform> children = new List<Transform>();
			for (int i = 0; i < anchor.childCount; ++i) children.Add(anchor.GetChild(i));
			for (int i = 0; i < children.Count; ++i) DestroyImmediate(children[i].gameObject);
		}

        void DetachCache()
        {
            List<Transform> children = new List<Transform>();
            for( int i = 0; i < cache.childCount; ++i ) children.Add( cache.GetChild( i ) );
            for( int i = 0; i < children.Count; ++i ) DestroyImmediate( children[i].gameObject );
            CurShowCacheCharactor = null;
        }

        #region PRIVATE
        static PreviewCamera ins;

		Camera previewCamera;
		Transform anchor;
        Transform cache;
        GameObject CurShowCacheCharactor;
        CircleRotate rotator;
		PostProcessingBehaviour postBehaviour;
		BlurOptimized blurOptimized;
		float defaultDepth;

        static int nShow = 0;
		#endregion
	}
}
