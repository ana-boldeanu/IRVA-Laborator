using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using TMPro;

public class PokemonARManager : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    TMP_Text statusUpdate;

    private ARAnchorManager  arAnchorManager = null;
    private ARAnchor pendingHostAnchor = null;
    private string anchorIdToResolve;
    private AnchorCreatedEvent anchorCreatedEvent = null;
    public static PokemonARManager Instance { get; private set; }
    public GameObject middle;
    public GameObject main;

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
        anchorCreatedEvent.AddListener((t) => CloudAnchorObjectPlacement.Instance.RecreatePlacement(t));
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
        FeatureMapQuality quality = arAnchorManager.EstimateFeatureMapQualityForHosting(GetCameraPose());
        StartCoroutine(DisplayStatus("HostAnchor call in progress. Feature Map Quality: " + quality));

        if (quality != FeatureMapQuality.Insufficient)
        {
            /* Start the hosting process */
            HostCloudAnchorPromise cloudAnchorPromise = arAnchorManager.HostCloudAnchorAsync(pendingHostAnchor, 1);

            /* Wait for the promise to solve (Hint! Pass the HostCloudAnchorPromise variable to the coroutine) */
            StartCoroutine(WaitHostingResult(cloudAnchorPromise));
        }
    }

    public void Resolve()
    {
        StartCoroutine(DisplayStatus("Resolve call in progress"));

        /* Start the resolve process and wait for the promise */
        ResolveCloudAnchorPromise cloudAnchorPromise = arAnchorManager.ResolveCloudAnchorAsync(anchorIdToResolve);

        StartCoroutine(WaitResolvingResult(cloudAnchorPromise));
    }

    private IEnumerator WaitHostingResult(HostCloudAnchorPromise hostingPromise)
    {
        /* Wait for the promise. Save the id if the hosting succeeded */
        yield return hostingPromise;

        if (hostingPromise.State == PromiseState.Cancelled)
        {
            yield break;
        }

        var result = hostingPromise.Result;

        if (result.CloudAnchorState == CloudAnchorState.Success)
        {
            anchorIdToResolve = result.CloudAnchorId;
            StartCoroutine(DisplayStatus("Anchor hosted successfully!"));
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
            StartCoroutine(DisplayStatus("Anchor resolved successfully!"));
        }
        else
        {
            StartCoroutine(DisplayStatus("Error while resolving cloud anchor"));
        }
    }
}
