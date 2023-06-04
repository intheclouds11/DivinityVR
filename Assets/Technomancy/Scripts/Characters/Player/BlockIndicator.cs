using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class BlockIndicator : MonoBehaviour
    {
        public bool inBothBlockTriggers;
        public bool inHigherCol { get; set; }
        public bool inLowerCol { get; set; }
        private HighlightEffect _highlightEffect;
        [SerializeField]
        private HighlightProfile _highlightProfileOriginal;
        [SerializeField]
        public HighlightProfile _highlightProfileGoodBlock;
        private Transform _playerControllerTrans;

        private void Awake()
        {
            _playerControllerTrans = LocalUserObjects.instance.ITCPlayerController.transform;
            _highlightEffect = GetComponent<HighlightEffect>();

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            inHigherCol = false;
            inLowerCol = false;
            _highlightEffect.highlighted = false;
            _highlightEffect.ProfileLoad(_highlightProfileOriginal);
        }

        void Update()
        {
            inBothBlockTriggers = inHigherCol && inLowerCol;

            if (inBothBlockTriggers && !_highlightEffect.highlighted)
            {
                _highlightEffect.highlighted = true;
            }
            else if (!inBothBlockTriggers)
            {
                _highlightEffect.highlighted = false;
            }
        }

        public void ToggleIndicator(EnemyStats enemy)
        {
            if (!gameObject.activeSelf)
            {
                var dirFromPlayerToEnemy = enemy.transform.position - _playerControllerTrans.position;
                var fwdDistanceFromPlayer = dirFromPlayerToEnemy.normalized * 0.5f;
                var height = Vector3.up * 1.5f;
                
                // Random position and rotation offsets for now. In the future could have enemy animation event specify pos/rot of block indicator
                var randomXOffset = Random.Range(-0.1f, 0.1f);
                var randomYOffset = Random.Range(-0.1f, 0.1f);
                var randomZPlanePosOffset = Vector3.ProjectOnPlane(new Vector3(randomXOffset, randomYOffset, 0), dirFromPlayerToEnemy);
                var randomRotOffset = Random.Range(-0.2f, 0.2f);
                var randomZPlaneRotOffset = Vector3.ProjectOnPlane(new Vector3(randomRotOffset, 1, randomRotOffset), dirFromPlayerToEnemy);
                
                transform.position = _playerControllerTrans.position + fwdDistanceFromPlayer + height + randomZPlanePosOffset;
                transform.rotation = Quaternion.LookRotation(dirFromPlayerToEnemy.normalized, randomZPlaneRotOffset);
                
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void GoodBlockHighlight()
        {
            _highlightEffect.ProfileLoad(_highlightProfileGoodBlock);
        }
    }
}