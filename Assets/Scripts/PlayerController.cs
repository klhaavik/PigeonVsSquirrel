using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int moveSpeed;
    public int hitStrength;
    public int carHitStrength;
    public float smoothing = 0.1f;
    public bool playerOne;
    public GameObject opponent;
    Rigidbody rb;
    bool stunned = false;
    Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    bool NormalAttackKeyPressed(){
        return playerOne ? Input.GetKeyDown("g") : Input.GetKeyDown("o");
    }

    bool SpecialAttackKeyPressed(){
        return playerOne ? Input.GetKeyDown("h") : Input.GetKeyDown("p");
    }

    // Update is called once per frame
    void Update()
    {
        if(stunned){
            return;
        }
        
        //movement
        velocity = playerOne ? 
            new Vector3(Input.GetAxisRaw("SquirrelHorizontal"), 0, Input.GetAxisRaw("SquirrelVertical")):
            new Vector3(Input.GetAxisRaw("PigeonHorizontal"), 0, Input.GetAxisRaw("PigeonVertical"))
        ;
        
        rb.velocity = velocity * moveSpeed;
        
        //movement with AddForce(), kinda bad tho
        /*if(pigeon){
            rb.AddForce(new Vector3(
                Input.GetAxisRaw("PigeonHorizontal"), 
                0, 
                Input.GetAxisRaw("PigeonVertical")
            ) * moveSpeed * 10 * Time.deltaTime, ForceMode.Impulse); 
        }else{
            rb.AddForce(new Vector3(
                Input.GetAxisRaw("SquirrelHorizontal"), 
                0, 
                Input.GetAxisRaw("SquirrelVertical")
            ) * moveSpeed * 10 * Time.deltaTime, ForceMode.Impulse);
        }*/

        //smoothly limit (lerp) velocity to maxSpeed
        if(rb.velocity.magnitude > moveSpeed){
            Vector3 desiredVelocity = Vector3.Normalize(rb.velocity) * moveSpeed; 
            //Vector3 smoothedVelocity = Vector3.Lerp(rb.velocity, desiredVelocity, smoothing);
            rb.velocity = desiredVelocity;
        }



        //if close enough to opponent, perform attack
        Vector3 distance = opponent.transform.position - transform.position;
        if(NormalAttackKeyPressed() && distance.magnitude < 2f){
            opponent.GetComponent<Rigidbody>().AddForce((opponent.transform.position - transform.position) * hitStrength + transform.up * (hitStrength / 2), ForceMode.Impulse);
        }
        if(SpecialAttackKeyPressed() && distance.magnitude < 3f){
            opponent.GetComponent<Rigidbody>().AddForce((opponent.transform.position - transform.position) * hitStrength * 5 + transform.up * hitStrength * 2, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision col){
        int layer = col.gameObject.layer;
        
        if(layer == 6){ //if colliding with a car, send player flying
            rb.AddForce(col.gameObject.transform.forward * carHitStrength + gameObject.transform.up * (carHitStrength / 2), ForceMode.Impulse);
            stunned = true;
        }else if(layer == 8){ //if colliding with the road, regain control of movement
            stunned = false;
        }else if(layer == 3){
            
        }
    }
}
