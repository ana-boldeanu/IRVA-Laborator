using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PokemonGameManager : MonoBehaviour
{
    public GameObject[] pokemonPrefabs;
    public TextMeshProUGUI selectedPokemonText;

    private int selectedIndex = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetSelectedPokemon()
    {
        return pokemonPrefabs[selectedIndex];
    }

    public void ButtonSelectNextPokemon()
    {
        selectedIndex++;

        if (selectedIndex >= pokemonPrefabs.Length)
            selectedIndex = 0;

        selectedPokemonText.text = pokemonPrefabs[selectedIndex].name;
    }

    public void ButtonSelectPreviousPokemon()
    {
        selectedIndex--;
        
        if (selectedIndex < 0)
            selectedIndex = pokemonPrefabs.Length - 1;

        selectedPokemonText.text = pokemonPrefabs[selectedIndex].name;
    }
}
