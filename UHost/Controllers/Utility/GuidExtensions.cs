using System;

namespace UHost.Controllers.Utility {
  public static class GuidExtensions {
    public static string ToUrlSafeString(this Guid guid) {
      return Convert.ToBase64String(guid.ToByteArray()).Replace("/", "-").Replace("+", "_").Replace("=", "");
    }

    public static Guid ToGuid(this string guidString) {
      var fixedGuidString = guidString.Replace("-", "/").Replace("_", "+") + "==";
      try {
        var guidData = Convert.FromBase64String(fixedGuidString);
        var guid = new Guid(guidData);
        return guid;
      } catch (Exception ex) {
        throw new ArgumentException("Invalid guid string: " + guidString, nameof(guidString), ex);
      }
    }
  }
}
