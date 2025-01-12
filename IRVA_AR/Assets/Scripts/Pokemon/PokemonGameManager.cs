using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokemonGameManager : MonoBehaviour
{
    public GameObject[] pokemonPrefabs;
    public int selectedIndex = 1;

    // UI
    public TextMeshProUGUI selectedPokemonText;

    // Pokemon stats
    public GameObject pokemonStatsParent;
    public TextMeshProUGUI pokemonLevelText;
    public Slider pokemonXPSlider;
    public GameObject[] pokemonMoods;

    public static PokemonGameManager Instance { get; private set; }

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
    }

    private void Start()
    {
        selectedPokemonText.text = pokemonPrefabs[selectedIndex].name;
        pokemonStatsParent.SetActive(false);
    }

    void Update()
    {
        GameObject spawnedPokemon = CloudAnchorObjectPlacement.Instance.spawnedObjects[selectedIndex];
        if (spawnedPokemon != null)
        {
            Vector3 worldPosition = spawnedPokemon.transform.position;
            Vector3 offset = spawnedPokemon.GetComponent<PokemonController>().localHeadPosition + new Vector3(-0.4f, 0.2f , 0); 
            pokemonStatsParent.transform.position = Camera.main.WorldToScreenPoint(worldPosition + offset);
        }
    }

    public GameObject GetSelectedPokemon()
    {
        return pokemonPrefabs[selectedIndex];
    }

    public void ButtonSelectPokemon()
    {
        if (CloudAnchorObjectPlacement.Instance.spawnedObjects[selectedIndex] == null)
        {
            StartCoroutine(ARCloudAnchorManager.Instance.DisplayStatus("Where do you wish to place " + pokemonPrefabs[selectedIndex].name + "?"));
        }
        else
        {
            pokemonStatsParent.SetActive(!pokemonStatsParent.activeSelf);
            SetPokemonStats();
        }
    }

    public void ButtonSelectNextPokemon()
    {
        selectedIndex++;

        if (selectedIndex >= pokemonPrefabs.Length)
            selectedIndex = 0;

        selectedPokemonText.text = pokemonPrefabs[selectedIndex].name;

        if (CloudAnchorObjectPlacement.Instance.spawnedObjects[selectedIndex] != null)
        {
            SetPokemonStats();
        }
    }

    public void ButtonSelectPreviousPokemon()
    {
        selectedIndex--;
        
        if (selectedIndex < 0)
            selectedIndex = pokemonPrefabs.Length - 1;

        selectedPokemonText.text = pokemonPrefabs[selectedIndex].name;

        if (CloudAnchorObjectPlacement.Instance.spawnedObjects[selectedIndex] != null)
        {
            SetPokemonStats();
        }
    }

    public void SetPokemonStats()
    {
        PokemonController currentPokemon = pokemonPrefabs[selectedIndex].GetComponent<PokemonController>();
        pokemonLevelText.text = currentPokemon.level.ToString();
        pokemonXPSlider.value = currentPokemon.experience;

        foreach (var mood in pokemonMoods)
        {
            mood.SetActive(false);
        }

        pokemonMoods[currentPokemon.mood].SetActive(true);
    }
}
