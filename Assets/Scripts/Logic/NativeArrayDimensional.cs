using Assets.Scripts.Infrastructure.Systems.GridFolder;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Logic
{
    public struct NativeArrayDimensional<T> where T : struct
    {
        private NativeArray<T> _arrray;
        int2 _saize;
        int _maxSaize;

        public NativeArrayDimensional(int2 saize, Allocator allocator) : this()
        {
            _saize = saize;
            _maxSaize = math.max(saize.x, saize.y);
            int length = saize.x * saize.y;
            _arrray = new NativeArray<T>(length, allocator);
        }

        public NativeArray<T> Arrray { get => _arrray; }

        public T this[int indexX, int indexY]
        {
            get =>
                _arrray[GetIndex(indexX, indexY)];

            set =>
                _arrray[GetIndex(indexX, indexY)] = value;
        }

        public int GetIndex(int indexX, int indexY) =>
            indexX + indexY * _maxSaize;

        public int GetLength(int dimension)
        {
            switch (dimension)
            {
                case 0:
                    return _saize.x;
                case 1:
                    return _saize.y;
                default:
                    throw new ArgumentException("Possible dimensions 0, 1");
            }
        }
    }
}
