using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

namespace TileAdventure
{

    public class RackController : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private RackBehavior _rackBehavior;
        public RackBehavior RackBehavior => _rackBehavior;

        private RackData _rackData;
        private Dictionary<string, TileBehavior> _rackTileBehaviors = new Dictionary<string, TileBehavior>();
        private bool _isAnimating;

        public Action OnMatchFinished;

        public bool IsFull => _rackData != null && _rackData.IsFull;
        public bool IsEmpty => _rackData != null && _rackData.Tiles.Count == 0;
        public bool IsAnimating => _isAnimating;

        [Header("Audio")]
        [SerializeField] private AudioClipSO _matchAudioClip;
        [SerializeField] private AudioClipEventSO _audioChannel;


        public void Initialize()
        {
            _rackData = new RackData();
            _rackTileBehaviors.Clear();
            _isAnimating = false;
        }


        public void AddTile(TileData tileData, TileBehavior tileBehavior)
        {
            _rackTileBehaviors[tileData.Id] = tileBehavior;
            _rackData.TryAddTile(tileData);
            _isAnimating = true;

            _rackBehavior.UpdateTilesPosition(_rackData.Tiles, _rackTileBehaviors, () =>
            {
                CheckMatch();
            });
        }

        private void CheckMatch()
        {
            List<TileData> matchedTiles = _rackData.CheckForMatch();
            if (matchedTiles != null)
            {
                
                _rackBehavior.PlayMatchAnimation(matchedTiles, _rackTileBehaviors, 
                onComplete: () =>
                {
                    _audioChannel.RaiseEvent(_matchAudioClip);
                    foreach (var tile in matchedTiles)
                    {
                        if (_rackTileBehaviors.TryGetValue(tile.Id, out TileBehavior tileBehavior))
                        {
                            AssetManager.ReleaseInstance(tileBehavior.gameObject);
                            _rackTileBehaviors.Remove(tile.Id);
                        }
                    }

                    _rackBehavior.UpdateTilesPosition(_rackData.Tiles, _rackTileBehaviors, () =>
                    {
                        _isAnimating = false;
                        OnMatchFinished?.Invoke();
                    });
                });
            }
            else
            {
                _isAnimating = false;
                OnMatchFinished?.Invoke();
            }
        }
    }
}
