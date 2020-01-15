using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkerTabUI : MonoBehaviour {

    public Text nameText;
    public Dropdown workOrders;
    public Text descriptionText;
    public Text toolText;
    Worker worker;
    Building building;

	public void Setup(Worker worker, Building building)
    {
        this.worker = worker;
        this.building = building;
        nameText.text = worker.name;
        workOrders.options = new List<Dropdown.OptionData>();
        foreach (WorkOrder work in building.workOrders)
            workOrders.options.Add(new Dropdown.OptionData(work.description));
        workOrders.onValueChanged.AddListener(delegate { SetWork(building.workOrders[workOrders.value]); });
        workOrders.value = building.GetWorkOrderIndex(worker.work);
        workOrders.RefreshShownValue();
        SetWork(building.workOrders[workOrders.value]);
    }

    void SetWork(WorkOrder work)
    {
        descriptionText.text = work.GetDescription();
        toolText.text = work.toolTier == -1 ? "Doesn't require a tool." : "Requires a tier " + work.toolTier + " tool or better.";
        if(building.GetWorkOrderIndex(worker.work) != building.GetWorkOrderIndex(work))
            worker.SetWork(work);
    }
}
