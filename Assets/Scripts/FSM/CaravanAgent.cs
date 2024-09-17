using UnityEngine;

public class CaravanAgent : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private FSM<Directions, Flags> fsm;

    void Start()
    {
        InitFSM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitFSM() 
    {
        fsm = new FSM<Directions, Flags>();

        //TODO: Add states and transitions.

        fsm.ForceTransition(Directions.Wait);
    }

    //TODO: Add parameters for the transitions.
}
