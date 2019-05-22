using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class InstructionsMenuButton : MenuButton {
    
    public Sprite[] instructionSlides;
    public int currentSlideIdx;
    public bool isDisplaying;
    private SpriteRenderer slideContainer;
    private SpriteRenderer overlay;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject returnButton;

    protected override void OnMouseUpAsButton() {
        base.OnMouseUpAsButton();
        isDisplaying = true;
    }

    protected void Start() {
        overlay = GameObject.Find("_DarknessOverlay").GetComponent<SpriteRenderer>();
        slideContainer = transform.Find("CurrentSlide").GetComponent<SpriteRenderer>();
        currentSlideIdx = 0;
        isDisplaying = false;
    }

    protected void Update() {
        if(isDisplaying) {
            overlay.enabled = true;
            slideContainer.enabled = true;
            nextButton.SetActive(true);
            prevButton.SetActive(true);
            returnButton.SetActive(true);
            
            if(currentSlideIdx == 0) {
                prevButton.GetComponent<SpriteRenderer>().enabled = false;
                prevButton.GetComponentInChildren<TextMeshPro>().enabled = false;
                prevButton.GetComponent<BoxCollider2D>().enabled = false;
            } else {
                prevButton.GetComponent<SpriteRenderer>().enabled = true;
                prevButton.GetComponentInChildren<TextMeshPro>().enabled = true;
                prevButton.GetComponent<BoxCollider2D>().enabled = true;
            }

            if(currentSlideIdx == 4) {
                nextButton.GetComponent<SpriteRenderer>().enabled = false;
                nextButton.GetComponentInChildren<TextMeshPro>().enabled = false;
                nextButton.GetComponent<BoxCollider2D>().enabled = false;
            } else {
                nextButton.GetComponent<SpriteRenderer>().enabled = true;
                nextButton.GetComponentInChildren<TextMeshPro>().enabled = true;
                nextButton.GetComponent<BoxCollider2D>().enabled = true;
            }

        // turn everything off
        } else {
            nextButton.SetActive(false);
            prevButton.SetActive(false);
            returnButton.SetActive(false);
            currentSlideIdx = 0;
            slideContainer.sprite = instructionSlides[0];
            slideContainer.enabled = false;
            overlay.enabled = false;
        }
    }

    public IEnumerator SwitchSlides(int incrementer) {
        currentSlideIdx += incrementer;
        Color initColor = slideContainer.color;
        slideContainer.DOColor(new Color(initColor.r, initColor.g, initColor.b, 0f), .25f);
        yield return new WaitForSeconds(.25f);
        slideContainer.sprite = instructionSlides[currentSlideIdx];
        slideContainer.DOColor(Color.white, .25f);
    }
}
