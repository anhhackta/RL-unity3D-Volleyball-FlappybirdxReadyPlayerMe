using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ChatHistoryDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public TMP_Text chatText;
    public ScrollRect scrollRect;
    public RectTransform content;
    
    [Header("Hover Settings")]
    public float hoverScrollSpeed = 100f;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.yellow;
    
    [Header("Auto Scroll Settings")]
    public bool autoScrollToBottom = true;
    public float autoScrollDelay = 0.1f;
    
    private bool isHovering = false;
    private float originalScrollPosition;
    private Coroutine autoScrollCoroutine;
    
    void Start()
    {
        if (chatText == null)
            chatText = GetComponent<TMP_Text>();
            
        if (scrollRect == null)
            scrollRect = GetComponentInParent<ScrollRect>();
    }
    
    void Update()
    {
        if (isHovering && scrollRect != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                scrollRect.verticalNormalizedPosition += scroll * hoverScrollSpeed * Time.deltaTime;
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (chatText != null)
        {
            chatText.color = hoverTextColor;
        }
        
        if (scrollRect != null)
        {
            originalScrollPosition = scrollRect.verticalNormalizedPosition;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (chatText != null)
        {
            chatText.color = normalTextColor;
        }
    }
    
    public void ScrollToTop()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
    
    public void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            if (autoScrollCoroutine != null)
            {
                StopCoroutine(autoScrollCoroutine);
            }
            autoScrollCoroutine = StartCoroutine(SmoothScrollToBottom());
        }
    }
    
    private IEnumerator SmoothScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases();
        yield return new WaitForSeconds(autoScrollDelay);
        
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    public void UpdateChatText(string newText)
    {
        if (chatText != null)
        {
            chatText.text = newText;
            
            if (autoScrollToBottom)
            {
                ScrollToBottom();
            }
        }
    }
    
    public void AppendChatText(string additionalText)
    {
        if (chatText != null)
        {
            chatText.text += additionalText;
            
            if (autoScrollToBottom)
            {
                ScrollToBottom();
            }
        }
    }
    
    public void SetAutoScroll(bool enabled)
    {
        autoScrollToBottom = enabled;
    }
}
