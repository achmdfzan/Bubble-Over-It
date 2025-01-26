using UnityEngine;

public class Cheat : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 BTeleportPoint;
    [SerializeField] private Vector3 NTeleportPoint;
    [SerializeField] private Vector3 MTeleportPoint;


    //[SerializeField] private bool hasCheated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (hasCheated) return;


        if(Input.GetKeyDown(KeyCode.B))
        {
            //hasCheated = true;
            player.position = BTeleportPoint;
        } else if (Input.GetKeyDown(KeyCode.N))
        {
            //hasCheated = true;
            player.position = NTeleportPoint;
        } else if (Input.GetKeyDown(KeyCode.M))
        {
            //hasCheated = true;
            player.position = MTeleportPoint;
        }
    }
}
