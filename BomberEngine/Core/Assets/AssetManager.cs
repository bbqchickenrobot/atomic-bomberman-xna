﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using BomberEngine.Debugging;

namespace BomberEngine.Core.Assets
{
    public class AssetManager
    {
        private ContentManager contentManager;

        private AssetManagerListener listener;

        private Dictionary<AssetType, AssetLoader> loaders;

        private Asset[] assets;

        private List<AssetLoadInfo> loadingQueue;

        private int loadedCount;
        
        private Timer loadingTimer;

        public AssetManager(ContentManager contentManager, int resourcesCount)
        {
            this.contentManager = contentManager;

            assets = new Asset[resourcesCount];
            loadingQueue = new List<AssetLoadInfo>();
            loaders = new Dictionary<AssetType, AssetLoader>();
        }

        public void AddToQueue(AssetLoadInfo info)
        {
            Debug.Assert(!IsAssetLoaded(info.index), "Resource already loaded: " + info.path);
            Debug.Assert(!loadingQueue.Contains(info), "Resource already in the loading queue: " + info.path);

            loadingQueue.Add(info);
        }

        public void LoadImmediately()
        {   
            foreach (AssetLoadInfo info in loadingQueue)
            {
                LoadResource(info);
            }
            loadingQueue.Clear();
        }

        public void Load()
        {
            
        }

        public void AddLoader(AssetType type, AssetLoader loader)
        {
            loaders.Add(type, loader);
        }

        public Asset GetAsset(int id)
        {
            return assets[id];
        }

        private void OnTimer(Timer timer)
        {
            AssetLoadInfo info = loadingQueue[loadedCount];
            if (LoadResource(info))
            {
                ++loadedCount;

                if (listener != null)
                {
                    listener.OnResourceLoaded(this, info);
                }

                if (loadingQueue.Count == loadedCount)
                {
                    if (listener != null)
                    {
                        GC.Collect();
                        listener.OnResourcesLoaded(this);
                    }
                    timer.Cancel();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected bool LoadResource(AssetLoadInfo info)
        {
            AssetLoader loader = null;
            loaders.TryGetValue(info.type, out loader);

            if (loader == null)
            {
                throw new InvalidOperationException("Loader not found for resource type: " + info.type);
            }

            Asset asset = loader.LoadAsset(contentManager, info);
            Debug.Assert(asset != null, "Loader returned null: " + info.path);

            return asset != null;
        }

        private bool IsAssetLoaded(int id)
        {
            return assets[id] != null;
        }
    }
}
