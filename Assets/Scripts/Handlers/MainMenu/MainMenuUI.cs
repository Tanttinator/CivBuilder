using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject mainMenuLayer;
    public GameObject newGameLayer;
    public GameObject loadGameLayer;

    public InputField seed;
    public InputField nameInput;
    public Button startGameButton;
    World world;

    public void OpenNewGameLayer()
    {
        mainMenuLayer.SetActive(false);
        newGameLayer.SetActive(true);
        loadGameLayer.SetActive(false);
        seed.text = World.seed.ToString();
        nameInput.text = null;
        GenerateRandomPreview();
    }

    public void CloseNewGameLayer()
    {
        OpenMenuLayer();
        World.instance.GenerateMenuBackgorund();
    }

    public void OpenMenuLayer()
    {
        mainMenuLayer.SetActive(true);
        newGameLayer.SetActive(false);
        loadGameLayer.SetActive(false);
    }

    public void OpenLoadGameLayer()
    {
        mainMenuLayer.SetActive(false);
        newGameLayer.SetActive(false);
        loadGameLayer.SetActive(true);
        loadGameLayer.GetComponent<LoadGameUI>().Show();
    }

    public void GenerateRandomPreview()
    {
        World.SetSeed(Random.Range(-999999, 999999));
        seed.text = World.seed.ToString();
        world.GeneratePreview();
    }

    public void StartNewGame()
    {
        GameController.OpenGame(GameController.NewGame(nameInput.text));
    }

    public void ExitGame()
    {
        GameController.ExitGame();
    }

    private void Awake()
    {
        world = FindObjectOfType<World>();

        seed.onEndEdit.AddListener(delegate { int num; if (int.TryParse(seed.text, out num)) { World.SetSeed(num); world.GeneratePreview(); }; });

        mainMenuLayer.SetActive(true);
        newGameLayer.SetActive(false);
        loadGameLayer.SetActive(false);
    }

    private void Update()
    {
        if (startGameButton.enabled)
        {
            if (nameInput.text == null || nameInput.text.Length == 0)
                startGameButton.enabled = false;
        }
        else
        {
            if (nameInput.text != null && nameInput.text.Length > 0)
                startGameButton.enabled = true;
        }
    }
}
