/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * Was edited by Lisa Carvalho for a 3D game.
 * */

using UnityEngine;
using System.Collections;

public class RoomArea : MonoBehaviour
{
    public float Xmax { get; private set; }
    public float Xmin { get; private set; }
    public float Zmax { get; private set; }
    public float Zmin { get; private set; }

    [SerializeField] private Collider mesh;
    // Configure game area limits object
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.9f);
        // Get the sprite renderer and its bounds
        mesh = GetComponent<Collider>();
        Bounds bounds = mesh.bounds;

        // Determine and keep game area limits
        Xmax = bounds.min.x;
        Xmin = bounds.max.x;
        Zmax = bounds.min.z;
        Zmin = bounds.max.z;
    }

    // Determine random position within game area
    public Vector3 RandomPosition(float margin)
    {
        return new Vector3(
            Random.Range(Xmin * margin, Xmax * margin), 0,
            Random.Range(Zmin * margin, Zmax * margin));
    }

    // Determine opposite position within game area
    public Vector2 OppositePosition(char wall, Vector2 currentPos)
    {
        Vector2 newPosition = wall switch
        {
            'E' => new Vector2(Xmax * 0.9f, currentPos.y),
            'W' => new Vector2(Xmin * 0.9f, currentPos.y),
            'N' => new Vector2(currentPos.x, Zmax * 0.9f),
            'S' => new Vector2(currentPos.x, Zmin * 0.9f),
            _ => new Vector2(),
        };
        return newPosition;
    }
}
