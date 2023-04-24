using System.Collections.Generic;
using UnityEngine;

namespace SingletonPattern
{
    public class OutlineManager : Singleton<OutlineManager>
    {
        #region Variables

        private Material _outline;

        private GameObject _target;
        private Renderer _renderer;
        private List<Material> _materialList = new();

        #endregion Variables

        #region Methods

        public void OnDrawOutline(GameObject target)
        {
            if (target == null) return;
            if (_outline == null) _outline = new Material(Shader.Find("Draw/OutlineShader"));

            DisableOutline();

            _target = target;
            _renderer = target.GetComponent<Renderer>();

            _materialList.Clear();
            _materialList.AddRange(_renderer.sharedMaterials);
            _materialList.Add(_outline);

            _renderer.materials = _materialList.ToArray();
        }

        public void DisableOutline()
        {
            if (_target == null || _renderer == null) return;

            _materialList.Clear();
            _materialList.AddRange(_renderer.sharedMaterials);
            _materialList.Remove(_outline);

            _renderer.materials = _materialList.ToArray();
        }

        #endregion Methods
    }
}