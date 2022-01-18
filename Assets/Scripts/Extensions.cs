using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Extensions 
{
    public static void AddOrder(this Queue<Order> orderList, Func<RaycastHit, IEnumerable<int>> order, RaycastHit pos)
    {
        orderList.Enqueue(new Order(order, pos));
    }

    public static Quaternion rotateTowards(this Vector3 vector3, Vector3 lookTowards)
    {
        Vector3 lookDirection = (lookTowards - vector3);
            
        Quaternion lookDirectionQuat = Quaternion.LookRotation(lookDirection);
        lookDirectionQuat.x = 0;
        lookDirectionQuat.z = 0;
        return lookDirectionQuat;
    }
}
