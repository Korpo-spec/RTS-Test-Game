using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

public class PlayerHandler : MonoBehaviour
{

    
    GameObject selectedGameobject;
    private float time;

    [SerializeField]
    private float timeUntilMouseHeldDown;

    [SerializeField] private LayerMask UnitMask;
    private Vector2 startPointRectSelection;
    private Vector2 endPointRectSelection;
    private GameObject SelectionSquare;
    private bool MouseIsHeld = false;
    private HashSet<GameObject> Selectedstuff = new HashSet<GameObject>();

    private List<GameObject> objectsInSelection;
    // Start is called before the first frame update
    void Start()
    {
        SelectionSquare = GameObject.Find("SelectionSquare").gameObject;
        objectsInSelection = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (Input.GetMouseButtonDown(0))
        {
            time = 0;
            
            
            startPointRectSelection = Input.mousePosition;
            
            
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000, UnitMask);
            if (hit) 
            {
                
                SelectUnit(hitInfo.transform.gameObject);
                
            }
        }
        if(Input.GetMouseButton(0))time += Time.deltaTime;

        if(time > timeUntilMouseHeldDown)
        {
            SelectionSquare.SetActive(true);
            if(!MouseIsHeld) SelectionSquare.transform.position = startPointRectSelection;
            endPointRectSelection = Input.mousePosition;
            RectTransform rectTransform = SelectionSquare.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Mathf.Abs(endPointRectSelection.x - startPointRectSelection.x),  Mathf.Abs(endPointRectSelection.y - startPointRectSelection.y));
            rectTransform.localScale = new Vector3(
                (endPointRectSelection.x - startPointRectSelection.x < 0) ? -1 : 1,
                (endPointRectSelection.y - startPointRectSelection.y < 0) ? -1 : 1,
                0);

            
            MouseIsHeld = true;
        }
        else
        {
            SelectionSquare.SetActive(false);
        }
        if(Input.GetMouseButtonUp(0)&& MouseIsHeld)
        {
            time = 0;
            MouseIsHeld = false;
            

            


            int differenceX = (int)Mathf.Abs(endPointRectSelection.x - startPointRectSelection.x);
            int differenceY = (int)Mathf.Abs(endPointRectSelection.y - startPointRectSelection.y);
            
            float startX = (endPointRectSelection.x - startPointRectSelection.x > 0)
                ? (int)startPointRectSelection.x
                : (int)endPointRectSelection.x;
            float startY = (endPointRectSelection.y - startPointRectSelection.y > 0)
                ? (int)startPointRectSelection.y
                : (int)endPointRectSelection.y;
            objectsInSelection.Clear();
            for(int x = 0; x < differenceX; x+=10)
            {
                for(int y = 0; y < differenceY; y+=10)
                {
                    
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(startX + x, startY + y, 0)), out RaycastHit hitInfo , 1000, UnitMask);
                    if(hit) objectsInSelection.Add(hitInfo.transform.gameObject);
                }
            }

            SelectUnit(objectsInSelection);


        }

        if (Input.anyKeyDown)
        {
            
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            //Debug.DrawRay(Camera.main.ScreenPointToRay(Input.mousePosition).origin, Camera.main.ScreenPointToRay(Input.mousePosition).direction * 1000, Color.green, 3);
            if (hit) 
            {
                
                
                foreach (var selected in Selectedstuff)
                {
                    Unit unit;
                    if(selected.TryGetComponent<Unit>(out unit))
                    {
                        
                        unit.Action(hitInfo);
                    
                    }
                   
                }
                
            }

        }
    }

    private void SelectUnit(GameObject selectedGameObject)
    {
        DeselectAll();
       
        
        
        Selectedstuff.Add(selectedGameObject);
        
        
    }

    private void SelectUnit(List<GameObject> selectedGameObjects)
    {
        DeselectAll();
        foreach (var selectedObj in selectedGameObjects)
        {
            Selectedstuff.Add(selectedObj);
            
        }
    }

    private void AddToSelection(GameObject selectedGameObject)
    {
        
        
        Selectedstuff.Add(selectedGameObject);
        
        
        
    }

    private void DeselectUnit()
    {
        
    }

    private void DeselectAll()
    {
        Selectedstuff.Clear();
        
    }
}
