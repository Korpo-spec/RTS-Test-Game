using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    [SerializeField]private int hp;
    public int health
    {
        get => hp;
        set
        {
            if ((hp -= value)<= 0)
            {
                Destroy(this.gameObject);
            }
            
        } 
    }
    private Order behavior;
    protected IEnumerator<int> task = null;

    public Order newBehavior;

    public readonly Queue<Order> unitOrders = new Queue<Order>();

    protected NavMeshAgent navMeshAgent;
    protected Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if(newBehavior != null)
        {
            
            behavior = newBehavior;
            task = newBehavior.unitOrder(newBehavior.hitInfo).GetEnumerator();
            newBehavior = null;
            ClearAnimation();

        }
        
        
        if(task == null || !task.MoveNext()) 
        {
            
            if (unitOrders.Count > 0)
            {
                Order order = unitOrders.Dequeue();
                task = order.unitOrder(order.hitInfo).GetEnumerator();
            }
            /*
            else if (behavior != null)
            {
                task = behavior.unitOrder(behavior.pos).GetEnumerator();
                behavior = null;
            }
            */
            else
            {
                RaycastHit hit = new RaycastHit();
                task = Idle(hit).GetEnumerator();
            }
            ClearAnimation();
            
            
            
            

        }
       
        animator.SetFloat("MovementSpeed", Mathf.Abs(navMeshAgent.velocity.magnitude));
        
        
    }

    
    protected IEnumerable<int> Idle(RaycastHit _)
    {
        
        navMeshAgent.isStopped = true;
        while (true)
        {
            yield return 0;
        }
    }
    

    private Vector3 vectorToMoveTo;
    protected virtual IEnumerable<int> Move(RaycastHit mousePos)
    {
        
        navMeshAgent.destination = mousePos.point;
        navMeshAgent.isStopped = false;
        while(Vector3.Distance(this.transform.position, mousePos.point) > 2.0f)
        {
            yield return 0;
        }
        
    }

    [SerializeField] private float detectionRange = 2f;
    [SerializeField] private bool useAnimationattackSpeed;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackSpeed = 0.5f;
    [SerializeField] private int attackDamage = 5;
    
    protected IEnumerable<int> AttackMove(RaycastHit hitinfo)
    {
        Vector3 enemyLastPos = hitinfo.point;
        Debug.Log("AttackMove");
        while (true)
        {
            IEnumerable<GameObject> gameObjects = GameObject.FindGameObjectsWithTag("Faction1");
            
            Unit unitToAttackScript = null;
            GameObject unitToAttack = null;
            if (gameObjects.Any())
            {
                Debug.Log("AttackMoveTryingToFind");
                gameObjects = gameObjects.Where(e => Vector3.Distance(transform.position, e.transform.position) < detectionRange);
                if (gameObjects.Any())
                {
                    unitToAttack = gameObjects.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
                    unitToAttackScript = unitToAttack.GetComponent<Unit>();
                    navMeshAgent.destination = unitToAttack.transform.position;
                    Debug.Log("AttackMoveFound");
                    enemyLastPos = unitToAttack.transform.position;
                }
                else
                {
                    navMeshAgent.destination = enemyLastPos;
                }
                
            }
            else
            {
                navMeshAgent.destination = enemyLastPos;
            }

            navMeshAgent.isStopped = false;
            bool attacked = false;
           
            while (unitToAttack != null)
            {
                if (Vector3.Distance(transform.position, unitToAttack.transform.position)> detectionRange)
                {
                    unitToAttack = null;
                    enemyLastPos = hitinfo.point;
                    break;
                }
                navMeshAgent.destination = unitToAttack.transform.position;
                if (Vector3.Distance(transform.position,unitToAttack.transform.position) < attackRange)
                {
                    navMeshAgent.isStopped = true;
                    animator.SetBool("AttackB", true);
                    transform.rotation = transform.position.rotateTowards(unitToAttack.transform.position);
                    
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && 
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.35f && 
                        animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                    {
                        if (!attacked)
                        {
                            Debug.Log("Attacked!!!");
                            attacked = true;
                            unitToAttackScript.health = attackDamage;
                            
                        }
                    }
                    else
                    {
                        if (attacked)
                        {
                            animator.SetBool("AttackB", false);
                        }
                        attacked = false;
                    }
                }
                else
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        navMeshAgent.isStopped = false;
                    }
                }
                yield return 0;
            }
            animator.SetBool("AttackB", false);

            
            yield return 0;
        }
        
        
    }
    /*
    public virtual void MoveTo(Vector3 moveTo)
    {
        //newBehavior = new Order(Move, moveTo);
        vectorToMoveTo = moveTo;
    }

    public virtual void MoveTo(RaycastHit hitinfo)
    {
        newBehavior = new Order(Move, hitinfo);
        vectorToMoveTo = hitinfo.point;
    }
    */
    private void ClearAnimation()
    {
        AnimatorControllerParameter[] parameter = animator.parameters;
        animator.SetBool("AttackB", false);
        animator.SetBool("Gathering", false);
        
    }

    public virtual void Action(RaycastHit mousePos)
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("Gathering", false);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                unitOrders.AddOrder(Move, mousePos);
            }
            else
            {
                
                newBehavior = new Order(Move, mousePos);
            }

            if (Input.GetKey(KeyCode.V))
            {
                newBehavior = new Order(AttackMove, mousePos);
            }
            
        }
        

    }
}
