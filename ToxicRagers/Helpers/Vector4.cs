﻿using System;

namespace ToxicRagers.Helpers
{
    public class Vector4
    {
        private Single _x;
        private Single _y;
        private Single _z;
        private Single _w;

        public Single X { get { return _x; } }
        public Single Y { get { return _y; } }
        public Single Z { get { return _z; } }
        public Single W { get { return _w; } }

        public Vector4(Single n)
        {
            _x = n;
            _y = n;
            _z = n;
            _w = n;
        }

        public Vector4(Single X, Single Y, Single Z, Single W)
        {
            _x = X;
            _y = Y;
            _z = Z;
            _w = W;
        }

        public static Vector4 Max(Vector4 v1, Vector4 v2)
        {
            return new Vector4(
                Math.Max(v1.X, v2.X),
                Math.Max(v1.Y, v2.Y),
                Math.Max(v1.Z, v2.Z),
                Math.Max(v1.W, v2.W)
            );
        }

        public Vector4 SplatX() { return new Vector4(this._x); }
        public Vector4 SplatY() { return new Vector4(this._y); }
        public Vector4 SplatZ() { return new Vector4(this._z); }
        public Vector4 SplatW() { return new Vector4(this._w); }

        public static Vector4 Reciprocal(Vector4 v)
        {
            return new Vector4(
                    1.0f / v.X,
                    1.0f / v.Y,
                    1.0f / v.Z,
                    1.0f / v.W
            );
        }

        public Vector3 ToVector3()
        {
            return new Vector3(this._x, this._y, this._z);
        }

        public static Vector4 operator +(Vector4 x, Vector4 y)
        {
            return new Vector4(x._x + y._x, x._y + y._y, x._z + y._z, x._w + y._w);
        }

        public static Vector4 operator *(Vector4 x, Vector4 y)
        {
            return new Vector4(x._x * y._x, x._y * y._y, x._z * y._z, x._w * y._w);
        }

        public static Vector4 MultiplyAdd(Vector4 a, Vector4 b, Vector4 c)
        {
            return a * b + c;
        }
    }
}
