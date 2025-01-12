using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class CloudAnchorObjectPlacement : MonoBehaviour
{
    public GameObject[] spawnedObjects = new GameObject[] { null, null, null, null };
    public Camera FirstPersonCamera;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    ARRaycastManager m_RaycastManager;

    public static CloudAnchorObjectPlacement Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            touchPosition = default;
            return false;
        }

        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                touchPosition = default;
                return false;
            }

            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (spawnedObjects[PokemonGameManager.Instance.selectedIndex] != null)
            return;

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;

            /* TODO 2.1. Instantiate a new prefab on scene */
            GameObject prefab = PokemonGameManager.Instance.GetSelectedPokemon();

            Vector3 direction = hitPose.position - FirstPersonCamera.transform.position;
            Quaternion lookAtRotation = Quaternion.Euler(0, Quaternion.LookRotation(direction).eulerAngles.y, 0);
            GameObject spawnedObject = Instantiate(prefab, hitPose.position, lookAtRotation * Quaternion.Euler(0, 180, 0));

            spawnedObjects[PokemonGameManager.Instance.selectedIndex] = spawnedObject;
            PokemonGameManager.Instance.SetPokemonStats();

            /* TODO 2.2 Attach an anchor to the prefab */
            ARAnchor anchor = spawnedObject.AddComponent<ARAnchor>();
            spawnedObject.transform.parent = anchor.transform;

            /* Send the anchor to ARCloudAnchorManager */
            ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        }
    }

    /* Add the object on scene after the anchor has been resolved */
    public void RecreatePlacement(Transform transform, int pokemonIndex)
    {
        if (spawnedObjects[pokemonIndex] != null)
        {
            StartCoroutine(ARCloudAnchorManager.Instance.DisplayStatus("This pokemon is already in place!"));
            return;
        }

        GameObject prefab = PokemonGameManager.Instance.pokemonPrefabs[pokemonIndex];
        GameObject spawnedObject = Instantiate(prefab, transform.position, transform.rotation);
        spawnedObject.transform.parent = transform;

        spawnedObjects[pokemonIndex] = spawnedObject;
    }

    public void RemovePlacement()
    {
        /* TODO 4 Remove the cube from screen */
        int index = PokemonGameManager.Instance.selectedIndex;
        PokemonGameManager.Instance.pokemonStatsParent.SetActive(false);
        PokemonGameManager.Instance.fightersParent.SetActive(false);

        Destroy(spawnedObjects[index]);
        spawnedObjects[index] = null;
    }
}
