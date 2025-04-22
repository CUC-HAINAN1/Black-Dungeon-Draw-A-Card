using UnityEngine;

public class BossRoomTipTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) {

            TipManager.Instance.ShowTip("前方有强大魔物的气息...", 3f);
            GetComponent<Collider2D>().enabled = false;

        }

    }

}
