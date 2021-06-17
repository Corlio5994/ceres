using UnityEngine;

public class Player : Entity {
    public override void Stop () {
        base.Stop ();
        PacketSender.PlayerMoved (transform.position);
    }
}