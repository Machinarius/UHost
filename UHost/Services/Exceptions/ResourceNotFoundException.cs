using System;
using Microsoft.Azure.Documents;

namespace UHost.Services.Exceptions {
  public class ResourceNotFoundException : Exception {
    private DocumentClientException ex;

    public ResourceNotFoundException() {
    }

    public ResourceNotFoundException(DocumentClientException ex) {
      this.ex = ex;
    }
  }
}
