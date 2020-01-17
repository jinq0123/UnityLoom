# UnityLoom
Loom class for Unity to simplify the interaction between the Unity main thread and the other threads.

## Threads on a Loom

Our class is called Loom.  Loom lets you easily run code on another thread and have that other thread run code on the main game thread when it needs to.

There are only two functions to worry about:

* RunAsync(Action) which runs a set of statements on another thread
* QueueOnMainThread(Action, [optional] float time) - which runs a set of statements on the main thread (with an optional delay).

You access Loom using Loom.Current - it deals with creating an invisible game object to interact with the games main thread.

## Usage
```
    //Scale a mesh on a second thread  
    void ScaleMesh(Mesh mesh, float scale)  
    {  
        //Get the vertices of a mesh  
        var vertices = mesh.vertices;  
        //Run the action on a new thread  
        Loom.RunAsync(()=>{  
            //Loop through the vertices  
            for(var i = 0; i < vertices.Length; i++)  
            {  
                //Scale the vertex  
                vertices[i] = vertices[i] * scale;  
            }  
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(()=>{  
                //Set the vertices  
                mesh.vertices = vertices;  
                //Recalculate the bounds  
                mesh.RecalculateBounds();  
            });  
       
        });  
    }  
```

## Reference

* https://www.iteye.com/blog/dsqiu-2028503

## Modification to the original version
* Don't limit thread number
* Initialize on access
* Check thread ID in Initialize()
* Don't destroy on load
* Don't check Application.isPlaying
* Don't try-catch in RunAction()
* Delete `QueueOnMainThread(Action action, float time)` because get_time can only be called from the main thread.
