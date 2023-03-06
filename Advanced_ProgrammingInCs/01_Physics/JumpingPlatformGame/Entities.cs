using System;
using System.Drawing;
using GamePhysics;

namespace JumpingPlatformGame {
	class Entity {
		public virtual Color Color => Color.Black;

		public WorldPoint Location { get; internal set; }
	}

	class WorldPoint
	{
		public Meter X;
		public Meter Y;
	}

	class axis
	{
		public Meter LowerBound;
		public Meter UpperBound;
		public Speed Speed;
	}

	class MovableEntity : Entity
	{
		public axis Horizontal;

    }

	class MovableJumpingEntity : MovableEntity {
		public axis Vertical;
    }

	class Joe : MovableEntity {
		public override string ToString() => "Joe";
		public override Color Color => Color.Blue;
	}

	class Jack : MovableEntity {
		public override string ToString() => "Jack";
		public override Color Color => Color.LightBlue;
	}

	class Jane : MovableJumpingEntity {
		public override string ToString() => "Jane";
		public override Color Color => Color.Red;
	}

	class Jill : MovableJumpingEntity {
		public override string ToString() => "Jill";
		public override Color Color => Color.Pink;
	}

}
