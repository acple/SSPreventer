using System;

namespace SSPreventer
{
    public readonly struct Position : IEquatable<Position>
    {
        public int X { get; }

        public int Y { get; }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Position other)
            => this == other;

        public override bool Equals(object? obj)
            => obj is Position other && this == other;

        public override int GetHashCode()
            => this.X.GetHashCode() << 16 ^ this.Y.GetHashCode();

        public override string ToString()
            => $"(x: {this.X}, y: {this.Y})";

        public static bool operator ==(Position left, Position right)
            => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Position left, Position right)
            => !(left == right);
    }
}
