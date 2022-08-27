using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public interface ICharacter
    {
        public string Name { get; set; }
        public GameObject CharacterType { get; set; }
    }
}