using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerScript : NetworkBehaviour
{
    public GameObject Bullet;


    public UnityEngine.UI.Slider Slider;

    // Denne variable skal kunne synces til andre klienter.
    [SyncVar]
    private int Health = 100;
    // Use this for initialization
    void Start()
    {
     
    }

 

    // Update is called once per frame
    void Update()
    {

        // Alle objekterne, værende mit eller andre spillere vil jeg have til at update dens health value.
        Slider.value = Health;

        // Hvis dette objekt er mit så...
        // Kan jeg styre det
        if (isLocalPlayer)
        {
       
            Rigidbody2D Rb = GetComponent<Rigidbody2D>();
            Rb.AddForce(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))*Time.deltaTime*1000);

            if (Input.GetMouseButtonDown(0))
            {
                //Spawn
                // Sender spawnposition info fra mig (klienten)
                CmdSpawn(Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 5);



            }
        }

    }

    public void TakeDamage(int Amount)
    {
        //Et objekt rører en bullet så dette script kører
        //Men jeg skal ikke trække noget liv fra hvis jeg ikke er serveren
        // Fordi serveren updater den [syncvar]'s data når den bliver ændret.
        // På den måde bliver objektet opdateret på alle klienter når det sker.

        if(!isServer)
        {
            // Hvis det ikke er serveren så lad vær.
            return;
        }

        // Det er serveren som tjekker det så...
        Health -= Amount;
        // Serveren kan så sende en rpc command ud til alle klienterne. I dette tilfælde er det bare et print til alle klienter.
        RpcDamage(Amount);

        

    }

    [ClientRpc]
    void RpcDamage(int amount)
    {

        Debug.Log("Took damage:" + amount + " Health left: "+Health);
    }



    [Command]
    void CmdSpawn(Vector3 Pos)
    {
        // Command er sendt direkte til serveren og dette kode køres på server computeren.
        // Alle argumenter vil blive modtaget til funktionen.
        GameObject tmp = Instantiate(Bullet) as GameObject;
        // Så dens position i dette tilfælde er ikke clientens input.mouseposition, men serverens.
        //tmp.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 5;
        // Det er derfor jeg nu sender den med i et argument.

        tmp.transform.position = Pos;

        // Server får nu at vide at den skal spawn og spawner den. Da det er serveren som spawner så sender den ud til alle klienter at den har spawnet.
        // Selve objektet kan så have et sync objekt på for at synce med resten af spillet.
        NetworkServer.Spawn(tmp);
    }
}
