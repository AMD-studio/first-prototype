namespace Climbing.DependencyInjection
{
    public interface IActionLoader
    {
        public Action LoadAction(string actionPath);
    }
}