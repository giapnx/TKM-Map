using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2i
{
    public int x { get; set; }

    public int y { get; set; }

    public Vector2i()
    {
        x = 0;
        y = 0;
    }

    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        var other = obj as Vector2i;
        if (other == null)
            return false;

        return this.x == other.x && this.y == other.y;
    }

    public override string ToString()
    {
        return "[" + x + "," + y + "]";
    }
}
