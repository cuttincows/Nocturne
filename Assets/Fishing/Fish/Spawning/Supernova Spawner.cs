using UnityEngine;

public class SupernovaSpawner : MonoBehaviour
{
    public GameObject trout;
    public float xOffset = 14;
    public float yOffsetMax = 5f;

    public void Spawn()
    {
        bool left = Random.value < 0.5f;
        float xOffset = this.xOffset * (left ? -1 : 1);
        float yOffset = Random.Range(-yOffsetMax, yOffsetMax);
        Vector3 pos = new Vector3(xOffset, yOffset, transform.position.z);
        GameObject newTrout = Instantiate(trout);
        newTrout.transform.position = pos;
        // move in opposite direction
        newTrout.GetComponent<MoveRight>().left = !left;
    }

    private void OnDrawGizmos()
    {
        Vector3 bl = transform.position + new Vector3(-xOffset,-yOffsetMax,0);
        Vector3 tl = transform.position + new Vector3(-xOffset, yOffsetMax, 0);
        Vector3 br = transform.position + new Vector3(xOffset, -yOffsetMax, 0);
        Vector3 tr = transform.position + new Vector3(xOffset, yOffsetMax, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(br, tr);
    }
}
