using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    [SerializeField] 
    private List<Neighbour> neighbours;

    public void CreateConnection(ClimbPoint point, Vector2 direction, ConnectionType connectionType, bool isTwoWay = true)
    {
        neighbours.Add(new Neighbour { point = point, direction = direction, connectionType = connectionType, isTwoWay = isTwoWay });
    }

    public Neighbour GetNeighbour(Vector2 direction)
    {
        Neighbour neighbour = null;

        if (direction.y != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.y == direction.y);

        if (neighbour == null && 
            direction.x != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.x == direction.x);

        return neighbour;
    }


    private void Awake()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].isTwoWay)
            {
                Neighbour neighbour = neighbours[i];

                neighbour.point?.CreateConnection(this, -neighbour.direction, neighbour.connectionType, neighbour.isTwoWay);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].point != null)
            {
                Neighbour neighbour = neighbours[i];

                Debug.DrawLine(transform.position, neighbour.point.transform.position, (neighbour.isTwoWay) ? Color.green : Color.gray);
            }
        }
    }
}
