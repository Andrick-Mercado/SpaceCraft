using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    
    [SerializeField] private Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        foreach (var t in menus)
        {
            if (t.menuName == menuName)
            {
                OpenMenu(t);
            }
            else if (t.open)
            {
                CloseMenu(t);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        menu.Open();
    }

    public void CloseMenu(string menuName)
    {
        foreach (var t in menus)
        {
            if (t.menuName == menuName)
            {
                CloseMenu(t);
            }
        }
        
    }
    
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseAllOpenMenus()
    {
        foreach (var t in menus)
        {
            if (t.open)
            {
                CloseMenu(t);
            }
        }
    }

    public bool IsMenuOpen(string menuName)
    {
        return menus.Any(t => t.menuName == menuName && t.open);
    }
}
