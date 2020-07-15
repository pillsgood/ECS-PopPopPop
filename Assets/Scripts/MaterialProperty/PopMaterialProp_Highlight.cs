using System;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace MaterialProperty
{
    [GenerateAuthoringComponent, Serializable]
    [MaterialProperty(PopMaterialProperty.BooleanHighlight,
        MaterialPropertyFormat.Float)]
    public struct PopMaterialProp_Highlight : IComponentData
    {
        [SerializeField] private float value;

        public bool Value
        {
            get => value <= 1;
            set => this.value = value ? 1f : 0f;
        }
    }
}