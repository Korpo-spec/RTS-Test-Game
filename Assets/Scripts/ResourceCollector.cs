using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System;

public class ResourceCollector : Unit
{
    public enum myEnum // your custom enumeration
    {
        Lumber, 
        Miner, 
        Fisher
    };
    public myEnum dropDown = myEnum.Lumber;
    // Start is called before the first frame update
    
    private string resourceTag;
    private int lumberCarried = 0;
    private int inventorySize = 6;
    
    // Start is called before the first frame update
    void Start()
    {
        newBehavior = new Order(Idle);
        
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        SetType(dropDown);
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
        base.Update();
        
        //while((task == null || !task.MoveNext())&& tries < 3) {task= behavior().GetEnumerator(); tries++;}
        
        
        
        
        
        
    }
    private IEnumerable<GameObject> NearestByTag(string resourceTag) 
    {
        
        return (IEnumerable<GameObject>)GameObject.FindGameObjectsWithTag(resourceTag).OrderBy(t => Vector3.Distance(t.transform.position, this.gameObject.transform.position));
    }

    private IEnumerable<int> ApproachResource(RaycastHit pos)
    {
        IEnumerable<GameObject> resourceList = NearestByTag(resourceTag);

        IEnumerator<GameObject> resourceEnumerator = resourceList.Where(e => e.GetComponent<ResourceScript>().Collector == null || e.GetComponent<ResourceScript>().Collector == this.gameObject).GetEnumerator();

        if(resourceEnumerator.MoveNext())
        {
            
            GameObject resource = resourceEnumerator.Current;
            
            resource.GetComponent<ResourceScript>().Collector = (GameObject)this.gameObject;
            
            navMeshAgent.destination = resource.transform.position;
            navMeshAgent.isStopped = false;
            
            while(Vector3.Distance(this.transform.position, resource.transform.position) > 1.0f)
            {
                //Vector3 dir = (tree.transform.position - this.transform.position).normalized;
                //this.transform.Translate(dir * 2 * Time.deltaTime);
                yield return 0;
            }
            navMeshAgent.isStopped = true;
            
            newBehavior = new Order(MineResource);
        }
        else newBehavior = new Order(CarryToStorage);

    }

    private IEnumerable<int> ApproachSpecificResource(RaycastHit pos)
    {
        
        navMeshAgent.destination = pos.point;
        navMeshAgent.isStopped = false;
        
        
        while(Vector3.Distance(this.transform.position, resourceLocation) > 1.0f)
        {
            //Vector3 dir = (tree.transform.position - this.transform.position).normalized;
            //this.transform.Translate(dir * 2 * Time.deltaTime);
            yield return 0;
        }
        navMeshAgent.isStopped = true;
        newBehavior = new Order(MineResource);
    }

    private IEnumerable<int> MineResource(RaycastHit pos)
    {
        Debug.Log("MINING");
        IEnumerable<GameObject> resourceList = NearestByTag(resourceTag);
        
        IEnumerator<GameObject> resourceEnumerator = resourceList.Where(e => e.GetComponent<ResourceScript>().Collector == null || this.gameObject).GetEnumerator();
        
        resourceEnumerator.MoveNext();
        
        GameObject resource = resourceEnumerator.Current;
        
        ResourceScript treeScript = resource.GetComponent<ResourceScript>();
        
        animator.SetBool("Gathering", true);
        
        
        float chopTimer = 1.0f;
        
        while(treeScript.health > 0 && lumberCarried < inventorySize)
        {
            Vector3 lookDirection = (resource.transform.position - transform.position);
            
            Quaternion lookDirectionQuat = Quaternion.LookRotation(lookDirection);
            lookDirectionQuat.x = 0;
            lookDirectionQuat.z = 0;
            transform.rotation = lookDirectionQuat;
            
            if(chopTimer > Time.deltaTime)chopTimer -= Time.deltaTime;
            else
            {
                treeScript.health--; 
                lumberCarried += treeScript.LumberInTree;
                chopTimer= 1.0f+ (Time.deltaTime - chopTimer);
            }
            yield return 0;
        }

        
        animator.SetBool("Gathering", false);
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Collecting"))
        {
            yield return 0;
        }
        Debug.Log("intheend");
        newBehavior = new Order(lumberCarried < inventorySize ? (Func<RaycastHit, IEnumerable<int>>)ApproachResource : CarryToStorage);
        if(treeScript.health <= 0)treeScript.OnCollected();
        
        
    }

    private IEnumerable<int> CarryToStorage(RaycastHit pos)
    {
        IEnumerable<GameObject> resourceList = NearestByTag(resourceTag + "Storage");
        //resourceList.Where(e => e.GetComponent<ResourceScript>().Collector == null);
        IEnumerator<GameObject> resourceEnumerator = resourceList.GetEnumerator();
        
        resourceEnumerator.MoveNext();
        
        GameObject storage = resourceEnumerator.Current;
        NavMeshPath path =  new NavMeshPath();
        navMeshAgent.destination = storage.transform.position;
        navMeshAgent.isStopped = false;

        while(Vector3.Distance(this.transform.position, storage.transform.position) > 1.0f)
        {
            
            yield return 0;
        }
        storage.GetComponent<WoodStorage>().woodStored += lumberCarried;
        lumberCarried = 0;

        newBehavior = new Order(ApproachResource);
        
    }

    private void SetType(myEnum type)
    {
        switch (type)
        {
            case myEnum.Lumber:
                resourceTag = "Tree";
                break;
            case myEnum.Miner:
                resourceTag = "Mine";
                break;
            case myEnum.Fisher:
                resourceTag = "Fish";
                break;
        }
    }
    private Vector3 resourceLocation;

    protected override IEnumerable<int> Move(RaycastHit hitInfo)
    {
        IEnumerator<int> baseMove = base.Move(hitInfo).GetEnumerator();
        if (hitInfo.transform.gameObject.TryGetComponent<ResourceScript>(out ResourceScript resourceScript))
        {
            
            newBehavior = new Order(ApproachSpecificResource,hitInfo);
            resourceLocation = hitInfo.transform.gameObject.transform.position;
        }
        else
        {
            while (baseMove.MoveNext())
            {
                yield return 0;

            }
        }
       
        
    }
    /*
    public override void MoveTo(RaycastHit hitInfo)
    {
        if(hitInfo.transform.gameObject.TryGetComponent<ResourceScript>(out ResourceScript resourceScript))
        {
            
        }
        else
        {
            base.MoveTo(hitInfo);
        }
    }
    */

    
}
