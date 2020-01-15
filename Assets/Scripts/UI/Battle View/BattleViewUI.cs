using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleViewUI : MonoBehaviour {

    World.TerrainChunk chunk;
    List<Unit> enemy;
    Army selectedArmy;

    public Text enemyForces;
    public Dropdown armySelection;
    public Button start;
    public Text odds;
    public Text result;
    public Text enemyStrength;
    public Text myStrength;

    private void Awake()
    {
        armySelection.onValueChanged.AddListener(delegate { SelectArmy(Army.armies[armySelection.value]); });
    }

    public void RegisterCallbacks()
    {
        Army.RegisterOnArmyCreated(CreateArmySelection);
        Army.RegisterOnArmyDeleted(DeleteArmySelection);
    }

    private void OnDisable()
    {
        Army.UnregisterOnArmyCreated(CreateArmySelection);
        Army.UnregisterOnArmyDeleted(DeleteArmySelection);
    }

    private void Update()
    {
        if (start.enabled)
        {
            if (selectedArmy == null)
                start.enabled = false;
        }
        else
        {
            if (selectedArmy != null)
                start.enabled = true;
        }
    }

    public void Setup(World.TerrainChunk chunk)
    {
        this.chunk = chunk;
        enemy = chunk.enemies;

        if (armySelection.options.Count > 0)
        {
            armySelection.value = 0;
            SelectArmy(Army.armies[0]);
            armySelection.RefreshShownValue();
        }
        else
        {
            armySelection.captionText.text = "No army available";
            SelectArmy(null);
        }

        Dictionary<string, int> enemyCount = new Dictionary<string, int>();
        foreach(Unit enemyUnit in enemy)
        {
            if (enemyCount.ContainsKey(enemyUnit.name))
                enemyCount[enemyUnit.name]++;
            else
                enemyCount.Add(enemyUnit.name, 1);
        }

        enemyForces.text = "";
        foreach (string enemyUnit in enemyCount.Keys)
            enemyForces.text += "\n" + enemyUnit + " x " + enemyCount[enemyUnit];

        enemyStrength.text = "Strength: " + Unit.GetStrength(enemy);

        result.text = "";
    }

    void SelectArmy(Army army)
    {
        if (army != null)
        {
            selectedArmy = army;
            myStrength.text = "Strength: " + army.Strength;
            odds.text = "Win chance: " + Army.GetOdds(army, enemy) + "%";
        }
        else
        {
            selectedArmy = null;
            myStrength.text = "Strength: 0";
            odds.text = "Win chance: 0%";
        }
    }

    public void Battle()
    {
        if(Army.Battle(selectedArmy, enemy))
        {
            chunk.KillEnemies();
            result.text = "Victory!";
        }
        else
        {
            Army.DestroyArmy(selectedArmy);
            result.text = "Defeat!";
        }
    }

    public void CreateArmySelection(Army army)
    {
        armySelection.options.Add(new Dropdown.OptionData(army.name));
    }

    public void DeleteArmySelection(Army army)
    {
        armySelection.options.RemoveAt(Army.armies.IndexOf(army));
    }
}
