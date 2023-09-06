// Make sure this file is not included twice
#ifndef GRASS_TRAMPLE_INCLUDED
#define GRASS_TRAMPLE_INCLUDED

// Trample global variables
float _NumGrassTramplePositions;
// The length should match the max value in the renderer feature
float4 _GrassTramplePositions[8];

void CalculateTrample_float(float3 WorldPosition, out float distance) {
    distance = 0;
#ifndef SHADERGRAPH_PREVIEW
    // For each trample position
    for (int i = 0; i < _NumGrassTramplePositions; i++) {
        // Find the distance to the trample position
        float3 objectPositionWS = _GrassTramplePositions[i].xyz;
        float3 distanceVector = WorldPosition - objectPositionWS;
        distance = length(distanceVector);
    }
#endif
}


void GetPlayerPos_float(out float3 playerPos) {
    playerPos = (0,0,0,0);
#ifndef SHADERGRAPH_PREVIEW
    // For each trample position
    for (int i = 0; i < _NumGrassTramplePositions; i++) {
        // Find the distance to the trample position
        playerPos = _GrassTramplePositions[i].xyz;
    }
#endif
}

#endif