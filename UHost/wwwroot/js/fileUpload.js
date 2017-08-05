function onUploadComplete(response) {
  location.reload();
}

function onUploadFailure(response) {
  $('#title-input').prop('disabled', false);
  $('#description-input').prop('disabled', false);
  $('#file-input').prop('disabled', false);
  $('#cancel-button').prop('disabled', false);
  $('#close-button').prop('disabled', false);
  $('#start-upload-button').prop('disabled', false);
  $('#progress-container').css('display', 'none');
  $('#upload-failure').css('display', 'block');
}

function startUpload() {
  $('#title-missing-error').css('display', 'none');
  $('#file-missing-error').css('display', 'none');
  $('#upload-failure').css('display', 'none');

  var formComplete = true;
  var selectedFiles = $('#file-input')[0].files;
  var file = null;

  if (!selectedFiles || !selectedFiles.length) {
    formComplete = false;
    $('#file-missing-error').css('display', 'block');
  } else {
    file = selectedFiles[0];
  }

  var title = $('#title-input').val();
  var description = $('#description-input').val();

  if (!title) {
    formComplete = false;
    $('#title-missing-error').css('display', 'block');
  }

  if (!file) {
    formComplete = false;
    $('#file-missing-error').css('display', 'block');
  }

  if (!formComplete) {
    return;
  }

  var uploadData = new FormData();
  uploadData.append('title', title);
  uploadData.append('description', description);
  uploadData.append('file', file);

  $.ajax({
    url: '/Files/Create',
    data: uploadData,
    cache: false,
    contentType: false,
    processData: false,
    type: 'POST',
    success: onUploadComplete,
    error: onUploadFailure
  });

  $('#title-input').prop('disabled', true);
  $('#description-input').prop('disabled', true);
  $('#file-input').prop('disabled', true);
  $('#cancel-button').prop('disabled', true);
  $('#close-button').prop('disabled', true);
  $('#start-upload-button').prop('disabled', true);
  $('#progress-container').css('display', 'block');
}

(function () {
  $('#start-upload-button').click(startUpload);
})();