using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;


public class Circle {
    public Vector2 Center;
    public float Radius;

    public Circle(Vector2 center, float radius) {
        this.Center = center;
        this.Radius = radius;
    }
    public bool Contains(Vector2 particle) {
        float distance = (particle - Center).Length();
        return distance <= this.Radius;
    }
    public bool Intersects(Rect2 rect)
    {
        float circleDistanceX = Math.Abs(this.Center.X - (rect.Position.X + rect.Size.X / 2));
        float circleDistanceY = Math.Abs(this.Center.Y - (rect.Position.Y + rect.Size.Y / 2));

        if (circleDistanceX > (rect.Size.X / 2 + this.Radius)) { return false; }
        if (circleDistanceY > (rect.Size.Y / 2 + this.Radius)) { return false; }

        if (circleDistanceX <= (rect.Size.X / 2)) { return true; }
        if (circleDistanceY <= (rect.Size.Y / 2)) { return true; }

        float cornerDistanceSq = (float)(Math.Pow(circleDistanceX - rect.Size.X / 2, 2) +
                                         Math.Pow(circleDistanceY - rect.Size.Y / 2, 2));

        return cornerDistanceSq <= Math.Pow(this.Radius, 2);
    }
}
public class QuadTree {
    public Rect2 Boundary { get; private set; }
    public int Capacity { get; private set; }
    public List<Particle> Particles { get; private set; }
    public bool Divided { get; private set; }

    private QuadTree Northeast { get; set; }
    private QuadTree Northwest { get; set; }
    private QuadTree Southeast { get; set; }
    private QuadTree Southwest { get; set; }


    public  QuadTree(Rect2 boundary, int capacity) {
        this.Boundary = boundary;
        this.Capacity = capacity;
        this.Particles = new List<Particle>(Capacity);
        this.Divided = false;
    }
    public void Clear() {
        this.Northeast = null;
        this.Northwest = null;
        this.Southeast = null;
        this.Southwest = null;
        this.Divided = false;
        this.Particles = new List<Particle>();
    }
    public void Subdivide() {
        float x = this.Boundary.Position.X;
        float y = this.Boundary.Position.Y;
        float w = this.Boundary.Size.X / 2;
        float h = this.Boundary.Size.Y / 2;

        Rect2 ne = new Rect2(x + w, y, w, h);
        this.Northeast = new QuadTree(ne, this.Capacity);

        Rect2 nw = new Rect2(x, y, w, h);
        this.Northwest = new QuadTree(nw, this.Capacity);

        Rect2 se = new Rect2(x + w, y + h, w, h);
        this.Southeast = new QuadTree(se, this.Capacity);

        Rect2 sw = new Rect2(x, y + h, w, h);
        this.Southwest = new QuadTree(sw, this.Capacity);

        this.Divided = true;
    }
    public bool Insert(Particle particle) {
        if (!this.Boundary.HasPoint(particle.ParticlePosition)) {
            return false;
        }
        if (this.Particles.Count < this.Capacity) {
            this.Particles.Add(particle);
            return true;
        } else {
            if (!this.Divided) {
                this.Subdivide();
            }
            if (this.Northeast.Insert(particle)) {
                return true;
            } else if (this.Northwest.Insert(particle)) {
                return true;
            } else if (this.Southeast.Insert(particle)) {
                return true;
            } else if (this.Southwest.Insert(particle)) {
                return true;
            }
        }

        return false;
    }
    public List<Particle> Query(Circle range, List<Particle> found = null) {
        if (found == null) {
            found = new List<Particle>();
        }

        if (!range.Intersects(this.Boundary)) {
            return found;
        } else {
            foreach (Particle particle in Particles) {
                if (range.Contains(particle.ParticlePosition)) {
                    found.Add(particle);
                }
            }
            if (this.Divided) {
                this.Northwest.Query(range, found);
                this.Northeast.Query(range, found);
                this.Southwest.Query(range, found);
                this.Southeast.Query(range, found);
            }
        }
        return found;
    }

    public void Draw(CanvasItem canvas) {
        canvas.DrawRect(new Rect2(this.Boundary.Position , this.Boundary.Size), Colors.White, false, 1f);
        if (this.Divided) {
            this.Northeast.Draw(canvas);
            this.Northwest.Draw(canvas);
            this.Southeast.Draw(canvas);
            this.Southwest.Draw(canvas);
        }
    }
}