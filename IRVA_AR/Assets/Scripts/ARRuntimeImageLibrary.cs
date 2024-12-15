using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;

/* TODO 1 Create in Unity an image database with minimum 3 images */
/* TODO 2 Augment the database */
public class ARRuntimeImageLibrary : MonoBehaviour
{
    private ARTrackedImageManager trackImageManager;
    public GameObject m_PlacedPrefab;

    void Start()
    {
        /* TODO 3.1 Download minimum one image from the internet */
        var url = "https://img.freepik.com/premium-photo/majestic-flying-fox-mid-air-close-up-fruit-bat-soaring-gracefully-through-forest-canopy_996993-66111.jpg";
        StartCoroutine(DownloadImage(url));
    }

    /* Download and create an image database */
    IEnumerator DownloadImage(string url)
    {
        Texture2D imageToAdd;

        /* UnityWebRequest API will be used to download the image */
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            /* Downloaded image */
            imageToAdd = ((DownloadHandlerTexture)request.downloadHandler).texture;

            /* TODO 3.2 Destroy the previous ARTrackedImageManager component. 
             * Hint! What's the difference between Destroy() and DestroyImmediate()? */
            DestroyImmediate(gameObject.GetComponent<ARTrackedImageManager>());

            /* TODO 3.3 Attach a new ARTrackedImageManager component */
            trackImageManager = gameObject.AddComponent<ARTrackedImageManager>();

            /* TODO 3.4 Create a new runtime library */
            var library = trackImageManager.CreateRuntimeLibrary();

            /* TODO 3.5 Add the image to the database*/
            if (library is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                mutableLibrary.ScheduleAddImageWithValidationJob(imageToAdd, "Image Name", 0.5f /* 50 cm */);
            }

            /* Set the maximum number of moving images */
            trackImageManager.requestedMaxNumberOfMovingImages = 3;

            /* TODO 3.6 Set the new library as the reference library */
            trackImageManager.referenceLibrary = library;

            /* TODO 3.7 Enable the new ARTrackedImageManager component */
            trackImageManager.enabled = true;

            /* Attach the event handling */
            trackImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            /* TODO 3.8 Instantiate a new object in scene so that it always follows the tracked image
             * Hint! Use SetParent() method */
            GameObject placedPrefab = Instantiate(m_PlacedPrefab);
            placedPrefab.transform.localScale = new Vector3(2, 2, 2);
            placedPrefab.transform.SetParent(trackedImage.gameObject.transform);
            placedPrefab.transform.localPosition = new Vector3(0, 0, 0);
            placedPrefab.transform.localRotation = new Quaternion(0, 0, 0, 1);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            /* Handle update event */
        }

        foreach (ARTrackedImage removedImage in eventArgs.removed)
        {
            /* Handle removed event */
        }
    }
}
