function previewImage(input) {
    const preview = document.getElementById('image-preview');
    const imageContainer = document.getElementById('image-container');
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            imageContainer.style.display = 'none'
            $("#image-validation-message").empty()
        }

        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = '#';
        preview.style.display = 'none';
        imageContainer.style.display = 'block'
    }
}