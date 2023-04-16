using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject blockadeRow;
    [SerializeField] private GameObject powerUp;
    [SerializeField] private int basePowerUpChance = 5;

    private GameObject instatiatedBlockadeRow;

    // Start is called before the first frame update
    private void Awake()
    {
        Messenger.AddListener(GameEvent.BLOCKADE_POWERUP_COLLECTED, OnBlockadePowerCollected);
        Messenger<Brick>.AddListener(GameEvent.BRICK_DESTROYED, OnBrickDestroyed);
        Messenger.AddListener(GameEvent.MULTIBALL_POWERUP_COLLECTED, OnMultiballCollected);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.BLOCKADE_POWERUP_COLLECTED, OnBlockadePowerCollected);
        Messenger<Brick>.RemoveListener(GameEvent.BRICK_DESTROYED, OnBrickDestroyed);
        Messenger.RemoveListener(GameEvent.MULTIBALL_POWERUP_COLLECTED, OnMultiballCollected);
    }
    void OnMultiballCollected()
    {
        StartCoroutine("multiball");
    }

    IEnumerator multiball()
    {
        Instantiate(ballPrefab, new Vector2(5, 0), Quaternion.identity);
        yield return new WaitForSeconds(3.5f);
        Instantiate(ballPrefab, new Vector2(5, 0), Quaternion.identity);
    }


    private void OnBrickDestroyed(Brick brick)
    {
        int randPercentile = Random.Range(0, 100);
        int powerUpChance = basePowerUpChance * (brick.value / 20);
        if (randPercentile < powerUpChance) {
            Instantiate(powerUp, brick.gameObject.transform.position, Quaternion.identity);
        }
        Debug.Log("percentile: " + randPercentile);
        Debug.Log("powerUpChance: " + powerUpChance);
    }
    private void OnBlockadePowerCollected()
    {
        //if we've already collected this power up destroy the blockRow before we make a fresh one.
        if (instatiatedBlockadeRow != null)
        {
            Destroy(instatiatedBlockadeRow);
        }
        //create a blockade row
        instatiatedBlockadeRow = Instantiate(blockadeRow);
    }
}
