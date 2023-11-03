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

// Function to convert a time tag to local date and time
function convertTimeTagsToLocal() {
    const timeTags = document.getElementsByTagName('time');

    for (const timeTag of timeTags) {
        const datetime = timeTag.getAttribute('datetime') + " UTC";
        if (datetime) {
            const utcDate = new Date(datetime);
            const localDate = new Date(utcDate.toLocaleString());

            // Format the localDate to "yyyy/MM/dd HH:mm" format
            const formattedDate = formatDateTIme(localDate);

            timeTag.textContent = formattedDate;
        }
    }
}

function formatDateTIme(date){
    const year = date.getFullYear();
    const month = (1 + date.getMonth()).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const seconds = date.getSeconds().toString().padStart(2, '0');
    const formattedDate = `${hours}:${minutes}:${seconds} ${day}/${month}/${year}`;
    return formattedDate;
}


// Call the conversion function
convertTimeTagsToLocal();