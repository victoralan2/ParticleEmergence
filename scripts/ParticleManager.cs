using ComputeShader.Compute;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

public partial class ParticleManager : Node2D
{
    [Export] public bool Enabled = true;
    [Export] public uint NumberOfParticles = 10;
    [Export] public Color[] ParticleColors;
    [Export] public Vector2 SpaceDimensions = new Vector2(10, 10); // Square
    // [Export] public Implementation Implementation = Implementation.RK4;
    [Export] public float TimeScale = 1f;
    [Export] public float MaxRadius = 10f;
    [Export] public int Seed = 0; // If 0 then it will be random
    public MultiMesh multiMesh = null;
    public Random rand;
    // public CellGrid Grid;
    public List<Particle> Particles = new List<Particle>();
    public InteractionMatrix InteractionMatrix;
    public ComputeManager computeManager;
    public int xCells;
    public int yCells;
    public void CreateParticle(Particle particle) {
        particle.OverallIndex = Particles.Count();
        Particles.Add(particle);
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Setup();
    }
    public void Setup() {
        if (!Enabled) {
            SetProcess(false);
            return;
        }
        xCells = Mathf.CeilToInt(SpaceDimensions.X / MaxRadius);
        yCells = Mathf.CeilToInt(SpaceDimensions.Y / MaxRadius);
        SpaceDimensions.X = xCells * MaxRadius;
        SpaceDimensions.Y = yCells * MaxRadius;
    
        if (Seed == 0 ) {
            this.Seed = new Random().Next();
        }
        this.rand = new Random(Seed);
 
        this.InteractionMatrix = new InteractionMatrix(ParticleColors, this.rand);
        computeManager = new ComputeManager();
        // this.Grid = new CellGrid(xCells, yCells, MaxRadius);


        CreateParticles();
        InitializeComputeManager();
        SetCameraPosition();
        SetupMesh();
    }
    public void Reset(Configuration? newConfiguration = null) {
        List<MultiMeshInstance2D> meshInstances = NodeFinder.FindNodesOfType<MultiMeshInstance2D>(this);
        foreach (var mesh in meshInstances) {
            mesh.Free();
        }
        this.computeManager.CleanUp();
        if (newConfiguration is Configuration configuration) {
            this.NumberOfParticles = configuration.NumberOfParticles;
            this.ParticleColors = configuration.ParticleColors;
            this.SpaceDimensions = configuration.SpaceDimensions;
            this.TimeScale = configuration.TimeScale;
            this.Seed = configuration.Seed;
        }
        this.Particles = new List<Particle>();
        this.multiMesh = null;
        this.computeManager = null;
        this.InteractionMatrix = null;

        this.Setup();
    }
    public void SetupMesh() {
        multiMesh = new MultiMesh() {
            UseColors = true,
            InstanceCount = (int)NumberOfParticles,
        };
        multiMesh.Mesh = new QuadMesh();
        int i = 0;
        foreach (Particle particle in Particles) {
            multiMesh.SetInstanceTransform2D(i, new Transform2D(0, particle.ParticlePosition));
            multiMesh.SetInstanceColor(i, particle.Color);
            i++;
        }

        MultiMeshInstance2D multiMeshInstance2D = new MultiMeshInstance2D() {
            Multimesh = multiMesh,
            Position = Vector2.Zero,
            Material = GD.Load<ShaderMaterial>("res://shaders/particle.tres"),
        };
        CallDeferred(MethodName.AddChild, multiMeshInstance2D);
    }

    public void CreateParticles() {
        for (int i = 0; i < NumberOfParticles; i++) {
            int index = rand.Next(0, ParticleColors.Length);
            Color color = ParticleColors[index];
            float x = (float)rand.NextDouble() * SpaceDimensions.X;
            float y = (float)rand.NextDouble() * SpaceDimensions.Y;
            Vector2 ParticlePosition = new Vector2(x, y);
            
            // Generate particles
            Particle particle = new Particle(color, ParticlePosition, Vector2.Zero, 1f);
            CreateParticle(particle);
        }
    }
    public void InitializeComputeManager() {
        ParticleInfo[] particleInfos = Particles.Select((p) => {
            uint indexOfColor = 0;
            foreach (Color c in ParticleColors) {
                if (c == p.Color) {
                    break;
                }
                indexOfColor++;
            }

            return new ParticleInfo() { posX = p.ParticlePosition.X, posY = p.ParticlePosition.Y, velX = p.Velocity.X, velY = p.Velocity.Y, color = indexOfColor };
        }).ToArray();
        Params parameters = new Params() { 
            numParticles = NumberOfParticles, 
            xCells = xCells, 
            yCells =  yCells, 
            delta = 0f,
            maxRadius = MaxRadius, 
            numColors = ParticleColors.Length,
            timeScale = TimeScale,
        };

        float[] interactionMatrix = this.InteractionMatrix.AsSimple();

        // CellArrayBuffers gridInfo = Grid.BuildParticleCells(Particles.ToArray());
        computeManager
            .AddPipeline("res://scripts/compute_shaders/particles.glsl", NumberOfParticles, 1, 1)
            .StoreAndAddStep(0, particleInfos)
            .StoreAndAddStep(1, parameters)
            .StoreAndAddStep(2, interactionMatrix)
            // .StoreAndAddStep(3, gridInfo.cellsArray)
            // .StoreAndAddStep(4, gridInfo.particleCellMembership)
            // .StoreAndAddStep(5, gridInfo.flatCelledIndices)
            .Build();
    }
    public void SetCameraPosition() {
        Camera camera = (Camera)GetViewport().GetCamera2D();
        camera.SetCameraPosition(SpaceDimensions/2, SpaceDimensions.X, SpaceDimensions.Y);
    }
    public override void _ExitTree()
    {
        computeManager.CleanUp();
    }
    public override void _Draw()
    {
        // DrawRect(new Rect2(Vector2.Zero, new Vector2(SpaceDimensions.X, SpaceDimensions.Y)), Colors.WhiteSmoke, false, 0.1f);
        
        // if(Input.IsMouseButtonPressed(MouseButton.Left))
        // {
            // var mouseWorldParticlePosition = GetCanvasTransform().AffineInverse() * GetViewport().GetMousePosition();
            // Circle circle = new Circle(mouseWorldParticlePosition, 20f);
            // DrawArc(circle.Center, circle.Radius, 0, MathF.Tau, 32, Colors.Red, 2f);

            // var particles = this.QTree.Query(circle);
            // foreach (Particle particle in Particles) {
                // if (particles.Contains(particle)) {
                    // particle.Modulate = Colors.Green;
                // } else {
                    // particle.Modulate = particle.Color;
                // }
            // }
        // }
    }
    public override void _Process(double delta)
	{
        if (!DesktopManager.Instance.IsDesktopVisible) return;
        xCells = Mathf.CeilToInt(SpaceDimensions.X / MaxRadius);
        yCells = Mathf.CeilToInt(SpaceDimensions.Y / MaxRadius);
        SpaceDimensions.X = xCells * MaxRadius;
        SpaceDimensions.Y = yCells * MaxRadius;
        if (computeManager.JustSynced) {
            
            ParticleInfo[] infos = computeManager.GetDataFromBufferAsArray<ParticleInfo>(0);
            Vector2[] newPositions = infos.Select((p) => new Vector2(p.posX, p.posY)).ToArray();


            for (int i = 0; i < NumberOfParticles; i++) {
                Particles[i].ParticlePosition = newPositions[i];
                multiMesh.SetInstanceTransform2D(i, new Transform2D(0f, newPositions[i]));
            }
            Params parameters = new Params() { 
                numParticles = NumberOfParticles, 
                xCells = xCells, 
                yCells =  yCells, 
                delta = (float)delta * ComputeManager.FramesPerSync,
                maxRadius = MaxRadius, 
                numColors = ParticleColors.Length,
                timeScale = TimeScale,
            };
            computeManager.UpdateBuffer(1, parameters);
            this.InteractionMatrix.Next(delta);
            computeManager.UpdateBuffer(2, this.InteractionMatrix.AsSimple());
        }
        computeManager.Execute();
	}

    // public void ProcessPhysics(float delta, Particle particle) {
        // Circle range = new Circle(particle.ParticlePosition, MaxRadius);
        // var oldPos = particle.ParticlePosition;
        // List<Particle> effectiveParticles = QTree.Query(range);
        // if (effectiveParticles )
        // effectiveParticles.Remove(particle);

        // switch (Implementation) {
            // case Implementation.EulerMethod:
        // EulerMethod.PhysicsProcess(particle, MaxRadius, Particles.ToArray(), InteractionMatrix, delta, TimeScale);
                // break;
            // case Implementation.Verlet:
                // Verlet.PhysicsProcess(particle, MaxRadius, effectiveParticles.ToArray(), InteractionMatrix, delta, TimeScale);
                // break;
            // case Implementation.RK4:
                // RK4Integration.PhysicsProcess(particle, MaxRadius, effectiveParticles.ToArray(), InteractionMatrix, delta, TimeScale);
                // break;
        // }
        
        // Vector2 diff = particle.ParticlePosition - (MaxRadius * new Vector2(SpaceSize, SpaceSize) / 2);
        // float dstFromCenter = Math.Abs(diff.X) + Math.Abs(diff.Y);

        // particle.Velocity -= MathF.Sqrt(dstFromCenter) * diff.Normalized() * 0.01f;

        // Calculating the modulo operations
        // float maxX = SpaceSize;
        // float maxY = SpaceSize;
        // particle.ParticlePosition = new Vector2(
        //     MathUtils.Fmod(particle.ParticlePosition.X, maxX),
        //     MathUtils.Fmod(particle.ParticlePosition.Y, maxY)
        // );

        // if (particle.ParticlePosition.X < 0 || particle.ParticlePosition.X >= maxX) {
        //     particle.Velocity.X = 0;
        //     particle.ParticlePosition
        // }
        // if (particle.ParticlePosition.Y < 0 || particle.ParticlePosition.Y >= maxY) {
        //     particle.Velocity.Y = 0;
        // }

        // CellGrid.UpdateParticle(particle);
    // }
}
