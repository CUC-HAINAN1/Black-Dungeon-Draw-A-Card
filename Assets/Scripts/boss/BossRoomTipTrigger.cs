using UnityEngine;

public class BossRoomTipTrigger : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")) {

            TipManager.Instance.ShowTip("墨室深处，藏魇之主，施主小心", 3f);
            GetComponent<Collider2D>().enabled = false;

        }

    }

}
