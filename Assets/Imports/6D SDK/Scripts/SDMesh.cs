﻿/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SixDegrees
{
    public class SDMesh : MonoBehaviour
    {
        private class SDMeshChunk
        {
            public Vector3Int coordinates;
            public Dictionary<Vector3Int, SDMeshBlock> blocks;
            public GameObject gameObject;
            public Mesh sharedMesh;
            public MeshFilter meshFilter;
            public MeshRenderer meshRenderer;
            public MeshCollider meshCollider;
            public int meshVersion;
        }

        private class SDMeshBlock
        {
            public Vector3Int coordinates;
            public List<Vector3> vertices;
            public List<Vector3> normals;
            public List<Vector3Int> triangles;
            public int version;
            public int meshVersion;
        }

        public const int MeshLayer = 9;

        private int meshVersion = -1;
        private bool meshVisible = true;
        public GameObject meshPrefab;
        public Material depthMaskMaterial;

        [Range(0.2f, 10f)]
        public float chunkSizeInMeters = 1f;
        private float blockSize = 0f;
        private int blocksPerChunk = 0;
        private Dictionary<Vector3Int, SDMeshChunk> chunks;

        void Awake()
        {
            chunks = new Dictionary<Vector3Int, SDMeshChunk>();
        }

        void FixedUpdate()
        {
            if (!SDPlugin.IsSDKReady)
            {
                return; // will try later
            }

            int blockBufferSize = 0;
            int vertexBufferSize = 0;
            int faceBufferSize = 0;
            int newVersion = -1;
            unsafe 
            {
                newVersion = SDPlugin.SixDegreesSDK_GetMeshBlockInfo(&blockBufferSize, &vertexBufferSize, &faceBufferSize);
            }

            if (newVersion > meshVersion && blockBufferSize > 0 && vertexBufferSize > 0 && faceBufferSize > 0) 
            {
                if (meshVersion < 0) 
                {
                    blockSize = SDPlugin.SixDegreesSDK_GetMeshBlockSize();
                    blocksPerChunk = Mathf.Max(1, Mathf.FloorToInt(chunkSizeInMeters / blockSize));
                    chunkSizeInMeters = blocksPerChunk * blockSize;
                }
                UpdateMesh(newVersion, blockBufferSize, vertexBufferSize, faceBufferSize);
            } 
            else if (newVersion == 0 && meshVersion > 0 && blockBufferSize == 0) 
            {
                // Mesh was reset after loading a new map
                ClearMesh();
            }
        }

        Vector3Int GetBlockCoords(Vector3 coords)
        {
            return new Vector3Int(Mathf.FloorToInt(coords.x / blockSize), Mathf.FloorToInt(coords.y / blockSize), Mathf.FloorToInt(coords.z / blockSize));
        }

        Vector3Int GetChunkCoords(Vector3Int blockCoords)
        {
            return new Vector3Int(Mathf.FloorToInt(blockCoords.x / blocksPerChunk), Mathf.FloorToInt(blockCoords.y / blocksPerChunk), Mathf.FloorToInt(blockCoords.z / blocksPerChunk));
        }

        void UpdateMesh(int newVersion, int blockBufferSize, int vertexBufferSize, int faceBufferSize)
        {
            meshVersion = newVersion;

            int[] blockArray = new int[blockBufferSize];
            float[] vertexArray = new float[vertexBufferSize];
            int[] faceArray = new int[faceBufferSize];

            int fullBlocks = 0;

            unsafe
            {
                fixed (int* blockBufferPtr = &blockArray[0], faceBufferPtr = &faceArray[0])
                {
                    fixed (float* vertexBufferPtr = &vertexArray[0])
                    {
                        fullBlocks = SDPlugin.SixDegreesSDK_GetMeshBlocks(blockBufferPtr, vertexBufferPtr, faceBufferPtr,
                                                                          blockBufferSize, vertexBufferSize, faceBufferSize);
                    }
                }
            }
            bool gotAllBlocks = (fullBlocks == blockBufferSize / 6);

            if (fullBlocks < 0)
            {
                Debug.Log("Error calling SixDegreesSDK_GetMeshBlocks(), will not update the mesh.");
                return;
            }
            else if (fullBlocks == 0)
            {
                Debug.Log("SixDegreesSDK_GetMeshBlocks() gave us an empty mesh, will not update.");
                return;
            }
            else if (!gotAllBlocks)
            {
                Debug.Log("SixDegreesSDK_GetMeshBlocks() returned " + fullBlocks + " full blocks, expected " + (blockBufferSize / 6));
            }

            int firstVertex = 0;
            int firstTriangle = 0;
            HashSet<Vector3Int> chunksToUpdate = new HashSet<Vector3Int>();
            HashSet<Vector3Int> meshBlocks = new HashSet<Vector3Int>();

            // Update all the full blocks returned by the API
            int fullBlockSize = fullBlocks * 6;
            for (int b = 0; b + 5 < fullBlockSize; b += 6)
            {
                // Transform block coordinates from 6D right-handed coordinates to Unity left-handed coordinates
                // By flipping the sign of Z
                Vector3Int blockCoords = new Vector3Int(blockArray[b], blockArray[b + 1], -blockArray[b + 2]);
                Vector3Int chunkCoords = GetChunkCoords(blockCoords);
                int vertexCount = blockArray[b + 3];
                int triangleCount = blockArray[b + 4];
                int blockVersion = blockArray[b + 5];
                meshBlocks.Add(blockCoords);

                SDMeshChunk chunk = GetOrCreateChunk(chunkCoords);
                SDMeshBlock block = GetOrCreateBlock(chunk, blockCoords);

                // Update block if it is outdated
                if (block.version < blockVersion)
                {
                    chunksToUpdate.Add(chunkCoords);

                    block.version = blockVersion;
                    block.meshVersion = meshVersion;
                    block.vertices.Clear();
                    block.normals.Clear();
                    block.triangles.Clear();

                    // copy vertices
                    int fullVertices = 0;
                    int lastBlockVertex = Mathf.Min(vertexBufferSize, (firstVertex + vertexCount) * 6);
                    for (int va = firstVertex * 6; va + 5 < lastBlockVertex; va += 6)
                    {
                        // Transform vertices from 6D right-handed coordinates to Unity left-handed coordinates
                        // By flipping the sign of Z
                        block.vertices.Add(new Vector3(vertexArray[va], vertexArray[va + 1], -vertexArray[va + 2]));
                        block.normals.Add(new Vector3(vertexArray[va + 3], vertexArray[va + 4], -vertexArray[va + 5]));
                        fullVertices++;
                    }

                    if (fullVertices != vertexCount)
                    {
                        Debug.Log("Got only " + fullVertices + " vertices out of " + vertexCount);
                    }

                    // copy faces
                    int fullTriangles = 0;
                    int lastBlockTriangle = Mathf.Min(faceBufferSize, (firstTriangle + triangleCount) * 3);
                    for (int fa = firstTriangle * 3; fa + 2 < lastBlockTriangle; fa += 3)
                    {
                        block.triangles.Add(new Vector3Int(faceArray[fa] - firstVertex, faceArray[fa + 1] - firstVertex, faceArray[fa + 2] - firstVertex));
                        fullTriangles++;
                    }

                    if (fullTriangles != triangleCount)
                    {
                        Debug.Log("Got only " + fullTriangles + " triangles out of " + triangleCount);
                    }
                }

                firstVertex += vertexCount;
                firstTriangle += triangleCount;
            }

            // Clean up missing blocks only if we received all the expected full blocks
            if (gotAllBlocks)
            {
                foreach (SDMeshChunk chunk in chunks.Values)
                {
                    bool needsUpdate = false;
                    List<Vector3Int> blocks = new List<Vector3Int>(chunk.blocks.Keys);
                    foreach (Vector3Int block in blocks)
                    {
                        if (!meshBlocks.Contains(block)) {
                            needsUpdate = true;
                            chunk.blocks.Remove(block);
                        }
                    }
                    if (needsUpdate)
                    {
                        chunksToUpdate.Add(chunk.coordinates);
                    }
                }
            }

            // Update chunks with updated blocks
            UpdateChunks(chunksToUpdate);
        }

        SDMeshChunk GetOrCreateChunk(Vector3Int chunkCoords)
        {
            SDMeshChunk chunk = null;
            // Create new chunk if it's not part of the dict
            if (!chunks.TryGetValue(chunkCoords, out chunk) && meshPrefab)
            {
                GameObject chunkObject = GameObject.Instantiate(meshPrefab, Vector3.zero, Quaternion.identity);
                chunkObject.name = "Mesh Chunk " + chunkCoords.x + "," + chunkCoords.y + "," + chunkCoords.z;
                chunkObject.transform.SetParent(transform);
                Mesh sharedMesh = new Mesh();
                MeshFilter meshFilter = chunkObject.GetComponent<MeshFilter>();
                if (meshFilter) 
                {
                    meshFilter.sharedMesh = sharedMesh;
                }
                MeshRenderer meshRenderer = chunkObject.GetComponent<MeshRenderer>();
                if (meshRenderer && !meshVisible) 
                {
                    meshRenderer.material = depthMaskMaterial;
                }
                MeshCollider meshCollider = chunkObject.GetComponent<MeshCollider>();
                if (meshCollider) 
                {
                    meshCollider.sharedMesh = sharedMesh;
                }

                chunk = new SDMeshChunk
                {
                    coordinates = chunkCoords,
                    blocks = new Dictionary<Vector3Int, SDMeshBlock>(),
                    gameObject = chunkObject,
                    sharedMesh = sharedMesh,
                    meshFilter = meshFilter,
                    meshRenderer = meshRenderer,
                    meshCollider = meshCollider,
                    meshVersion = meshVersion
                };
                chunks.Add(chunkCoords, chunk);
            }

            return chunk;
        }

        SDMeshBlock GetOrCreateBlock(SDMeshChunk chunk, Vector3Int blockCoords)
        {
            SDMeshBlock block = null;
            // Create new block if it's not part of the dict
            if (!chunk.blocks.TryGetValue(blockCoords, out block))
            {
                block = new SDMeshBlock
                {
                    coordinates = blockCoords,
                    vertices = new List<Vector3>(),
                    normals = new List<Vector3>(),
                    triangles = new List<Vector3Int>(),
                    version = -1,
                    meshVersion = -1
                };
                chunk.blocks.Add(blockCoords, block);
            }

            return block;
        }

        public void ShowMesh()
        {
            meshVisible = true;
            Material baseMaterial = null;
            if (meshPrefab) 
            {
                MeshRenderer meshRenderer = meshPrefab.GetComponent<MeshRenderer>();
                if (meshRenderer) 
                {
                    baseMaterial = meshRenderer.material;
                }
            }
            if (!baseMaterial) 
            {
                return;
            }
            foreach (SDMeshChunk chunk in chunks.Values) 
            {
                if (chunk.meshRenderer)
                {
                    chunk.meshRenderer.material = baseMaterial;
                }
            }
        }

        public void HideMesh() 
        {
            meshVisible = false;
            foreach (SDMeshChunk chunk in chunks.Values)
            {
                if (chunk.meshRenderer)
                {
                    chunk.meshRenderer.material = depthMaskMaterial;
                }
            }
        }

        void ClearMesh()
        {
            meshVersion = 0;
            List<Vector3Int> allChunks = new List<Vector3Int>(chunks.Keys);
            DeleteChunks(allChunks);
        }

        void UpdateChunks(IEnumerable<Vector3Int> chunksToUpdate)
        {
            foreach (Vector3Int chunkCoords in chunksToUpdate)
            {
                UpdateChunk(chunkCoords);
            }
        }

        void UpdateChunk(Vector3Int chunkToUpdate)
        {
            SDMeshChunk chunk = null;
            if (chunks.TryGetValue(chunkToUpdate, out chunk))
            {
                chunk.meshVersion = meshVersion;
                if (!chunk.sharedMesh) 
                {
                    return;
                }

                List<Vector3> vertices = new List<Vector3>();
                List<Vector3> normals = new List<Vector3>();
                List<int> indices = new List<int>();
                List<Color32> colors = new List<Color32>();
                int offset = 0;

                foreach (SDMeshBlock block in chunk.blocks.Values)
                {
                    vertices.AddRange(block.vertices);
                    normals.AddRange(block.normals);

                    foreach (Vector3Int triangle in block.triangles)
                    {
                        indices.Add(triangle.x + offset);
                        indices.Add(triangle.y + offset);
                        indices.Add(triangle.z + offset);
                    }

                    offset += block.vertices.Count;

                    Color32 vertexColor = Color.HSVToRGB((block.meshVersion % 256) * 0.00392157f, 1f, 1f);
                    colors.AddRange(System.Linq.Enumerable.Repeat(vertexColor, block.vertices.Count));
                }

                if (vertices.Count > 0 && indices.Count > 0)
                {
                    chunk.sharedMesh.Clear();
                    
                    chunk.sharedMesh.vertices = vertices.ToArray();
                    chunk.sharedMesh.normals = normals.ToArray();
                    chunk.sharedMesh.triangles = indices.ToArray();
                    chunk.sharedMesh.colors32 = colors.ToArray();

                    if (chunk.meshFilter)
                    {
                        chunk.meshFilter.sharedMesh = chunk.sharedMesh;
                    }

                    if (chunk.meshCollider)
                    {
                        chunk.meshCollider.sharedMesh = chunk.sharedMesh;
                    }
                }
                else
                {
                    DeleteChunk(chunkToUpdate);
                }
            }
        }

        void DeleteChunks(IEnumerable<Vector3Int> chunksToDelete)
        {
            foreach (Vector3Int chunkCoords in chunksToDelete)
            {
                DeleteChunk(chunkCoords);
            }
        }

        void DeleteChunk(Vector3Int chunkToDelete)
        {
            SDMeshChunk chunk = null;
            if (chunks.TryGetValue(chunkToDelete, out chunk))
            {
                Destroy(chunk.meshFilter.sharedMesh);
                Destroy(chunk.gameObject);
                chunks.Remove(chunkToDelete);
            }
        }
    }
}
