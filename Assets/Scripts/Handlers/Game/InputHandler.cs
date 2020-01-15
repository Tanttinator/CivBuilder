using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InputHandler : MonoBehaviour {

    public static Transform cam;
    CameraMovement camMovement;
    BuildingHandler bh;
    UIHandler UI;
    ChunkViewUI chunkView;
    WindowUI chunkViewWindow;
    public static LayerMask terrainMask;
    bool isDragging = false;
    bool buildRoad = false;
    bool isBuildingRoad = false;
    Vector3 roadStart;
    public static EscStack escActionStack = new EscStack();

    private void Awake()
    {
        cam = Camera.main.transform;
        camMovement = cam.GetComponent<CameraMovement>();
        bh = FindObjectOfType<BuildingHandler>();
        UI = FindObjectOfType<UIHandler>();
        chunkView = FindObjectOfType<ChunkViewUI>();
        chunkViewWindow = chunkView.GetComponent<WindowUI>();
        terrainMask = LayerMask.GetMask("Terrain");
    }

    void Update () {
        
        //WASD Movement
        if (Input.GetKey(KeyCode.W))
        {
            if (chunkViewWindow.isOpen)
                chunkView.Move(new Vector2(0, -1));
            else
                camMovement.MoveForward();
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (chunkViewWindow.isOpen)
                chunkView.Move(new Vector2(0, 1));
            else
                camMovement.MoveBack();
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (chunkViewWindow.isOpen)
                chunkView.Move(new Vector2(1, 0));
            else
                camMovement.MoveLeft();
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (chunkViewWindow.isOpen)
                chunkView.Move(new Vector2(-1, 0));
            else
                camMovement.MoveRight();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            bh.RotateGhostRight();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            bh.RotateGhostLeft();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {

            }
            else if (bh.isPlacingBuilding)
            {
                if (IsMouseOverTerrain())
                    bh.PlaceBuilding();
                else
                    bh.CancelBuild();
            }
            else if (buildRoad)
            {
                if (IsMouseOverTerrain())
                {
                    roadStart = GetMousePos().point;
                    isBuildingRoad = true;
                }
            }
            else if(GetBuildingUnderMouse() != null)
            {
                FindObjectOfType<BuildingInfoUI>().Show(InputHandler.GetBuildingUnderMouse());
            }
            else
            {
                StartDrag();
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if(isDragging)
                camMovement.DragMove();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
                EndDrag();
            else if (isBuildingRoad)
            {
                isBuildingRoad = false;
                if (IsMouseOverTerrain())
                {
                    Vector3 roadEnd = GetMousePos().point;
                    BuildingHandler.instance.BuildRoad(new Vector2(roadStart.x, roadStart.z), new Vector2(roadEnd.x, roadEnd.z));
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            camMovement.DragRotate();
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            camMovement.Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (escActionStack.Count > 0)
            {
                escActionStack.Call();
            }
            else if (UI.isMenuOpen)
            {
                UI.CloseMenu();
            }
            else
            {
                UI.OpenMenu();
            }
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            GameController.debug = !GameController.debug;
        }
    }

    void StartDrag()
    {
        isDragging = true;
    }

    void EndDrag()
    {
        isDragging = false;
    }

    public static RaycastHit GetMousePos()
    {

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out hit, Mathf.Infinity, terrainMask.value);
        return hit;
    }

    public static bool IsMouseOverTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, Mathf.Infinity, terrainMask.value);
    }

    public static Building GetBuildingUnderMouse()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
            return Building.buildingGOMap[hit.transform.gameObject];
        return null;
    }


    public class EscStack
    {
        List<Action> actions;
        int last;

        public int Count
        {
            get
            {
                return last + 1;
            }
        }

        public EscStack()
        {
            last = -1;
            actions = new List<Action>();
        }

        public void Add(Action action)
        {
            if (!actions.Contains(action))
            {
                actions.Add(action);
                last++;
            }
        }

        public void Call()
        {
            if (last >= 0) {
                Action action = actions[last];
                actions.RemoveAt(last);
                last--;
                action();
            }
        }

        public void Remove(Action action)
        {
            if (actions.Contains(action))
            {
                actions.Remove(action);
                last--;
            }
        }
    }
}
