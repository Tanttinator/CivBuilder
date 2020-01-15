using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResourceViewUI : MonoBehaviour {

    public Transform menu;
    public Transform listParent;
    public GameObject selector;
    public GameObject list;
    public GameObject resourceTab;

    Dictionary<ResourceType, GameObject> selectors = new Dictionary<ResourceType, GameObject>();
    Dictionary<ResourceType, GameObject> lists = new Dictionary<ResourceType, GameObject>();

    Transform activeList;

    public void RegisterCallbacks()
    {
        ResourcePool.RegisterOnResourceAdded(CreateResource);
    }

    public void PreStart()
    {
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            CreateResourceType(type);
            CloseList(lists[type].transform);
        }
        OpenList(lists.First().Value.transform);
    }

    private void OnDisable()
    {
        foreach (GameObject go in selectors.Values)
            Destroy(go);
        selectors.Clear();

        foreach (GameObject go in lists.Values)
            Destroy(go);
        lists.Clear();
        ResourcePool.UnregisterOnResourceAdded(CreateResource);
    }

    void CreateResourceType(ResourceType type)
    {
        GameObject buttonGO = Instantiate(selector, menu);
        Button button = buttonGO.GetComponent<Button>();
        buttonGO.GetComponentInChildren<Text>().text = type.ToString();
        GameObject listGO = Instantiate(list, listParent);
        button.onClick.AddListener(delegate { CloseActiveList(); OpenList(listGO.transform); });
        selectors.Add(type, buttonGO);
        lists.Add(type, listGO);
    }

    void CreateResource(Resource resource)
    {
        Instantiate(resourceTab, lists[resource.type].GetComponent<ResourceListUI>().content).GetComponent<ResourceTabUI>().Setup(resource);
    }

    void OpenList(Transform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
        activeList = trans;
    }

    void CloseList(Transform list)
    {
        list.localScale = new Vector3(0, 0, 0);
    }

    void CloseActiveList()
    {
        if (activeList == null)
            return;

        CloseList(activeList);
        activeList = null;
    }
}
