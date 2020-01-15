using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorkListUI : MonoBehaviour {

    public Button addWorker;
    public Button removeWorker;
    public Text workerCount;
    public GameObject workerTab;
    Building building;
    Dictionary<Worker, GameObject> workerTabs = new Dictionary<Worker, GameObject>();

    public void Setup(Building building)
    {
        if(this.building != null)
        {
            this.building.UnregisterOnWorkersChanged(UpdateWorkerText);
            this.building.UnregisterOnWorkerAdded(AddWorker);
            this.building.UnregisterOnWorkerRemoved(RemoveWorker);
            this.building.city.UnregisterOnUnemployedChanged(UpdateWorkerButtons);
            addWorker.onClick.RemoveAllListeners();
            removeWorker.onClick.RemoveAllListeners();
            while(workerTabs.Count > 0)
                RemoveWorker(workerTabs.First().Key);
        }

        this.building = building;
        addWorker.onClick.AddListener(delegate { building.AddWorker(); UpdateWorkerButtons(); });
        removeWorker.onClick.AddListener(delegate { building.UnemployWorker(); UpdateWorkerButtons(); });
        building.RegisterOnWorkersChanged(UpdateWorkerText);
        building.RegisterOnWorkerAdded(AddWorker);
        building.RegisterOnWorkerRemoved(RemoveWorker);
        building.city.RegisterOnUnemployedChanged(UpdateWorkerButtons);
        foreach (Worker worker in building.workers)
            AddWorker(worker);
        UpdateWorkerText(building.workers.Count);
        UpdateWorkerButtons();
    }

    void AddWorker(Worker worker)
    {
        GameObject tabGO = Instantiate(workerTab, GetComponent<BuildingInfoListUI>().content);
        tabGO.GetComponent<WorkerTabUI>().Setup(worker, building);
        workerTabs.Add(worker, tabGO);
    }

    void RemoveWorker(Worker worker)
    {
        Destroy(workerTabs[worker]);
        workerTabs.Remove(worker);
    }

    void UpdateWorkerText(int workers)
    {
        workerCount.text = workers + "/" + building.maxWorkers;
    }

    void UpdateWorkerButtons()
    {
        if (building == null)
            return;
        if (building.workers.Count > 0)
        {
            removeWorker.interactable = true;
        }
        else
        {
            removeWorker.interactable = false;
        }
        if (building.workers.Count < building.maxWorkers && building.city.unemployed > 0)
        {
            /*Debug.Log("Workers are not full and there are unemployed workers");
            Debug.Log(building.workers.Count + "/" + building.maxWorkers);
            Debug.Log(building.city.unemployed);
            Debug.Log("-----------------------------");*/
            addWorker.interactable = true;
        }
        else
        {
            /*Debug.Log("Workers are full or no unemployed workers");
            Debug.Log(building.workers.Count + "/" + building.maxWorkers);
            Debug.Log(building.city.unemployed);
            Debug.Log("-----------------------------");*/
            addWorker.interactable = false;
        }
    }
}
