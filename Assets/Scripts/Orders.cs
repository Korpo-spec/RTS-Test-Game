using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Order
{
    public Order(Func<RaycastHit, IEnumerable<int>> order, RaycastHit pos)
    {
        unitOrder = order;
        this.hitInfo = pos;
    }

    public Order(Func<RaycastHit, IEnumerable<int>> order)
    {
        unitOrder = order;
    }
    public Func<RaycastHit, IEnumerable<int>> unitOrder;
    public RaycastHit hitInfo;
    public Vector3 pos => hitInfo.point;
}
