namespace Stasis.Engine.Scene;

public class SceneHandler {
    List<IScene> Scenes = [];

    List<IScene> RemovalQueue = [];
    List<IScene> AddQueue = [];

    public void UpdateAllScenes(Window window, double dt) {
        foreach(var scene in Scenes)
            scene.Update(window, dt);

        foreach(var scene in RemovalQueue)
            Scenes.Remove(scene);

        foreach(var scene in AddQueue)
            Scenes.Add(scene);

        AddQueue.Clear();
        RemovalQueue.Clear();
    }

    public void RenderAllScenes(Window window) {
        foreach(var scene in Scenes)
            scene.Render(window);
    }

    public void ClearScenes() {
        Scenes = new List<IScene>();
    }

    public void AddScene(IScene scene) {
        AddQueue.Add(scene);
    }

    public void RemoveSceneByType<T>() where T : IScene {
        for(int i = 0; i < Scenes.Count; i++) {
            if(Scenes[i] is T t) {
                RemovalQueue.Add(t);
                break;
            }
        }
    }

    public IScene? GetSceneByType<T>() where T : IScene {
        for(int i = 0; i < Scenes.Count; i++) {
            if(Scenes[i] is T t)
                return t;
        }
        return null;
    }

    public IScene? GetSceneByTypeIndexed<T>(int which) where T : IScene {
        int j = 0;
        for(int i = 0; i < Scenes.Count; i++) {
            if(Scenes[i] is T t) {
                if(j >= which) return t;
                else j++;
            }
        }
        return null;
    }
}