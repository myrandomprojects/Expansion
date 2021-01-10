#ifdef _WIN32

#include "matrix.h"

#endif

typedef struct {
	int2 binSize;
	int2 binsCount;
} BinSettings;

bool rectanglesIntersect(const float4 a, const float4 b)
{
	float2 pt = (b.xy + b.zw);

	return (pt.x >= a.x && pt.x <= a.x + a.z + b.z
		&& pt.y >= a.y && pt.y <= a.y + a.w + b.w);
}

kernel void bin(
	//global const Triangle* triangles,
	global const TriangleBounds* triBounds,
	global WorldSettings* worldSettings,
	global Batch* batches
) {
	const unsigned int batchCount = get_global_size(0);
	const unsigned int batchId = get_global_id(0);
	const unsigned int localId = get_global_id(1);
	const int2 binCount = (int2)((worldSettings->camera.screenSize.x + 15) / 16, (worldSettings->camera.screenSize.y + 15) / 16);


	local TriangleBounds boundsCache[255];

	barrier(CLK_LOCAL_MEM_FENCE);
	event_t event = async_work_group_copy((local char*)boundsCache,
		(global const char*) (triBounds + 255*batchId),
		255*sizeof(TriangleBounds), 0);
	wait_group_events(1, &event);

	const int binsTotal = binCount.x * binCount.y;
	for (int iBin = localId; iBin < binsTotal; iBin += 256)
	{
		Batch batch;
		batch.vec = (ulong4)0;

		const float4 binLoc = (float4)(iBin % binCount.x * 16, iBin / binCount.x * 16, 16, 16);
		
		int inclTri = 0;

		for (int iTri = 0; iTri < 255; iTri++)
		{
			if (boundsCache[iTri].bounds.x == -1)
				continue;

			if (!rectanglesIntersect(boundsCache[iTri].bounds, binLoc))
				continue;

			batch.bytes[iTri / 8] |= 1 << (iTri % 8);
			inclTri = 128;
		}

		batch.bytes[31] |= inclTri;
		vstore4(batch.vec, batchId + iBin * batchCount, (global ulong*)batches);
	}
}
