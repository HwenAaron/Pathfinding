using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cpalgo : MonoBehaviour {


    public Map map;

    //for pathfinding
    private Dictionary<Cell, Cell> cameFrom;
    private Dictionary<Cell, float> costSoFar;
    private Dictionary<Threshold, Threshold> tcameFrom;
    private Dictionary<Threshold, float> tcostSoFar;
    private List<Cell> pathToTake;
    private Cell firstStartCell;
    private Cell finalGoalCell;

    //the goal the agent wants to move to
    public Transform goal;
    public Vector3 goalPosition; //NEED TO CHANGE




    //for movement
    public float moveSpeed = 1f;
    private int stepsTaken = 0; //aka cell in the pathToTake list
    private float Timer;
    private Cell nextCell;
    private List<Cell> visitedCells = new List<Cell>();





    void Start()
    {
        if (map == null)
        {
            map = GameObject.Find("MapManager").GetComponent<Map>();
        }


    }

    public void FindPath()
    {
        if (goal != null)
        {
            pathToTake = FindCellPath(map.CellFromWorldPos(transform.position), map.CellFromWorldPos(goal.position));
        }

    }


    private List<Cell> FindCellPath(Cell startCell, Cell goalCell)
    {


        PriorityQueue<Cell> frontier = new PriorityQueue<Cell>();
        cameFrom = new Dictionary<Cell, Cell>();
        costSoFar = new Dictionary<Cell, float>();
        List<Cell> path = null;

        frontier.Enqueue(startCell, 0);
        cameFrom[startCell] = startCell;
        costSoFar[startCell] = 0;
        int temp = 0;
        while (frontier.Count() > 0)
        {
            temp++;

            //Check if the agent is able to calculate their path.
            //if (hasHaltedCalculating){
            //    StartCoroutine(WaitUntilCanCalculate());
            //}


            //else, continue to calculate the path.
            Cell current = frontier.Deqeueue();

            if (current.Equals(goalCell))
            {
                path = RetraceCellPath(startCell, goalCell);
                return path;
            }


            foreach (Edge e in current.edgesToNeighbors)
            {
                Cell neighbor = e.incident;

                float newCost = costSoFar[current] + GetCellCost(current, neighbor);
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {
                    costSoFar[neighbor] = newCost;
                    float priority = newCost + GetCellHeuristic(neighbor, goalCell);
                    frontier.Enqueue(neighbor, priority);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return path;
    }


    /* Given the start cell and the goal cell of the agent, give calculated path 
     * using the cells that are in cameFrom.
     * Input: start location, goal location.
     * Output: the calculated path as a list of cells the agent must visit to
     * get to the goal.
     */
    private List<Cell> RetraceCellPath(Cell startCell, Cell goalCell)
    {

        //loop through the cameFrom dictionary.
        List<Cell> path = new List<Cell>();
        Cell currentCell = goalCell;
        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = cameFrom[currentCell];
        }

        path.Add(startCell);
        path.Reverse();
        return path;
    }

    private float GetCellCost(Cell source, Cell sink)
    {
        float cost = Vector3.Distance(source.worldPosition, sink.worldPosition);
        return cost;
    }

    /* Calculate the heuristic cost from one cell to the next. 
     * This is the distance from the source cell to the sink cell
     * Input: source cell, sink cell
     * Output: cost
     */
    private float GetCellHeuristic(Cell source, Cell sink)
    {
        float h = Vector3.Distance(source.worldPosition, sink.worldPosition);
        return h;
    }

    //for debugging
    private void OnDrawGizmos()
    {

        if (pathToTake != null)
        {
            foreach (Cell c in pathToTake)
            {
                Vector3 increment = new Vector3(c.cellSize / 2f, 0, -1f * c.cellSize / 2f);
                Vector3 center = c.worldPosition + increment;
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(center, new Vector3(c.cellSize, c.cellSize, c.cellSize));

            }
        }

    }
}


