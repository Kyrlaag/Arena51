using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject spriteSelectedGrid;

    private GameObject selectedAlly;
    private bool dragging = false;
    private Vector2 selectedAllyStartPos;

    private GameObject selectedGrid;
    private GameObject oldSelectedGrid;

    public bool canInteract = true;
    void Start()
    {
        //�zerine enemy koyulmu� olan gridleri kontrol etme
        CheckGrids();
    }

    
    void Update()
    {
        if (canInteract == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // select ally
                
                AllySelection(true);
            }
            if (dragging == true && selectedAlly != null)
            {
                // ally drag
                AllyDrag();

            }
            if (Input.GetMouseButtonUp(0))
            {
                // deselect ally
                
                AllySelection(false);
            }
        }
        
    }
    private void AllySelection(bool select)
    {
        //Select i�leminin yap�lmas�
        if (select == true)
        {
            //ally birimine t�kland� m�
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Ally"));
            if (hit.collider != null)
            {
                
                //se�ili ally olarak atama, ba�lang�� pozisyonunu kaydetme ve s�r�klemeyi aktif etme
                selectedAlly = hit.collider.gameObject;
                selectedAllyStartPos = selectedAlly.transform.position;
                FindObjectOfType<AudioManager>().Play("drag");
                dragging = true;

                //se�ili ally'�n en �nde g�z�kmesini sa�lama
                selectedAlly.GetComponent<SpriteRenderer>().sortingOrder = 20;

                //zaten bir grid �zerinde ise gridi kullan�ma a�ma ve gerekti�inde kullanmak i�in gridi de�i�ken olarak tutma
                hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Grid"));
                if (hit.collider != null)
                {
                    oldSelectedGrid = hit.collider.gameObject;
                    hit.collider.gameObject.tag = "Grid";
                }
                else
                    oldSelectedGrid = null;
            }
        }
        //Deselect i�leminin yap�lmas�
        else if(selectedAlly != null)
        {
            
            //�e�ili ally'� en �nde g�r�nmesini iptal etme
            selectedAlly.GetComponent<SpriteRenderer>().sortingOrder = 15;
            
            //bir grid �zerinde mi b�rak�ld�
            if (selectedGrid != null)
            {
                //b�rak�ld��� gridin pozisyonunu alma, hangi grid �zerinde bulundu�unu belli eden sprite�n inaktif yap�lmas�
                //ve �zerinde bulundu�umuz gride tekrar ba�ka bir ally koyulmamas� i�in gridin tag�n� de�i�tirme
                selectedAlly.transform.position = selectedGrid.transform.position;
                spriteSelectedGrid.SetActive(false);
                selectedGrid.tag = "Untagged";
                
            }
            //grid d���nda bir yerde mi b�rak�ld�
            else
            {
                //grid d���nda bir yere b�rak�ld��� i�in ally�n al�nmadan �nceki konumuna b�rakma, se�ili ally'� iptal etme
                selectedAlly.transform.position = selectedAllyStartPos;
                selectedAlly = null;

                if (oldSelectedGrid != null) 
                    oldSelectedGrid.tag = "Untagged";
            }
            FindObjectOfType<AudioManager>().Play("drop");
            dragging = false;
        }
    }

    private void AllyDrag()
    {
        //se�ili ally'�n mouse pozisyonunu takip etmesi
        Vector2 dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        selectedAlly.transform.position = dragPos;

        //�st�nde durulan gridi kontrol edicek olan metot
        GridSelection();
    }

    private void GridSelection()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Grid"));

        //Kullan�labilir bir grid �zerinde ise sprite ile belli et ve se�ili grid olarak ata
        if (hit.collider != null && hit.collider.CompareTag("Grid"))
        {
            spriteSelectedGrid.SetActive(true);
            spriteSelectedGrid.transform.position = hit.collider.transform.position;
            selectedGrid = hit.collider.gameObject;
        }
        else
        {
            spriteSelectedGrid.SetActive(false);
            selectedGrid = null;
        }
    }


    private void CheckGrids()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Vector2.down, FindObjectOfType<BattleSystem>().gridPower, LayerMask.GetMask("Grid"));
            if (hit.collider != null)
            {
                hit.collider.gameObject.tag = "Untagged";
            }
        }
    }
}
