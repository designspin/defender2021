using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public Material TransitionMaterial;
    public Texture2D TransitionTexture;
    public static event Action OnTransitionEnd = delegate { };
    private float transitionDuration = 1.5f;
    private Texture2D[] TransitionTextures = new Texture2D[4];

    void Start() {
        TransitionTextures[0] = Resources.Load<Texture2D>("transition-1");
        TransitionTextures[1] = Resources.Load<Texture2D>("transition-2");
        TransitionTextures[2] = Resources.Load<Texture2D>("transition-3");
        TransitionTextures[3] = Resources.Load<Texture2D>("transition-4");

        if(SceneManagerScript.Instance.IsTransitioned) {
            TransitionMaterial.SetFloat("_Cutoff", 0f);
        } else {
            TransitionMaterial.SetFloat("_Cutoff", 1f);
        }
        
        TransitionMaterial.SetTexture("_TransitionTex", TransitionTexture);
    }
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, TransitionMaterial);
    }

    public void StartTransition()
    {
        StartCoroutine(Transition());
    }

    public void SetTransition(int textureNo)
    {
        TransitionMaterial.SetTexture("_TransitionTex", TransitionTextures[textureNo]);
    }

    IEnumerator Transition()
    {
        float complete = (SceneManagerScript.Instance.IsTransitioned) ? 0f : 1f;

        while (SceneManagerScript.Instance.IsTransitioned ? complete < 1f : complete > 0f)
        {
            if(SceneManagerScript.Instance.IsTransitioned) {
                complete += Time.deltaTime / transitionDuration;
            } else {
                complete -= Time.deltaTime / transitionDuration;
            }

            complete = Mathf.Clamp01(complete);
            TransitionMaterial.SetFloat("_Cutoff", complete);
            yield return null;
        }

        SceneManagerScript.Instance.IsTransitioned = !SceneManagerScript.Instance.IsTransitioned;
        OnTransitionEnd();
    }
}
