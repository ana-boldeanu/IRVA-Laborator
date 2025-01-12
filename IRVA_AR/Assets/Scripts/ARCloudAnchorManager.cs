using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using TMPro;
using System.Collections.Generic;

public class AnchorCreatedEvent : UnityEvent<Transform> { }

/* TODO 1. Enable ARCore Cloud Anchors API on Google Cloud Platform */
public class ARCloudAnchorManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    TMP_Text statusUpdate;

    private ARAnchorManager  arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public static ARCloudAnchorManager Instance { get; private set; }
    public GameObject middle;
    public GameObject main;

    private string[] anchorIdsToResolve = new string[] { null, null, null, null };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        anchorCreatedEvent = new AnchorCreatedEvent();
        anchorCreatedEvent.AddListener((t) => CloudAnchorObjectPlacement.Instance.RecreatePlacement(t, PokemonGameManager.Instance.selectedIndex));
    }

    private Pose GetCameraPose()
    {
        return new Pose(arCamera.transform.position, arCamera.transform.rotation);
    }
    public void QueueAnchor(ARAnchor arAnchor)
    {
        pendingHostAnchor = arAnchor;
    }

    public IEnumerator DisplayStatus(string text)
    {
        statusUpdate.text = text;
        yield return new WaitForSeconds(3);
        statusUpdate.text = "";
    }

    public void HostAnchor()
    {
        /* TODO 3.1 Get FeatureMapQuality */
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        StartCoroutine(DisplayStatus("Feature Map Quality: " + quality));

        if (quality != FeatureMapQuality.Insufficient)
        {
            /* TODO 3.2 Start the hosting process */
            HostCloudAnchorPromise cloudAnchorPromise = arAnchorManager.HostCloudAnchorAsync(pendingHostAnchor, 1);

            /* Wait for the promise to solve (Hint! Pass the HostCloudAnchorPromise variable to the coroutine) */
            StartCoroutine(WaitHostingResult(cloudAnchorPromise));
        }
    }

    public void Resolve()
    {
        string anchorIdToResolve = anchorIdsToResolve[PokemonGameManager.Instance.selectedIndex];

        if (anchorIdToResolve == null)
        {
            StartCoroutine(DisplayStatus("This pokemon has not been hosted yet!"));
            return;
        }

        StartCoroutine(DisplayStatus("Resolve call in progress"));

        /* TODO 5 Start the resolve process and wait for the promise */
        ResolveCloudAnchorPromise cloudAnchorPromise = arAnchorManager.ResolveCloudAnchorAsync(anchorIdToResolve);

        StartCoroutine(WaitResolvingResult(cloudAnchorPromise));
    }

    private IEnumerator WaitHostingResult(HostCloudAnchorPromise hostingPromise)
    {
        /* TODO 3.3 Wait for the promise. Save the id if the hosting succeeded */
        yield return hostingPromise;

        if (hostingPromise.State == PromiseState.Cancelled)
        {
            yield break;
        }

        var result = hostingPromise.Result;

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            anchorIdsToResolve[PokemonGameManager.Instance.selectedIndex] = result.CloudAnchorId;
            StartCoroutine(DisplayStatus(PokemonGameManager.Instance.GetSelectedPokemon().name +  " hosted successfully!"));
        }
        else
        {
            StartCoroutine(DisplayStatus("Error in hosting the anchor: " + result.CloudAnchorState));
        }
    }

    private IEnumerator WaitResolvingResult(ResolveCloudAnchorPromise resolvePromise)
    {
        yield return resolvePromise;

        if (resolvePromise.State == PromiseState.Cancelled)
        {
            yield break;
        }

        var result = resolvePromise.Result;

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            anchorCreatedEvent?.Invoke(result.Anchor.transform);
            StartCoroutine(DisplayStatus(PokemonGameManager.Instance.GetSelectedPokemon().name + " resolved successfully!"));
        }
        else
        {
            StartCoroutine(DisplayStatus("Error while resolving cloud anchor" + result.CloudAnchorState));
        }
    }
}
