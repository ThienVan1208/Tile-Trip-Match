using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TileAdventure
{

    public static class UIController
    {
        private static readonly Dictionary<Type, UIPage> _pageCache = new Dictionary<Type, UIPage>();
        private static Transform _uiRoot;

        public static UIPage CurrentPage { get; private set; }

        public static UIPage PreviousPage { get; private set; }

        public static void Initialize(Transform uiRoot)
        {
            _uiRoot = uiRoot;
            _pageCache.Clear();
            CurrentPage = null;
            PreviousPage = null;

            foreach (var page in uiRoot.GetComponentsInChildren<UIPage>(true))
            {
                var type = page.GetType();
                if (_pageCache.ContainsKey(type))
                {
                    continue;
                }
                _pageCache[type] = page;
                // page.gameObject.SetActive(false);
            }

        }

        public static async UniTask<T> OpenUI<T>(bool isPrevPageClosed = true) where T : UIPage
        {
            var page = GetOrFind<T>();
            if (page == null)
            {
                Debug.LogError($" UIPage of type {typeof(T).Name} not found!");
                return null;
            }


            if (page.IsShowing)
            {
                return page as T;
            }

        
            if (CurrentPage != null && CurrentPage != page)
            {
                PreviousPage = CurrentPage;

                if (isPrevPageClosed)
                {
                    await PreviousPage.Hide();
                }
            }


            CurrentPage = page;
            await page.Show();

            return page as T;
        }

        public static async UniTask CloseUI<T>() where T : UIPage
        {
            var page = GetOrFind<T>();
            if (page == null || !page.IsShowing) return;

            await page.Hide();

            if (CurrentPage == page)
            {
                CurrentPage = PreviousPage;
                PreviousPage = null;
            }
        }


        public static T GetUI<T>() where T : UIPage
        {
            return GetOrFind<T>();
        }

        private static T GetOrFind<T>() where T : UIPage
        {
            if (_pageCache.TryGetValue(typeof(T), out var cached))
            {
                return cached as T;
            }


            if (_uiRoot != null)
            {
                var found = _uiRoot.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    _pageCache[typeof(T)] = found;
                    return found;
                }
            }

            return null;
        }
    }
}
