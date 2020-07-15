using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace MaterialProperty
{
    [GenerateAuthoringComponent, Serializable]
    [MaterialProperty(PopMaterialProperty.IntCurrentFrame,
        MaterialPropertyFormat.Float)]
    public struct PopMaterialProp_CurrentFrame : IComponentData
    {
        [SerializeField] private float value;

        public int Value
        {
            get => Mathf.FloorToInt(value);
            set => this.value = value;
        }
    }
}