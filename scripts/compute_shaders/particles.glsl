#[compute]
#version 460

struct Particle {
    float posX;
    float posY;
    float velX;
    float velY;
    uint type;
};

struct Params {
    uint numParticles;
    int xCells;
    int yCells;
    float delta;
    float maxRadius;
    int numColors;
    float timeScale;
};

struct QTree {
    bool isLeaf;
    int childIdx;
    int[10] p;
};

// Invocations in the (x, y, z) dimension
layout(local_size_x = 256, local_size_y = 1, local_size_z = 1) in;

// Structured buffers
layout(set = 0, binding = 0, std430) restrict buffer Particles {
    Particle data[];
}
particles;

layout(set = 0, binding = 1, std430) restrict readonly buffer ParamsBuffer
{
    Params params;
}
params_buffer;

layout(set = 0, binding = 2, std430) restrict readonly buffer AttrationMatrix
{
    float data[];
}
attrationMatrix_buffer;

// layout(set = 0, binding = 3, std430) restrict readonly buffer Cells {
//     Cell data[];
// }
// cells;

// layout(set = 0, binding = 4, std430) restrict readonly buffer CellMembership {
//     int data[];
// }
// cellMembership;

// layout(set = 0, binding = 5, std430) restrict readonly buffer FlatCelledIndices {
//     int data[];
// }
// flatCelledIndices;

float getAttraction(uint type1, uint type2) {
    return attrationMatrix_buffer.data[type1 * params_buffer.params.numColors + type2];
}

ivec2 getCellAt(vec2 point) {
    int x = int(floor(point.x / params_buffer.params.maxRadius));
    int y = int(floor(point.y / params_buffer.params.maxRadius));

    return ivec2(x, y);
}

float force(float r, float attraction) {
    float beta = 0.4;
    if (r < beta) {
        return r / beta - 1;
    } else if (beta < r && r < 1) {
        return attraction * (1 - abs(2 * r - 1 - beta) / (1 - beta));
    }
    return 0;
}

vec2 calculateForce(Particle particle, Particle other) {
    float maxRadius = params_buffer.params.maxRadius;
    float width = float(params_buffer.params.xCells) * maxRadius;
    float height = float(params_buffer.params.yCells) * maxRadius;

    vec2 otherPosition = vec2(other.posX, other.posY);

    float distX = otherPosition.x - particle.posX;
    float wrappedDistX = distX - sign(distX) * width;
    float distY = otherPosition.y - particle.posY;
    float wrappedDistY = distY - sign(distY) * height;

    float effectiveDistX = abs(distX) < abs(wrappedDistX) ? distX : wrappedDistX;
    float effectiveDistY = abs(distY) < abs(wrappedDistY) ? distY : wrappedDistY;

    vec2 difference = vec2(effectiveDistX, effectiveDistY);
    float dist = sqrt(effectiveDistX*effectiveDistX + effectiveDistY*effectiveDistY);

    if (dist <= 0.0 || dist >= maxRadius) return vec2(0);

    vec2 direction = difference / dist; // Normalized distance

    float attractionFactor = getAttraction(particle.type, other.type); // FIXME: Change the atraction
    float forceModule = force(dist / maxRadius, attractionFactor); 
    vec2 force = forceModule * direction;
    
    return force;
}
// vec2 calculateForceCell(Particle particle, int cellIndex) {
//     uint id = gl_GlobalInvocationID.x;
//     Cell cell = cells.data[cellIndex];

//     vec2 totalForce = vec2(0.0);
//     for (int i = cell.start; i < cell.end; i++) {
//         int index = flatCelledIndices.data[i];
//         if (index == id) continue;
//         Particle other = particles.data[index];
//         totalForce += calculateForce(particle, other);
//     }
//     return totalForce;
// }
vec2 calculateForces(Particle particle) {
    uint id = gl_GlobalInvocationID.x;
    float maxRadius = params_buffer.params.maxRadius;
    uint numParticles = params_buffer.params.numParticles;
    // int cellIndex = cellMembership.data[id];
    // Cell cell = cells.data[cellIndex];
    vec2 totalForce = vec2(0.0, 0.0);



    for (int i = 0; i < numParticles; i++) {
        Particle other = particles.data[i];
        if (particle.posX == other.posX && particle.posY == other.posY) continue;
        vec2 force = calculateForce(particle, other);
        totalForce += force;
    }
    // totalForce += calculateForceCell(particle, cellIndex, debug);
    // totalForce += calculateForceCell(particle, cell.left, debug);
    // totalForce += calculateForceCell(particle, cell.upLeft, debug);
    // totalForce += calculateForceCell(particle, cell.up, debug);
    // totalForce += calculateForceCell(particle, cell.upRight, debug);
    // totalForce += calculateForceCell(particle, cell.right, debug);
    // totalForce += calculateForceCell(particle, cell.downRight, debug);
    // totalForce += calculateForceCell(particle, cell.down, debug);
    // totalForce += calculateForceCell(particle, cell.downLeft, debug);

    return totalForce * maxRadius;
}

void main() {
    uint numParticles = params_buffer.params.numParticles;
    float maxRadius = params_buffer.params.maxRadius;
    float width = float(params_buffer.params.xCells * maxRadius);
    float height = float(params_buffer.params.yCells * maxRadius);
    float timeScale = params_buffer.params.timeScale;
    float delta = 0.006060606061;

    uint id = gl_GlobalInvocationID.x;

    if (id >= numParticles) {
        return;
    }
    Particle particle = particles.data[id];

    vec2 pos = vec2(particle.posX, particle.posY);
    vec2 vel = vec2(particle.velX, particle.velY);

    float frictionFactor = 0.9;
    vec2 acceleration = calculateForces(particle) * 5;
    // Only euler's method for now
    vec2 newVel = vel * frictionFactor + acceleration * delta;
    vec2 newPos = pos + newVel * delta;
    // Wrap...
    newPos.x = mod(newPos.x, width);
    newPos.y = mod(newPos.y, height);

    particles.data[id] = Particle(newPos.x, newPos.y, newVel.x, newVel.y, particle.type); // int(sqrt(acceleration.x*acceleration.x + acceleration.y*acceleration.y) *)
}


