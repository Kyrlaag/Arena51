using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public int health;
    private int maxHealth;
    public int damage;
    public int range;
    private float linePos;

    [SerializeField] private GameObject healthBarTick;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI heartText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI rangeText;

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.fillAmount = (float)health / maxHealth;

        if (health <= 0)
        {
            healthBar.fillAmount = 0f;
            FindObjectOfType<GameManager>().CheckForEnemies();
            Destroy(this.gameObject);
            
        }
    }

    void Awake()
    {
        maxHealth = health;
        healthBar.fillAmount = 1;
        heartText.text = health.ToString();
        attackText.text = damage.ToString();
        rangeText.text = range.ToString();

        int lineCount = (maxHealth - 1) / 5;

        linePos = -99f/2f;

        for (int i = 0; i < lineCount; i++)
        {
            linePos += (99f / (float)maxHealth * 5f);
            GameObject temp = Instantiate(healthBarTick, new Vector2(0,0), transform.rotation);
            temp.transform.SetParent(healthBar.gameObject.transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(linePos, 0);
            temp.GetComponent<RectTransform>().localScale = new Vector2(1,1);
        }
    }

    private float elapsedTime;
    [SerializeField] private float lerpTime = 1;
    public IEnumerator GetReadyForFight()
    {
        elapsedTime = 0;
        while (elapsedTime < lerpTime)
        {
            heartText.gameObject.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, elapsedTime/lerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < lerpTime)
        {
            healthBar.transform.parent.gameObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, elapsedTime/lerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        yield return null;
    }

    public void startCor()
    {
        StartCoroutine(GetReadyForFight());
    }
}
