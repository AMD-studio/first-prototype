using UnityEngine;

namespace Climbing.DependencyInjection
{
    public class ActionLoader : IActionLoader
    {
        public Action LoadAction(string actionPath) => Resources.Load<Action>(actionPath);
    }
}
