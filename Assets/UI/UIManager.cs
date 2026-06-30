using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Canvas Roots")]
    //[SerializeField] private Transform bgRoot;
    [SerializeField] private Transform mainRoot;
    //[SerializeField] private Transform contentRoot;
    [SerializeField] private Transform popupRoot;
    //[SerializeField] private Transform topmostRoot;

    private Dictionary<UIType, UIBase> cachedUIs = new Dictionary<UIType, UIBase>();

    private HashSet<UIType> openedUIs = new HashSet<UIType>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        this.ShowStartupUIOnGameStart();
    }

    public UIBase OpenUI(UIRootType rootType, UIType uiType)
    {
        UIBase ui = GetOrCreateUI(rootType, uiType);

        if (ui != null)
        {
            ui.gameObject.SetActive(true);
            openedUIs.Add(uiType);
            ui.Setup();
        }

        return ui;
    }

    public void CloseUI(UIType uiType)
    {
        if (cachedUIs.TryGetValue(uiType, out UIBase ui))
        {
            ui.gameObject.SetActive(false);
            openedUIs.Remove(uiType);
        }
    }

    private UIBase GetOrCreateUI(UIRootType rootType, UIType uiType)
    {
        if (cachedUIs.TryGetValue(uiType, out UIBase existingUI))
        {
            return existingUI;
        }

        string path = GetUIPath(uiType);
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"[UIManager] UI 프리팹을 찾을 수 없습니다. 경로를 확인하세요: Resources/{path}");
            return null;
        }

        Transform parentRoot = GetRootTransform(rootType);
        GameObject uiInstance = Instantiate(prefab);
        uiInstance.transform.SetParent(parentRoot, false);
        UIBase uiBase = uiInstance.GetComponent<UIBase>();

        if (uiBase != null)
        {
            cachedUIs.Add(uiType, uiBase);
        }
        else
        {
            Debug.LogError($"[UIManager] 프리팹에 UIBase 컴포넌트가 없습니다: {uiType}");
        }

        return uiBase;
    }

    private Transform GetRootTransform(UIRootType rootType)
    {
        return rootType switch
        {
            ////UIRootType.Background => bgRoot,
            UIRootType.Main => mainRoot,
            //UIRootType.Content => contentRoot,
            UIRootType.Popup => popupRoot,
            //UIRootType.Topmost => topmostRoot,
            _ => mainRoot,
        };
    }

    private string GetUIPath(UIType uiType)
    {
        return $"UI/Prefabs/{uiType}";
    }
}
