using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapOpen : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject miniMap;
    public RawImage mapImage;

    public float minScale = 0.5f;
    private float maxScale;
    public float pinchSensitivity = 0.01f;

    private bool isMapLocked = true;
    private bool isDragging = false;
    private bool isDraggingMap = false;
    private bool isPinching = false; 
    private Vector2 lastMousePosition;
    private RectTransform miniMapRect;
    private Vector2 lockedPosition;

    void Start()
    {
        miniMap.SetActive(true);
        miniMapRect = miniMap.GetComponent<RectTransform>();

        if (mapImage == null && miniMap != null)
        {
            mapImage = miniMap.GetComponent<RawImage>();
        }

        float screenSize = Mathf.Min(Screen.width, Screen.height);
        maxScale = (0.9f * screenSize) / miniMapRect.rect.width;
    }

    void Update()
    {
        if (miniMap.activeInHierarchy && !isMapLocked)
        {
            HandlePinchToZoom();
        }
    }

    void HandlePinchToZoom()
    {
        if (Input.touchCount == 2)
        {
            if (!isPinching)
            {
                isPinching = true;
                lockedPosition = miniMapRect.anchoredPosition;
            }

            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevDistance = Vector2.Distance(touch1PrevPos, touch2PrevPos);
            float currentDistance = Vector2.Distance(touch1.position, touch2.position);

            float deltaDistance = (currentDistance - prevDistance) * pinchSensitivity * 2.5f; 

            if (Mathf.Abs(deltaDistance) > 0.01f)
            {
                float newScale = miniMapRect.localScale.x + deltaDistance;
                newScale = Mathf.Clamp(newScale, minScale, maxScale);

                miniMapRect.localScale = new Vector3(newScale, newScale, 1f);
            }

            miniMapRect.anchoredPosition = lockedPosition; 
        }
        else
        {
            isPinching = false; 
        }
    }

    public void OpenCloseMap()
    {
        if (miniMap.activeInHierarchy)
        {
            isMapLocked = !isMapLocked;
        }
        else
        {
            miniMap.SetActive(true);
            isMapLocked = false;
        }
    }

   public void OnDrag(PointerEventData eventData)
    {
        if (!isMapLocked && isDragging && !isPinching)
        {
            Vector2 localPointerPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPos);
            
            Vector2 localLastMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect.parent as RectTransform, lastMousePosition, eventData.pressEventCamera, out localLastMousePos);
            
            Vector2 delta = localPointerPos - localLastMousePos;
            miniMapRect.anchoredPosition += delta;
            lastMousePosition = eventData.position;
            
            ClampMapPosition();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isMapLocked && Input.touchCount == 1)
        {
            isDragging = true;
            isDraggingMap = true;
            lastMousePosition = eventData.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        isDraggingMap = false;
        isPinching = false;
    }

    public bool IsDraggingMap()
    {
        return isDraggingMap;
    }

    public void ToggleMapVisibility()
    {
        miniMap.SetActive(!miniMap.activeInHierarchy);
    }

    void ClampMapPosition()
    {
        Vector3 position = miniMapRect.anchoredPosition;

        RectTransform canvasRect = miniMapRect.root as RectTransform;
        if (canvasRect == null)
            return;

        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        float halfWidth = (miniMapRect.rect.width * miniMapRect.localScale.x) / 2f;
        float halfHeight = (miniMapRect.rect.height * miniMapRect.localScale.y) / 2f;

        float margin = Mathf.Min(canvasWidth, canvasHeight) * 0.02f;

        float minX = -canvasWidth / 2f + halfWidth + margin;
        float maxX = canvasWidth / 2f - halfWidth - margin;
        float minY = -canvasHeight / 2f + halfHeight + margin;
        float maxY = canvasHeight / 2f - halfHeight - margin;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        miniMapRect.anchoredPosition = position;
    }
}
