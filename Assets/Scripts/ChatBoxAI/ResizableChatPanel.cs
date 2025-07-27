using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizableChatPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Panel Settings")]
    public RectTransform chatPanel;
    public Vector2 minSize = new Vector2(300, 200);
    public Vector2 maxSize = new Vector2(800, 600);
    public float resizeZoneWidth = 10f;
    
    [Header("UI References")]
    public Button toggleButton; // Button hiện/ẩn chat
    public GameObject chatContent;
    
    [Header("Toggle Settings")]
    public Sprite showSprite;  // Sprite khi chat ẩn (hiển thị icon mở)
    public Sprite hideSprite;  // Sprite khi chat hiện (hiển thị icon thu gọn)
    
    private RectTransform rectTransform;
    private bool isDragging = false;
    private bool isInResizeZone = false;
    private Vector2 lastMousePosition;
    private bool isPanelVisible = true;
    private Vector2 originalSize;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null && chatPanel != null)
            rectTransform = chatPanel;
            
        if (rectTransform != null)
            originalSize = rectTransform.sizeDelta;
        
        // Setup button events
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleChatPanel);
            UpdateToggleButtonSprite();
        }
    }
    
    void Update()
    {
        if (rectTransform == null) return;
        
        // Change cursor when in resize zone
        if (isInResizeZone && !isDragging)
        {
            // You can change cursor here if needed
            // Cursor.SetCursor(resizeCursor, Vector2.zero, CursorMode.Auto);
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsInResizeZone(eventData.position))
        {
            isDragging = true;
            lastMousePosition = eventData.position;
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || rectTransform == null) return;
        
        Vector2 currentMousePosition = eventData.position;
        Vector2 deltaPosition = currentMousePosition - lastMousePosition;
        
        // Convert delta to local space
        Vector2 localDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform, 
            deltaPosition, 
            eventData.pressEventCamera, 
            out localDelta);
        
        // Calculate new size
        Vector2 newSize = rectTransform.sizeDelta + new Vector2(localDelta.x, -localDelta.y);
        
        // Clamp to min/max size
        newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
        newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);
        
        rectTransform.sizeDelta = newSize;
        lastMousePosition = currentMousePosition;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isInResizeZone = IsInResizeZone(eventData.position);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isInResizeZone = false;
        if (!isDragging)
        {
            // Reset cursor if needed
            // Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
    
    private bool IsInResizeZone(Vector2 screenPosition)
    {
        if (rectTransform == null) return false;
        
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, screenPosition, null, out localPosition);
        
        Vector2 size = rectTransform.rect.size;
        
        // Check if near right or bottom edge
        bool nearRightEdge = localPosition.x > (size.x * 0.5f - resizeZoneWidth);
        bool nearBottomEdge = localPosition.y < (-size.y * 0.5f + resizeZoneWidth);
        
        return nearRightEdge || nearBottomEdge;
    }
    
    public void ToggleChatPanel()
    {
        isPanelVisible = !isPanelVisible;
        
        if (chatContent != null)
        {
            chatContent.SetActive(isPanelVisible);
        }
        
        UpdateToggleButtonSprite();
        
        // Optionally resize panel when toggling
        if (rectTransform != null)
        {
            if (isPanelVisible)
            {
                rectTransform.sizeDelta = originalSize;
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(originalSize.x, 50f); // Show only header
            }
        }
    }
    
    public void ShowChatPanel()
    {
        if (chatPanel != null)
        {
            chatPanel.gameObject.SetActive(true);
        }
        else if (gameObject != null)
        {
            gameObject.SetActive(true);
        }
        
        isPanelVisible = true;
        if (chatContent != null)
        {
            chatContent.SetActive(true);
        }
        UpdateToggleButtonSprite();
    }
    
    private void UpdateToggleButtonSprite()
    {
        if (toggleButton == null) return;
        
        Image buttonImage = toggleButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = isPanelVisible ? hideSprite : showSprite;
        }
    }
    
    public bool IsPanelVisible()
    {
        return isPanelVisible;
    }
    
    public void SetPanelSize(Vector2 newSize)
    {
        if (rectTransform != null)
        {
            newSize.x = Mathf.Clamp(newSize.x, minSize.x, maxSize.x);
            newSize.y = Mathf.Clamp(newSize.y, minSize.y, maxSize.y);
            rectTransform.sizeDelta = newSize;
        }
    }
}
