using System.Threading.Tasks;

namespace UHost.Services {
  public interface ISmsSender {
    Task SendSmsAsync(string number, string message);
  }
}
