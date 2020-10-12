///////////////////////////////////////////
// author     : chen yong
// create time: 2019/01/04
// modify time: 
// description: Camera transition effect
///////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraTurnOver : MonoBehaviour {
    public GameObject scene1;
    public GameObject scene2;

    public Material filterMtl;
    public float strength = 0.4f;
    public float duration = 1.0f;

    RenderTexture screenShotRT = null;
    bool running = false;
    float time = 0;

    int width;
    int height;

    public void ChangeScene(bool active1, bool active2)
    {
        KaCa();
        running = true;
        time = 0;

        if (scene1 != null) {
            scene1.SetActive(active1);
        }

        if (scene2 != null) {
            scene2.SetActive(active2);
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(screenShotRT);
    }

    void Awake()
    {
        width = Screen.width;
        height = Screen.height;
    }

    // Use this for initialization
    void Start () {
        // StartCoroutine(TransitionCoroutine());
	}
	
	// Update is called once per frame
	void Update () {
        if (!running)
            return;

        if (time <= duration)
        {
            time += Time.deltaTime;            
        }
        else
        {
            running = false;
        }
	}

    IEnumerator TransitionCoroutine()
    {
        yield return new WaitForSeconds(3);

        KaCa();
        running = true;
        time = 0;

        if (scene1 != null)
        {
            scene1.SetActive(false);
        }

        if (scene2 != null)
        {
            scene2.SetActive(true);
        }
    }

    void KaCa()
    {
        screenShotRT = new RenderTexture(width, height, 24);

        Camera camera = GetComponent<Camera>();
        if (camera != null && camera.enabled && camera.targetTexture == null)
        {
            camera.targetTexture = screenShotRT;
            camera.Render();
            camera.targetTexture = null;
        }     
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!running)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        if (filterMtl != null)
        {
            filterMtl.SetFloat("_T", time);
            filterMtl.SetFloat("_GlitchStrength", strength);
            filterMtl.SetTexture("_SecondTex", source);
            Graphics.Blit(screenShotRT, destination, filterMtl, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
