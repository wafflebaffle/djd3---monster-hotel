/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using LibGameAI.FSMs;

// The script that controls an agent using an FSM
public abstract class AIBehaviour : MonoBehaviour
{
    // Create the FS
    protected abstract void Start();

    // Request actions to the FSM and perform them
    protected abstract void Update();
}