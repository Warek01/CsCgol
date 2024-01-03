namespace CsGame.Models;

public interface IGame : IDisposable
{
  void Start();
  void SetNextScene<TSceneClass>() where TSceneClass : Scene;
  void SetScene<TSceneClass>() where TSceneClass : Scene;
}
