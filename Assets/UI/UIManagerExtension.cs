using UnityEngine;

public enum UIRootType
{
    Background,
    Main,
    Content,
    Popup,
    Topmost
}

public enum UIType
{
    PlayerHUD,
    InteractionPopup,
    EnemyHPBar,
    GameOverPopup,
    Inventory
}


public static class UIManagerExtension
{
    public static void ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        uiManager.OpenPlayerHUD();
    }

    public static void OpenPlayerHUD(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.Main, UIType.PlayerHUD);

        if (uiBase == null)
        {
            Debug.LogWarning("[UIManagerExtension] PlayerHUD 생성 실패!");
            return;
        }

    }

    public static void OpenInteractionPopup(this UIManager uiManager, string message)
    {
        var uiBase = uiManager.OpenUI(UIRootType.Popup, UIType.InteractionPopup);

        if (uiBase == null)
        {
            Debug.LogWarning("[UIManagerExtension] InteractionPopup 생성 실패!");
            return;
        }

    }

    public static void OpenEnemyHPBar(this UIManager uiManager, EnemyStats enemy, string name)
    {
        var uiBase = uiManager.OpenUI(UIRootType.Main, UIType.EnemyHPBar);

        if (uiBase == null)
        {
            Debug.LogWarning("[UIManagerExtension] EnemyHPBar 생성 실패!");
            return;
        }

        if (uiBase is UI_EnemyHPBar enemyHUD)
        {
            enemyHUD.SetTarget(enemy, name);
        }
    }

    public static void OpenInventoryUI(this UIManager uiManager)
    {
        var uiBase = uiManager.OpenUI(UIRootType.Popup, UIType.Inventory);
        if (uiBase == null) Debug.LogWarning("[UIManagerExtension] Inventory 생성 실패!");
    }
}
